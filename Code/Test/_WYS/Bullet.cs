using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._WYS
{
    class Bullet : Entity
    {
        Level level;
        Color color;
        Vector2 speed;
        bool is_horiz, is_damage_player;

        public Bullet(Vector2 position, Vector2 speed, Color color, bool is_horiz, bool is_damage_player = true) : base(position)
        {
            this.speed = speed;
            this.color = color;
            this.is_horiz = is_horiz;
            this.is_damage_player = is_damage_player;
            Collider = is_horiz ? new Hitbox(6, 2, -3, -1) : new Hitbox(2, 6, -1, -3);
            if (is_damage_player) Add(new PlayerCollider(onPlayer, Collider));
        }

        protected virtual void onPlayer(Player player)
        {
            if (Methods.PlayerIsAlive(player))
                player.Die(Vector2.Normalize(this.Center - player.Center));
            CreateParticles(count: 4, offset: 3);
            RemoveSelf();
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = scene as Level;
        }

        public override void Update()
        {
            base.Update();
            if (!level.IsInBounds(this.Center))
            {
                RemoveSelf();
                return;
            }

            Position += speed * Engine.DeltaTime;
            //if (level.OnInterval(0.05f))
            //    TrailManager.Add(this, color * 0.2f, duration: 0.8f);

            // 
            Solid solid = CollideFirst<Solid>();
            if (solid != null)
            {
                //Audio.Play(SoundController.);
                CreateParticles(count: 4, offset: 3);
                CollideSolid(solid);
                RemoveSelf();
            }

            /*
            int del = (int)(speed.Length() * Engine.DeltaTime / 2);
            if (del == 0) del = 1;
            for (int i = 0; i < del; i++)
            {
                if (CheckCollisions())
                    break;
            }
            */
        }

        void CollideSolid(Solid solid)
        {
            if (solid is MovingBlock)
                (solid as MovingBlock).AddValue((solid as MovingBlock).GetValueAddByShot,
                    this.Center, this.speed, this.is_horiz ? "Horizontal" : "Vertical");
        }

        void CreateParticles(int count, int offset = 3)
        {
            for (int i = 0; i < count; i++)
            {
                Vector2 del = Calc.AngleToVector(Calc.Random.NextAngle(), Calc.Random.Next(0, offset));
                level.ParticlesBG.Emit(ParticleTypes.Dust, Position + del, color);
            }
        }

        public override void Render()
        {
            base.Render();
            Draw.Rect(Collider, color);
        }





    }
}
