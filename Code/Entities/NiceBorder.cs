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
    [CustomEntity("MeliHelper/NiceBorder")]
    class NiceBorder : Entity
    {
        const float THICK_ALPHA2 = 0.23f;

        Rectangle rect;
        Color color_inside, color_border;
        string border_type;
        float alpha, alpha_border;
        bool show_left, show_right, show_top, show_bottom;

        public NiceBorder(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            rect = new Rectangle((int)Position.X, (int)Position.Y, data.Width, data.Height);
            border_type = data.Attr("borderType", "Solid");
            color_border = Methods.GetColorFromString(data.Attr("colorBorder"));
            color_inside = Methods.GetColorFromString(data.Attr("colorInside"));
            alpha = data.Float("opacityInside");
            alpha_border  = data.Float("opacityBorder", 1f);
            show_left = data.Bool("borderLeft");
            show_right = data.Bool("borderRight");
            show_top = data.Bool("borderTop");
            show_bottom = data.Bool("borderBottom");
        }


        public override void Render()
        {
            base.Render();
            Draw.Rect(rect, color_inside);
            switch (border_type)
            {
                case "Solid": RenderBorderSolid(); break;
                case "Thick": RenderBorderThick(); break;
            }

        }

        void RenderBorderSolid()
        {
            if (show_top)
            {
                Draw.Rect(rect.Left, rect.Top - 1, rect.Width, 2, color_border * alpha_border);
            }
            if (show_bottom)
            {
                Draw.Rect(rect.Left, rect.Bottom - 1, rect.Width, 2, color_border * alpha_border);
            }

            if (show_left)
            {
                Draw.Rect(rect.Left - 1, rect.Top, 2, rect.Height, color_border * alpha_border);
            }
            if (show_right)
            {
                Draw.Rect(rect.Right - 1, rect.Top, 2, rect.Height, color_border * alpha_border);
            }
        }


        void RenderBorderThick()
        {
            if (show_top)
            {
                Draw.Rect(rect.Left, rect.Top - 2, rect.Width, 4, color_border * alpha_border * THICK_ALPHA2);
                Draw.Rect(rect.Left, rect.Top - 1, rect.Width, 2, color_border * alpha_border);
            }
            if (show_bottom)
            {
                Draw.Rect(rect.Left, rect.Bottom - 2, rect.Width, 4, color_border * alpha_border * THICK_ALPHA2);
                Draw.Rect(rect.Left, rect.Bottom - 1, rect.Width, 2, color_border * alpha_border);
            }

            if (show_left)
            {
                Draw.Rect(rect.Left - 2, rect.Top, 4, rect.Height, color_border * alpha_border * THICK_ALPHA2);
                Draw.Rect(rect.Left - 1, rect.Top, 2, rect.Height, color_border * alpha_border);
            }
            if (show_right)
            {
                Draw.Rect(rect.Right - 2, rect.Top, 4, rect.Height, color_border * alpha_border * THICK_ALPHA2);
                Draw.Rect(rect.Right - 1, rect.Top, 2, rect.Height, color_border * alpha_border);
            }
        }
    
    }
}
