using Celeste;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;
using Celeste.Mod.Entities;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    [CustomEntity("MeliHelper/BattleCityTank")]
    class Enemy : Solid
    {
        Field field;
        Level level;
        EnemyTypeOptions opts;
        Sprite sprite_explosion;
        Player player;
        Flag flag;
        CustomTimer timer_shoot, timer_bonuses_blink, timer_frames, timer006;
        int frame_id, lifes, points;
        float texture_angle, timer_hit, koef_dirt;
		bool is_dead, is_show_hit, is_armored, is_on_dirt;
        bool is_contains_bonus, is_show_bonus, is_bonus_pos_random;


        BCEnum_EnemyState state;
		DirectionEnum dir_enum;
        Vector2 dir, dest;
		float speed_move, path_length, path_saved;
        int cx, cy, tx, ty;
        bool is_need_to_register;
		static int dx, dy, nx, ny;
        const int sx = 4, sy = 4;

        public Enemy(EntityData data, Vector2 offset)
            : base(data.Position + offset - new Vector2(8, 8), 16, 16, false)
        {
            string type = data.Attr("tankType", "Basic");
            this.opts = new EnemyTypeOptions(
                id: type[0],
                type: (BCEnum_EnemyType)Enum.Parse(typeof(BCEnum_EnemyType), type),
                points: data.Int("points"),
                health: data.Int("health"),
                speed_move: data.Float("speedMove"),
                speed_bullets: data.Float("speedBullets"),
                shoot_frequency: data.Float("shootFrequency", 2.4f),
                is_can_break_through_steel: data.Bool("canBreakThroughSteel")
                );
            this.points = opts.points;
            this.dest = Position;
            this.speed_move = opts.speed_move;
            this.lifes = opts.health;
            this.is_contains_bonus = data.Bool("containsBonus");
            this.is_bonus_pos_random = data.Bool("bonusRandomPosition", true);
            this.is_need_to_register = true;
        }

        public Enemy(Field field, Vector2 center, EnemyTypeOptions opts, bool is_contains_bonus)
            : base(center - new Vector2(8, 8), 16, 16, false)
        {
            this.field = field;
            this.opts = opts;
            this.points = opts.points;
            this.dest = Position;
            this.speed_move = opts.speed_move;
            this.lifes = opts.health;
            this.is_contains_bonus = is_contains_bonus;
            this.is_bonus_pos_random = true;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = scene as Level;
            player = scene.Tracker.GetEntity<Player>();
            flag = scene.Tracker.GetEntity<Flag>();
            this.koef_dirt = 1;
            this.is_show_bonus = is_contains_bonus;
            this.is_armored = (lifes > 1);
            this.timer_shoot = new CustomTimer(opts.shoot_frequency);
            this.timer_bonuses_blink = new CustomTimer(0.32f);
            this.timer_frames = new CustomTimer(0.04f);
            timer006 = new CustomTimer(0.06f);
            this.OnDashCollide += onDashCollide;
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            if (is_need_to_register)
            {
                field = Field.Instance;
                field.GetEnemiesComponent.RegisterEnemy(this);
            }
            this.cx = field.GetCellCX(Position);
            this.cy = field.GetCellCY(Position);
            this.tx = field.GetTileCX(Position);
            this.ty = field.GetTileCY(Position);
        }

        public override void Update()
        {
            base.Update();
            if (is_contains_bonus && timer_bonuses_blink.Tick())
                is_show_bonus = !is_show_bonus;
            if (timer_hit > 0)
            {
                timer_hit -= Engine.DeltaTime;
                if (timer_hit <= 0)
                    is_show_hit = false;
            }


            if (is_dead)
            {
                if (sprite_explosion != null && sprite_explosion.CurrentAnimationID == "stop")
                {
                    if (points > 0)
                        level.Add(new FloatyPointsEntity(Position, points));
                    RemoveSelf();
                }
                return;
            }
            if (field.GetGameState != BCEnum_GameState.Normal || field.GetEventUI.isEventExists(BCEnum_BonusEvent.TimeStop))
                return;


            //--------------------------------------
            // Moving
            this.cx = field.GetCellCX(Position);
            this.cy = field.GetCellCY(Position);
            this.tx = field.GetTileCX(Position);
            this.ty = field.GetTileCY(Position);
            switch (state)
            {
                case BCEnum_EnemyState.Wait:
                    if (timer006.Tick())
                    {
                        //DirectionEnum dir_enum = GetRandomDirection();  // no need to be overintellectual... it's a demo after all!
                        DirectionEnum dir_enum = GetNextDirection(BCEnum_EnemyBehaviour.PrioritizeFlag);  // no need to be overintellectual... it's a demo after all!
                        Methods.GetDirectionParams(dir_enum, ref dx, ref dy);
                        this.dir = new Vector2(dx, dy);
                        this.texture_angle = dir.Angle() + MathExt.PI2 / 4;

                        int path_length = GetPathLength(dir);
                        if (path_length > 0)
                        {
                            int l2 = Math.Min(16, path_length);
                            while (l2 < path_length)
                                if (Methods.GetRandomizer().NextDouble() < 1 - 0.005f * l2) l2 += 8;
                                else break;

                            this.state = BCEnum_EnemyState.Move;
                            this.dir_enum = dir_enum;
                            this.dir = new Vector2(dx, dy);
                            this.dest = Position + dir * l2;
                            this.texture_angle = dir.Angle() + MathExt.PI2 / 4;
                            this.path_length = l2;
                            this.path_saved = l2;
                        }
                    }
                    break;

                case BCEnum_EnemyState.Move:
                    float max_len = speed_move * koef_dirt * Engine.DeltaTime;
                    if (max_len >= path_length)
                    {
                        MoveH(dest.X - Position.X);
                        MoveV(dest.Y - Position.Y);
                        state = BCEnum_EnemyState.Wait;
                        path_length = 0;
                    }
                    else
                    {
                        Vector2 del = max_len * dir;
                        Vector2 location_new = Position + del;
                        nx = field.GetTileCX(location_new);
                        ny = field.GetTileCY(location_new);
                        if (field.isZoneFree(nx, ny, 4, 4))
                        {
                            MoveH(del.X);
                            MoveV(del.Y);
                            //Position = location_new;
                            path_length -= max_len;
                        }
                        else
                        {
                            state = BCEnum_EnemyState.Wait;
                            path_length = 0;
                        }
                    }



                    if (timer_frames.Tick())
                        frame_id = 1 - frame_id;

                    //if (Scene.OnRawInterval(0.1f))
                    //Audio.Play();
                    break;
            }
            

            if (is_on_dirt)
                is_on_dirt = false;
            else if (koef_dirt >= 1f)
            {
                koef_dirt -= 0.6f * Engine.DeltaTime;
                if (koef_dirt < 1) koef_dirt = 1;
            }



            if (MeliHelperModule.Settings.Debug_EnemiesPoisoned && Scene.OnInterval(0.5f))
                TakeDamage(true);
            if (MeliHelperModule.Settings.Debug_EnemiesShootingEndlessly && Scene.OnInterval(0.3f))
                Shoot();

            if (timer_shoot.Tick())
                Shoot();
        }

        int GetPathLength(Vector2 dir)
        {
            int path = 0;
            while (true)
            {
                Vector2 pos = Position + (path + 4) * dir;
                if (!level.IsInBounds(pos))
                    return path - 4;

                Rectangle rect = new Rectangle((int)pos.X, (int)pos.Y, (int)Width, (int)Height);
                List<Solid> list_solids = level.CollideAll<Solid>(rect).FindAll(t => !(t is Enemy));
                if (list_solids.Count == 0)
                    path += 4;
                else 
                    return path;
            }
        }

        public DirectionEnum GetRandomDirection()
		{
			float chance = Methods.GetRandomizer().NextFloat();
            if (chance < 0.25) return DirectionEnum.Left;
            if (chance < 0.50) return DirectionEnum.Right;
            if (chance < 0.75) return DirectionEnum.Up;
            return DirectionEnum.Down;
        }


        public DirectionEnum GetNextDirection(BCEnum_EnemyBehaviour behaviour)
        {
            float chance = Methods.GetRandomizer().NextFloat();

            switch (behaviour)
            {
                case BCEnum_EnemyBehaviour.Random:
                    return GetRandomDirection();

                case BCEnum_EnemyBehaviour.PrioritizePlayer:
                    if (chance < 0.3f)
                        return GetNextDirection(BCEnum_EnemyBehaviour.FreakyPlayer);
                    return GetRandomDirection();

                case BCEnum_EnemyBehaviour.PrioritizeFlag:
                    if (chance < 0.3f)
                        return GetNextDirection(BCEnum_EnemyBehaviour.FreakyFlag);
                    return GetRandomDirection();

                case BCEnum_EnemyBehaviour.FreakyPlayer:
                    if (!Methods.PlayerIsAlive(player)) return GetRandomDirection();
                    if (chance < 0.5)
                        return (player.Position.X < this.Position.X) ? DirectionEnum.Left : DirectionEnum.Right;
                    else
                        return (player.Position.Y < this.Position.Y) ? DirectionEnum.Up : DirectionEnum.Down;

                case BCEnum_EnemyBehaviour.FreakyFlag:
                    if (flag == null) return GetRandomDirection();
                    if (chance < 0.5)
                        return (flag.Position.X < this.Position.X) ? DirectionEnum.Left : DirectionEnum.Right;
                    else
                        return (flag.Position.Y < this.Position.Y) ? DirectionEnum.Up : DirectionEnum.Down;
            }

            return DirectionEnum.Down;
        }


        public override void Render()
        {
            if (MeliHelperModule.Settings.Debug_ShowEnemyAhhhhMovebox)
            {
                Vector2 tile = field.GetTilePosition(tx, ty);
                switch (state)
                {
                    case BCEnum_EnemyState.Wait: Draw.Rect(dest, Field.PIX_TILE * sx, Field.PIX_TILE * sy, Color.Red * 0.4f); break;
                    case BCEnum_EnemyState.Move: Draw.Rect(dest, Field.PIX_TILE * sx, Field.PIX_TILE * sy, Color.Red * 0.4f); break;
                }
                Draw.HollowRect(tile, Field.PIX_TILE * sx, Field.PIX_TILE * sy, Color.Red);
                Draw.Line(dest + new Vector2(8,8), tile + new Vector2(8, 8), Color.Purple);
                if (koef_dirt != 1)
                    ActiveFont.Draw("dirt:" + koef_dirt.ToString("0.00"), dest, new Vector2(0.5f), new Vector2(0.3f), Color.White);
            }

            
            base.Render();
            
            if (!is_dead)
            {
                if (true)
                    GFX.Game["Evidence02/objects_bc/tanks/gray" + opts.id_sprite.ToString() + frame_id.ToString()]
                        .Draw(Position + new Vector2(7.5f), new Vector2(7.5f), Color.White, scale: 1f, rotation: texture_angle);
                if (is_contains_bonus && is_show_bonus)
                    GFX.Game["Evidence02/objects_bc/tanks/red"   + opts.id_sprite.ToString() + frame_id.ToString()]
                        .Draw(Position + new Vector2(7.5f), new Vector2(7.5f), Color.White, scale: 1f, rotation: texture_angle);
                else if (timer_hit > 0 && is_show_hit)
                    GFX.Game["Evidence02/objects_bc/tanks/yellow" + opts.id_sprite.ToString() + frame_id.ToString()]
                        .Draw(Position + new Vector2(7.5f), new Vector2(7.5f), Color.White, scale: 1f, rotation: texture_angle);
                else if (is_armored)
                    GFX.Game["Evidence02/objects_bc/tanks/green" + opts.id_sprite.ToString() + frame_id.ToString()]
                        .Draw(Position + new Vector2(7.5f), new Vector2(7.5f), Color.White, scale: 1f, rotation: texture_angle);
            }

            //ActiveFont.Draw(path_saved.ToString(), Position, Vector2.Zero, new Vector2(0.25f), Color.White);
        }

        protected virtual DashCollisionResults onDashCollide(Player player, Vector2 dir)
        {
            if (is_dead)
                return DashCollisionResults.Ignore;

            TakeDamage(true);
            player.RefillDash();
            return DashCollisionResults.Bounce;
        }

        public void Shoot()
        {
            level.Add(new Bullet(
                parent: this,
                position: this.Center + 6 * dir,
                speed: opts.speed_bullets * dir,
                color: Color.Gray,
                can_break_steel: opts.is_can_break_through_steel,
                is_player_bullet: false));
        }

        public bool TakeDamage(bool save_to_statistic)
        {
            if (is_dead)
                return false;

            if (is_contains_bonus)
            {
                is_contains_bonus = false;
                this.Scene.Add(new BonusDefault(
                    is_bonus_pos_random ? Field.Instance.GetPositionForBonus() : this.Center,
                    BonusesController.GetRandomBonus(),
                    12f
                    ));
            }

            if (--lifes <= 0)
                Die(save_to_statistic);
            else
            {
                Audio.Play(SoundController.BC_FIRING_THE_ENEMY_BIG_TANK);
                is_show_hit = true;
                timer_hit = 0.32f;
            }
            return true;
        }

        public void Die(bool is_save_to_statistic)
        {
            if (!is_dead)
            {
                is_dead = true;
                if (!is_save_to_statistic) points = 0;
                Audio.Play(SoundController.BC_ENEMY_DESTROYED);
				Add(sprite_explosion = GFX.SpriteBank.Create("MeliHelper_BC_ExplosionBig"));
                sprite_explosion.Position += new Vector2(8, 8);
                Field.Instance.GetEnemiesComponent.KillEnemy(this, is_save_to_statistic);
                ProgressController.AddPoints(points);
            }
        }

        public void UpdateOnDirt()
        {
            is_on_dirt = true;
            koef_dirt += 0.8f * Engine.DeltaTime;
            if (koef_dirt >= 2f) koef_dirt = 2f; 
        }

        public bool isOnTile(int tx, int ty)
        {
            return !is_dead 
                && this.tx <= tx && tx < this.tx + sx 
                && this.ty <= ty && ty < this.ty + sy;
        }

        public EnemyTypeOptions GetOpts
        {
            get
            {
                return opts;
            }
        }
    }
}
