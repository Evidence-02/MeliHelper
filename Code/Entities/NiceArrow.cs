using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    [CustomEntity("MeliHelper/NiceArrow")]
    class NiceArrow : Entity
    {
        MTexture texture_inside, texture_border;
        Color color_inside, color_border;
        float alpha_inside, alpha_border;
        float hsv_inside, hsv_border, hsv_inside_del, hsv_border_del;
        bool is_show_inside, is_show_border;
        float angle;

        public NiceArrow(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            texture_inside = GFX.Game["Evidence02/objects_melihelper/nicearrow/inside"];
            texture_border = GFX.Game["Evidence02/objects_melihelper/nicearrow/border"];
            color_inside = Methods.GetColorFromString(data.Attr("colorInside", "FF0000"));
            color_border = Methods.GetColorFromString(data.Attr("colorBorder", "000000"));
            alpha_inside = data.Float("opacityInside", 0.4f);
            alpha_border = data.Float("opacityBorder", 0.4f);
            is_show_inside = data.Bool("showInside");
            is_show_border = data.Bool("showBorder");
            hsv_inside_del = data.Float("colorInsideChangeSpeed", 0);
            if (hsv_inside_del > 0) hsv_inside = Methods.GetHueFromColor(color_inside);
            hsv_border_del = data.Float("colorBorderChangeSpeed", 0);
            if (hsv_border_del > 0) hsv_border = Methods.GetHueFromColor(color_border);

            string direction = data.Attr("direction", "Right");
            switch (direction)
            {
                case "Right": angle = 0 * MathExt.PI2 / 4; break;
                case "Down":  angle = 1 * MathExt.PI2 / 4; break;
                case "Left":  angle = 2 * MathExt.PI2 / 4; break;
                case "Up":    angle = 3 * MathExt.PI2 / 4; break;
                case "Custom": angle = data.Float("angle"); break;
            }
        }

        public override void Update()
        {
            base.Update();
            if (hsv_inside_del > 0) color_inside = Methods.GetColorHSV(hsv_inside += hsv_inside_del * Engine.DeltaTime);
            if (hsv_border_del > 0) color_border = Methods.GetColorHSV(hsv_border += hsv_border_del * Engine.DeltaTime);
        }

        public override void Render()
        {
            base.Render();
            if (is_show_border) texture_border.DrawCentered(Position, color_border * alpha_border, scale: 1f, rotation: angle);
            if (is_show_inside) texture_inside.DrawCentered(Position, color_inside * alpha_inside, scale: 1f, rotation: angle);
        }
    }
}
