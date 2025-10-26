using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._Baddy
{
    class ClassicBadelineShot : Entity
    {
        protected Level level;
        protected Vector2 Speed;
        protected bool is_damage_player;
        bool is_collide_walls, is_damage_theo, is_loaded_player_collider;
        float delay_hit_player, gravity;

        protected Sprite sprite;
        ParticleType particle_type;
        Color particle_color;
        
        public ClassicBadelineShot(Vector2 pos, Vector2 speed, ClassicBadelineShotColorEnum color, 
            bool is_can_damage_player = true,
            bool is_collide_walls = true,
            bool is_can_damage_theo = false,
            float gravity = 0,
            float burst_power = 0,
            float delay_hit_player = 0
            ) : base(pos)
        {
            this.Depth = -9999995;
            this.is_collide_walls = is_collide_walls;
            this.is_damage_player = is_can_damage_player;
            this.is_damage_theo = is_can_damage_theo;
            this.delay_hit_player = delay_hit_player;
            this.Speed = speed;
            this.gravity = gravity;
            this.particle_type = FinalBossShot.P_Trail;

            Add(sprite = GFX.SpriteBank.Create("MeliHelper_ClassicBadelineShot"));
            SetColor(color, is_can_damage_player);
            
            Collider = new Hitbox(4, 4, -2, -2);
            if (is_can_damage_player)
                LoadPlayerCollider();

            if (burst_power > 0) Add(new BurstComponent(burst_power));
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = SceneAs<Level>();
        }

        public override void Update()
        {
            base.Update();
            if (gravity != 0)
                Speed.Y += gravity * Engine.DeltaTime;
            X += Speed.X * Engine.DeltaTime;
            Y += Speed.Y * Engine.DeltaTime;
            if (!level.IsInBounds(this))
            {
                RemoveSelf();
                return;
            }


            if (Scene.OnInterval(0.04f))
                level.ParticlesFG.Emit(particle_type, Center, particle_color);
            if (is_damage_player && delay_hit_player > 0) delay_hit_player -= Engine.DeltaTime;

            
            // check collide with walls
            if (is_collide_walls)
            {
                Platform platform = CollideFirst<Solid>();
                if (platform != null)
                    CollideWall(platform);
            }

            // Theo crystal
            if (is_damage_theo)
            {
                TheoCrystal theocrystal = level.Entities.FindAll<TheoCrystal>()
                    .FirstOrDefault(t => t.CollideCheck(this));
                if (theocrystal != null)
                {
                    //Audio.Play(SFX.game_05_crystaltheo_impact_side);
                    Audio.Play(SFX.game_05_crystaltheo_impact_ground);
                    theocrystal.Die();
                    RemoveSelf();
                }
            }
        }

        public void SetColor(ClassicBadelineShotColorEnum color, bool is_can_damage_player)
        {
            this.particle_color = (color == ClassicBadelineShotColorEnum.Black) ? Color.Black : Color.Red;
            this.sprite.Play(color.ToString().ToLower() + "_start");
            this.is_damage_player = is_can_damage_player;
            if (is_can_damage_player)
                LoadPlayerCollider();
        }

        protected void LoadPlayerCollider()
        {
            if (is_loaded_player_collider)
                return;

            is_loaded_player_collider = true;
            Add(new PlayerCollider(
                new Action<Player>((player) => KillPlayer(player)),
                new Hitbox(4, 4, -2, -2)));
        }

        public void SetParticleColor(ParticleType particle_type, Color particle_color)
        {
            this.particle_type = particle_type;
            this.particle_color = particle_color;
        }

        protected virtual void CollideWall(Platform platform)
        {
            RemoveSelf();
        }
        
        void KillPlayer(Player player)
        {
            if (is_damage_player && delay_hit_player <= 0 && Methods.PlayerIsAlive(player))
                player.Die((player.Center - Center).SafeNormalize(Vector2.UnitX));
        }
        
        public void ChangeDirection(Vector2 speed_new, float length)
        {
            Speed = speed_new;
        }

        protected Vector2 GetPrevLocation()
        {
            return Center - Speed * Engine.DeltaTime;
        }
    }
}
