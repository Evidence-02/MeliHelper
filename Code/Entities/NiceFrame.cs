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
    [CustomEntity("MeliHelper/NiceFrame")]
    class NiceFrame : Entity
    {
        MTexture texture;
        Rectangle rect;
        Color color_border, color_inside;
        float alpha_border, alpha_inside;
        int w, h;
        bool show_topleft, show_topright, show_bottomleft, show_bottomright, is_solid;

        public NiceFrame(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            texture = GFX.Game[data.Attr("textureBorder")];
            w = data.Width;
            h = data.Height;
            rect = new Rectangle((int)Position.X, (int)Position.Y, data.Width, data.Height);

            color_border = Methods.GetColorFromString(data.Attr("colorBorder"));
            color_inside = Methods.GetColorFromString(data.Attr("colorInside"));
            alpha_border = data.Float("opacityBorder");
            alpha_inside = data.Float("opacityInside");

            show_topleft     = data.Bool("topLeft");
            show_topright    = data.Bool("topRight");
            show_bottomleft  = data.Bool("bottomLeft");
            show_bottomright = data.Bool("bottomRight");
            is_solid = data.Bool("solid");

            int depth = data.Int("depth");
            if (depth != 0) Depth = depth;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            if (is_solid)
                (scene as Level).Add(new Solid(Position, Width, Height, true));
        }

        public override void Render()
        {
            base.Render();
            Draw.Rect(rect, color_inside * alpha_inside);
            Draw.HollowRect(rect, color_border * alpha_border);
            if (show_topleft) 
                texture.Draw(Position, Vector2.Zero, color_border * alpha_border);
            if (show_topright) 
                texture.Draw(Position + new Vector2(w, 0), origin: Vector2.Zero, color: color_border * alpha_border, scale: new Vector2(-1, 1));
            if (show_bottomleft)
                texture.Draw(Position + new Vector2(0, h), origin: Vector2.Zero, color: color_border * alpha_border, scale: new Vector2(1, -1));
            if (show_bottomright)
                texture.Draw(Position + new Vector2(w, h), origin: Vector2.Zero, color: color_border * alpha_border, scale: new Vector2(-1, -1));
        }

    }
}
