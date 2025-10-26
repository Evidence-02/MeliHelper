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
        int limit_bounces, limit_jumps;
        float timer_nokill;

        public LaniSnowball(EntityData data, Vector2 offset) : base(data, offset + new Vector2(0, 8))
        {
            sprite_path = data.Attr("sprite", "MeliHelper_LaniSnowball");
            limit_bounces = data.Int("bounces", 8);
            limit_jumps = data.Int("jumps", 6);
        }

        public LaniSnowball(Vector2 center, Vector2 speed, int count_bounces = 8) : base(center, speed)
        {
            this.sprite_path = "MeliHelper_LaniSnowball";
            this.limit_bounces = count_bounces;
            this.limit_jumps = 6;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            Add(sprite = GFX.SpriteBank.Create(sprite_path));
            Add(new PlayerCollider(OnPlayerJump, new Hitbox(18, 4, -9, -8)));
            Add(new PlayerCollider(OnPlayerKill, new Hitbox(10, 6, -5, -6)));
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
                if (--limit_jumps <= 0)
                    Die();
            }
        }

        void OnPlayerKill(Player player)
        {
            //
            if (!is_holding && timer_nokill <= 0 && player.Center.Y > this.Center.Y + 4 && Methods.PlayerIsAlive(player) && Speed.Length() > 20)    
                player.Die(Vector2.Normalize(player.Center - this.Center));
        }

        protected override void onCollideH(CollisionData data)
        {
            if (is_holding) return;

            if (data.Hit is DashSwitch)
                InteractionController.ActivateDashSwitch(data.Hit as DashSwitch);

            if (--limit_bounces <= 0)
                Die();
            else
            {
                Audio.Play(SFX.char_mad_bounce_boost);
                for (int i = 0; i < 3; i++)
                    level.ParticlesFG.Emit(ParticleTypes.Chimney,
                        Center + new Vector2(4 * Math.Sign(speed.X), Calc.Random.Next(-4, 5)),
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

        void Die()
        {
            Audio.Play(SFX.game_04_snowball_impact);
            for (int i = 0; i < 6; i++)
                level.ParticlesFG.Emit(ParticleTypes.Chimney,
                    Center + new Vector2(Calc.Random.Next(-4, 5), Calc.Random.Next(-4, 5)),
                    Color.White * (0.6f + 0.05f * i));
            RemoveSelf();
        }
    }
}
