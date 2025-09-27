using Celeste;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    class Bullet : Entity
    {
        static MTexture texture = GFX.Game["Evidence02/objects_bc/bullets/idle"];
        Level level;
        Field field;
        Entity parent;
        Color color;
        Vector2 speed, dir;
        float angle;
        bool is_can_break_steel, is_player_bullet, is_shadow_bullet;

        public Bullet(Entity parent, Vector2 position, Vector2 speed, Color color,
            bool can_break_steel = false, bool is_player_bullet = false, bool is_shadow_bullet = false) : base(position) 
        {
            this.parent = parent;
            this.color = color;
            this.speed = speed;
            this.dir = Vector2.Normalize(speed);
            this.angle = speed.Angle() + (float)Math.PI / 2;
            this.is_player_bullet = is_player_bullet;
            this.is_shadow_bullet = is_shadow_bullet;
            this.is_can_break_steel = can_break_steel;
            Collider = new Hitbox(4, 4, -2, -2);
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = scene as Level; // Engine.Scene as Level
            field = Field.Instance;
            if (field == null)
            {
                Die(false);
                return;
            }

            if (!is_player_bullet)
                Add(new PlayerCollider(onPlayer, new Hitbox(6, 6, -3, -3)));
        }

        public override void Update()
        {
            base.Update();
            if (!Methods.isInCamera(level, Center, 20) || !level.IsInBounds(this.Center))
            {
                RemoveSelf();
                return;
            }


            int del = (int)(speed.Length() * Engine.DeltaTime / 2);
            if (del == 0) del = 1;
            for (int i = 0; i < del; i++)
            {
                Position += speed * Engine.DeltaTime / del;
                if (CheckCollisions())
                    break;
            }
        }

        bool CheckCollisions()
        {
            Solid solid = CollideFirst<Solid>();
            if (solid != null)
            {
                Entity parent = MeliHelperActualParentComponent.GetActualParent(solid);
                bool destroy_bullet = true;
                bool is_show_explosion = true;

                if (solid is Enemy)
                {
                    if (is_player_bullet)
                    {
                        (solid as Enemy).TakeDamage(true);
                        is_show_explosion = false;
                    }
                    else
                        destroy_bullet = false;
                }
                else if (solid is Flag)
                    (solid as Flag).Destroy();
                else if (solid is DashSwitch)
                    InteractionController.ActivateDashSwitch(solid as DashSwitch);
                else if (solid is InteractiveLevelLoadEntity)
                    (solid as InteractiveLevelLoadEntity).IncValue();
                else if (solid is InteractiveLevelLoadEntityCenter)
                    (solid as InteractiveLevelLoadEntityCenter).IncValue();
                else if (parent is FieldCellWater)
                    destroy_bullet = false; // move through water anytime!
                else if (parent is _Minesweeper.MinesweeperCell)
                {
                    if (is_player_bullet) (parent as _Minesweeper.MinesweeperCell).Open();
                    is_show_explosion = false;
                }
                else if (solid is BombermanCapsule)
                {
                    if (is_player_bullet) 
                        (solid as BombermanCapsule).Hit();
                }
                else if (!field.isInField(Position))
                    Audio.Play(SoundController.BC_FIRING_AT_THE_WALL);
                else
                {
                    bool is_brick = parent is FieldCellBrick;
                    bool is_steel = parent is FieldCellSteel;
                    if (!is_brick && !(is_steel && is_can_break_steel))
                        Audio.Play(SoundController.BC_FIRING_AT_THE_WALL);
                    else
                    {
                        Audio.Play(SoundController.BC_FIRING_AT_THE_BRICKS);

                        //Rectangle rect = field.CheckWallCollisions(parent.Center, dir, is_can_break_steel);   // welp...

                        Rectangle rect = field.CheckWallCollisionsByDash(this.Center, dir, depth: 2);
                        if (rect == Rectangle.Empty)
                            rect = field.CheckWallCollisions(parent.Center, dir, is_can_break_steel); 
                        if (rect != Rectangle.Empty)
                            field.DestroyCells(rect);
                    }
                }

                if (destroy_bullet)
                    Die(is_show_explosion);
                return true;
            }



            // Celeste entities: spinners and seekers
            CrystalStaticSpinner spinner = level.Entities.FindAll<CrystalStaticSpinner>().FirstOrDefault(t => t.CollidePoint(this.Center));
            if (spinner != null)
            {
                spinner.Destroy();
                Die();
                return true;
            }

            Seeker seeker = level.Entities.FindAll<Seeker>().FirstOrDefault(t => t.CollidePoint(this.Center));
            if (seeker != null)
            {
                InteractionController.HitSeeker(seeker, this);
                Die();
                return true;
            }

            return false;
        }

        public override void Render()
        {
            base.Render();
            texture.DrawCentered(this.Position, color, scale: 1f, rotation: angle);
            if (is_can_break_steel)
                texture.DrawCentered(this.Position, Color.Red, scale: is_player_bullet ? 1f : 1.5f, rotation: angle);
        }

        void Die(bool is_show_explosion = true)
        {
            RemoveSelf();
            if (is_show_explosion)
                SceneAs<Level>().Add(new TemporalSpriteEntity(this.Center, "MeliHelper_BC_Explosion", "stop"));
        }

        void onPlayer(Player player)
        {
            if (Methods.PlayerIsAlive(player) && !field.GetEventUI.isEventExists(BCEnum_BonusEvent.Shield))
            {
                player.Die(Vector2.Normalize(speed));
                Die();
            }
        }

        public Entity GetParent
        {
            get
            {
                return parent;
            }
        }

        public Vector2 GetSpeed
        {
            get
            {
                return speed;
            }
            set
            {
                speed = value;
            }
        }

        public bool isFromPlayer
        {
            get
            {
                return is_player_bullet;
            }
            set
            {
                is_player_bullet = value;
            }
        }

        public bool isShadowBullet
        {
            get
            {
                return is_shadow_bullet;
            }
        }

        public Color SetColor
        {
            set
            {
                color = value;
            }
        }
    }
}
