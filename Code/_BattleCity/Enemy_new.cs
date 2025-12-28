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
    //[CustomEntity("MeliHelper/BattleCityTank")]
    class Enemy_new : Solid
    {
        Field field;
        Level level;
        EnemyTypeOptions opts;
        Sprite sprite_explosion;
        Player player;
        Flag flag;
        CustomTimer timer_shoot, timer_bonuses_blink, timer_frames, timer006;
        int frame_id, lifes, points;
        float texture_angle, timer_hit, koef_dirt, speed_move;
		bool is_dead, is_show_hit, is_armored, is_on_dirt, is_need_to_register;
        bool is_contains_bonus, is_show_bonus, is_bonus_pos_random;

        Dictionary<DirectionEnum, int> list_possible_dirs;  // dir --> priority
        DirectionEnum dir_current, dir_prev;
        Vector2 speed_current;
        float delay_move, delay_move_next;
        bool is_moving;

        public Enemy_new(EntityData data, Vector2 offset)
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
            this.speed_move = opts.speed_move;
            this.lifes = opts.health;
            this.is_contains_bonus = data.Bool("containsBonus");
            this.is_bonus_pos_random = data.Bool("bonusRandomPosition", true);
            this.is_need_to_register = true;
        }

        public Enemy_new(Field field, Vector2 center, EnemyTypeOptions opts, bool is_contains_bonus)
            : base(center - new Vector2(8, 8), 16, 16, false)
        {
            this.field = field;
            this.opts = opts;
            this.points = opts.points;
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
            list_possible_dirs = new Dictionary<DirectionEnum, int>();
            this.koef_dirt = 1;
            this.is_armored = (lifes > 1);
            this.timer_shoot = new CustomTimer(opts.type == BCEnum_EnemyType.Power ? 1.8f : 2.4f);
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
                //field.GetEnemiesComponent.RegisterEnemy(this);
            }
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
                        level.Add(new TextOutlineEntity(Position, points.ToString(), Color.White));
                    RemoveSelf();
                }
                return;
            }
            if (field.GetGameState != BCEnum_GameState.Normal || field.GetEventUI.isEventExists(BCEnum_BonusEvent.TimeStop))
                return;


            //--------------------------------------
            // Moving
            if (delay_move > 0)
                delay_move -= Engine.DeltaTime;
            else if (!is_moving)
            {
                dir_current = UpdateListPossibleDirections();
                speed_current = speed_move * Methods.DirectionToVector(dir_current);
                texture_angle = speed_current.Angle() + MathExt.PI2 / 4;
                is_moving = true;
            }
            else
            {
                Vector2 del = speed_current * Engine.DeltaTime;
                Rectangle rect = new Rectangle((int)(Position.X + del.X), (int)(Position.Y + del.Y), (int)Width, (int)Height);
                List<Solid> list_solids = level.CollideAll<Solid>(rect).FindAll(t => !(t is Enemy));
                if (list_solids.Count == 0)
                {
                    MoveH(del.X);
                    MoveV(del.Y);
                    if (timer_frames.Tick()) frame_id = 1 - frame_id;
                }
                else
                {
                    is_moving = false;
                    delay_move = delay_move_next;
                    delay_move_next += 1;
                }
            }
            


            //--------------------------------------------
            // Dirt
            if (is_on_dirt)
                is_on_dirt = false;
            else if (koef_dirt >= 1f)
            {
                koef_dirt -= 0.6f * Engine.DeltaTime;
                if (koef_dirt < 1) koef_dirt = 1;
            }

            // Shoot
            if (timer_shoot.Tick())
                Shoot();

            if (MeliHelperModule.Settings.Debug_EnemiesPoisoned && Scene.OnInterval(0.5f))
                TakeDamage(true);
            if (MeliHelperModule.Settings.Debug_EnemiesShootingEndlessly && Scene.OnInterval(0.3f))
                Shoot();
        }

        DirectionEnum UpdateListPossibleDirections()
        {
            list_possible_dirs.Clear();
            foreach (DirectionEnum item in Enum.GetValues(typeof(DirectionEnum)))
            {
                Vector2 dir = Methods.DirectionToVector(item);
                Vector2 pos = Position + 8 * dir;
                Rectangle rect = new Rectangle((int)pos.X, (int)pos.Y, (int)Width, (int)Height);

                List<Solid> list_solids = level.CollideAll<Solid>(rect).FindAll(t => !(t is Enemy));
                if (list_solids.Count == 0)
                {
                    int priority = 0;
                    //if (opts.type != BCEnum_EnemyType.Basic && player != null)
                    if (player != null)
                    {
                        Vector2 to_player = player.Center - this.Center;
                        if (Math.Sign(to_player.X) == Math.Sign(dir.X) || Math.Sign(to_player.Y) == Math.Sign(dir.Y))
                            priority++;
                    }

                    if (opts.type == BCEnum_EnemyType.Power)
                    {
                        foreach (Flag flag in level.Entities.FindAll<Flag>())
                        {
                            Vector2 to_flag = flag.Center - this.Center;
                            if (Math.Sign(to_flag.X) == Math.Sign(dir.X) || Math.Sign(to_flag.Y) == Math.Sign(dir.Y))
                                priority++;
                        }
                    }

                    // No back!
                    if (   item == DirectionEnum.Left  && dir_prev == DirectionEnum.Right
                        || item == DirectionEnum.Right && dir_prev == DirectionEnum.Left
                        || item == DirectionEnum.Up    && dir_prev == DirectionEnum.Down
                        || item == DirectionEnum.Down  && dir_prev == DirectionEnum.Up)
                        priority--;

                    list_possible_dirs[item] = priority;
                }
            }


            if (list_possible_dirs.Count > 0)
            {
                delay_move_next = 0;
                delay_move = 0;
                dir_prev = dir_current;

                int max_priority = list_possible_dirs.Max(t => t.Value);
                foreach (var item in list_possible_dirs)
                    if (item.Value == max_priority)
                        return item.Key;

                return DirectionEnum.Down;
            }
            else
            {
                switch (Calc.Random.Next(0, 4))
                {
                    case 0: return DirectionEnum.Left;
                    case 1: return DirectionEnum.Right;
                    case 2: return DirectionEnum.Up;
                    default: return DirectionEnum.Down;
                }
            }
        }


        public override void Render()
        {
            if (MeliHelperModule.Settings.Debug_ShowEnemyAhhhhMovebox)
            {
                foreach (var item in list_possible_dirs)
                    ActiveFont.Draw(item.Value.ToString(), 
                        Center + 16 * Methods.DirectionToVector(item.Key), 
                        new Vector2(0.5f), new Vector2(0.2f), Color.White);
                Draw.Line(Center, Center + 16 * Methods.DirectionToVector(dir_current), is_moving ? Color.Red : Color.Gray, thickness: 3f);


                //Vector2 tile = field.GetTilePosition(tx, ty);
                //Draw.Rect(dest, Field.PIX_TILE * sx, Field.PIX_TILE * sy, Color.Red * 0.4f);
                //Draw.HollowRect(tile, Field.PIX_TILE * sx, Field.PIX_TILE * sy, Color.Red);
                //Draw.Line(dest + new Vector2(8,8), tile + new Vector2(8, 8), Color.Purple);
                if (koef_dirt != 1)
                    ActiveFont.Draw("dirt:" + koef_dirt.ToString("0.00"), Position, new Vector2(0.5f), new Vector2(0.3f), Color.White);
            }

            
            base.Render();
            
            if (!is_dead)
            {
                if (true)
                    GFX.Game["Evidence02/objects_bc/tanks/gray"   + opts.id_sprite + frame_id.ToString()]
                        .Draw(Position + new Vector2(8, 8), new Vector2(8, 8), Color.White, scale: 1f, rotation: texture_angle);
                if (is_contains_bonus && is_show_bonus)
                    GFX.Game["Evidence02/objects_bc/tanks/red"    + opts.id_sprite + frame_id.ToString()]
                        .Draw(Position + new Vector2(8, 8), new Vector2(8, 8), Color.White, scale: 1f, rotation: texture_angle);
                else if (timer_hit > 0 && is_show_hit)
                    GFX.Game["Evidence02/objects_bc/tanks/yellow" + opts.id_sprite + frame_id.ToString()]
                        .Draw(Position + new Vector2(8, 8), new Vector2(8, 8), Color.White, scale: 1f, rotation: texture_angle);
                else if (is_armored)
                    GFX.Game["Evidence02/objects_bc/tanks/green"  + opts.id_sprite + frame_id.ToString()]
                        .Draw(Position + new Vector2(8, 8), new Vector2(8, 8), Color.White, scale: 1f, rotation: texture_angle);
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
                position: this.Center + 6 * Methods.DirectionToVector(dir_current),
                speed: opts.speed_bullets * Methods.DirectionToVector(dir_current),
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
                //Field.Instance.GetEnemiesComponent.KillEnemy(this, is_save_to_statistic);
                ProgressController.AddPoints(points);
            }
        }

        public void UpdateOnDirt()
        {
            is_on_dirt = true;
            koef_dirt += 0.8f * Engine.DeltaTime;
            if (koef_dirt >= 2f) koef_dirt = 2f; 
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
