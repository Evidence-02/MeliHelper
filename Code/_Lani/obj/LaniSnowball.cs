using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._Lani
{
    [CustomEntity("MeliHelper/LaniSnowball")]
    class LaniSnowball : LaniThrowableObject
    {
        Sprite sprite;
        string sprite_path;
        int count_bounce;
        float timer_nokill;

        public LaniSnowball(EntityData data, Vector2 offset) : base(data, offset)
        {
            sprite_path = data.Attr("sprite", "MeliHelper_LaniSnowball");
            count_bounce = data.Int("bounces", 8);
        }

        public LaniSnowball(Vector2 center, Vector2 speed, int count_bounces = 8) : base(center, speed)
        {
            this.sprite_path = "MeliHelper_LaniSnowball";
            this.count_bounce = count_bounces;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            Add(sprite = GFX.SpriteBank.Create(sprite_path));
            Add(new PlayerCollider(OnPlayerJump, new Hitbox(12, 4, -6, -8)));
            Add(new PlayerCollider(OnPlayerKill, new Hitbox(+8, 6, -4, -6)));
            Depth = -9999999;
        }

        protected override Collider CreateCollider()
        {
            return new Hitbox(8, 9, -4, -9);
        }

        public override void Update()
        {
            base.Update();
            if (timer_nokill > 0)
                timer_nokill -= Engine.DeltaTime;
        }

        void OnPlayerJump(Player player)
        {
            if (player.Speed.Y > 10 && !is_holding && !(GetHoldable != null && GetHoldable.Holder.Leader != null))
            {
                Audio.Play(Input.Jump.Check ? SFX.char_mad_jump_assisted : SFX.char_mad_jump);
                //sprite.Play("jump");
                player.Speed.Y = (Input.Jump.Check) ? -270 : -220;
                player.RefillDash();
                speed.Y -= 40;
            }
        }

        void OnPlayerKill(Player player)
        {
            if (!is_holding && timer_nokill <= 0 && Speed.Y > 80 && player.Center.Y > this.Center.Y + 4)
                player.Die(this.Speed);
        }

        protected override void onCollideH(CollisionData data)
        {
            if (is_holding) return;

            if (data.Hit is DashSwitch)
                InteractionController.ActivateDashSwitch(data.Hit as DashSwitch);

            if (count_bounce-- <= 0)
            {
                Audio.Play(SFX.game_04_snowball_impact);
                for (int i = 0; i < 6; i++)
                    level.ParticlesFG.Emit(ParticleTypes.Chimney, 
                        Center + new Vector2(rand.Next(-4, 5), rand.Next(-4, 5)), 
                        Color.White * (0.6f + 0.05f * i));
                RemoveSelf();
            }
            else
            {
                Audio.Play(SFX.char_mad_bounce_boost);
                for (int i = 0; i < 3; i++)
                    level.ParticlesFG.Emit(ParticleTypes.Chimney,
                        Center + new Vector2(4 * Math.Sign(speed.X), rand.Next(-4, 5)),
                        Color.White * (0.8f + 0.1f * i));

                Center -= speed * Engine.DeltaTime;
                speed.X *= -1f;
                if (speed.Y > -20)
                    speed.Y = -40;
            }
        }

        protected override void onCollideV(CollisionData data)
        {
            if (is_holding) return;

            if (data.Hit is DashSwitch)
                InteractionController.ActivateDashSwitch(data.Hit as DashSwitch);
            
            if (Math.Abs(speed.Y) < 20)
                speed.Y = 0;
            else
                speed.Y *= -0.4f;

            if (sprite.CurrentAnimationID != "rotate")
                sprite.Play("rotate");
        }

        protected override void OnPickup()
        {
            base.OnPickup();
            sprite.Play("idle");
            //Collider.Height = 24;
        }

        protected override void OnRelease(Vector2 force)
        {
            base.OnRelease(force);
            timer_nokill = 0.7f;
        }
    }
}
