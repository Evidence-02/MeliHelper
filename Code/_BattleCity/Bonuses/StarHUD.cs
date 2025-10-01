using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    class StarHUD : Entity
    {
        MTexture texture;
        Vector2 pos, speed, dest;
        float star_angle;
        int star_id;
        bool is_fixed;

        public StarHUD(Vector2 pos, Vector2 dest, Vector2 speed, int star_id)
        {
            this.pos = pos;
            this.dest = dest;
            this.speed = speed;
            this.star_id = star_id;
            this.texture = GFX.Gui["Evidence02/bc/uiStar"];
            Tag = Tags.HUD;
            Depth = DepthController.DEFAULT_UI;
        }

        public override void Update()
        {
            base.Update();
            pos += speed * Engine.DeltaTime;
            speed *= 0.96f;
            speed += 0.28f * (dest - pos);
            
            if (!is_fixed)
            {
                star_angle += 3 * Vector2.Distance(dest, pos) * MathExt.DegreesToRadians * Engine.DeltaTime;
                is_fixed = (Vector2.Distance(dest, pos) < 48 && speed.Length() <= 60);
            }
            else
            {
                star_angle += 0.06f * (MathExt.PI2 - star_angle % MathExt.PI2);
            }


            if (Vector2.Distance(dest, pos) <= 6 && speed.Length() <= 40)
            {
                Field.Instance.GetEventUI.StarSetDelay(star_id, false);
                RemoveSelf();
            }
        }

        public override void Render()
        {
            base.Render();
            texture.DrawCentered(pos, Color.White, scale: 1f, rotation: star_angle);
        }

    }
}
