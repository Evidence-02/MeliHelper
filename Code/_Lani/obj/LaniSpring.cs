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
    [CustomEntity("MeliHelper/LaniSpring")]
    class LaniSpring : LaniThrowableObject
    {
        Sprite sprite;
        float speed_y, speed_y_jump, koef_mult_x;
        bool is_refill_dash, is_refill_stamina, is_destroy_crumble_platform_under;
        
        public LaniSpring(EntityData data, Vector2 offset) : base(data, offset)
        {
            Add(sprite = GFX.SpriteBank.Create(data.Attr("sprite", "MeliHelper_LaniSpring")));
            Add(new PlayerCollider(OnPlayer, new Hitbox(16, 4, -8, -2)));
			koef_mult_x  = data.Float("koefMultX", 0.6f);
            speed_y      = data.Float("speedY", 320);
            speed_y_jump = data.Float("speedYWithJump", 420);
            is_destroy_crumble_platform_under = data.Bool("destroyCrumblePlatformUnder", true);
			is_refill_dash = data.Bool("refillDash", true);
        	is_refill_stamina = data.Bool("refillStamina", false);
        }
        
        void OnPlayer(Player player)
        {
            if (player.Speed.Y > 20 && !is_holding)
            {
                Audio.Play(SFX.game_gen_spring);
                sprite.Play("jump");
                player.Speed.X *= koef_mult_x;
                player.Speed.Y = (Input.Jump.Check) ? -speed_y_jump : -speed_y;
				if (is_refill_dash)    player.RefillDash();
				if (is_refill_stamina) player.RefillStamina();

                if (is_destroy_crumble_platform_under)
                {
                    CrumblePlatform block_crumble = this.SceneAs<Level>().Entities.FindAll<CrumblePlatform>()
                        .FirstOrDefault(t => t.CollidePoint(this.Center + new Vector2(0, 8))
                                          || t.CollidePoint(this.Center + new Vector2(+8, 8))
                                          || t.CollidePoint(this.Center + new Vector2(-8, 8)));
                    if (block_crumble != null)
                        InteractionController.ActivateCrumblePlatform(block_crumble, player);
                }
            }
        }

        protected override void onCollideH(CollisionData data)
        {
            if (is_holding) return;

            Center -= speed * Engine.DeltaTime;
            speed.X *= -0.75f;
        }

        protected override void onCollideV(CollisionData data)
        {
            if (is_holding) return;

            //Center -= Speed * Engine.DeltaTime;
            speed.X *= (data.Hit is LaniIceBlock) ? 0.6f : 0.2f;
            if (Math.Abs(speed.Y) < 20)
                speed.Y = 0;
            else
                speed.Y *= -0.4f;
        }
    }
}
