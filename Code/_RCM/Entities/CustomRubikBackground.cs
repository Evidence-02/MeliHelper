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
    [CustomEntity("MeliHelper/CustomRubikBackground")]
    class CustomRubikBackground : Entity
    {
        Color[] mass_colors;
        CustomRubikBackgroundPixel[,] mass_pixels;
        int w, h;
        float alpha;
        string appear;

        public CustomRubikBackground(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            w = data.Width / 8;
            h = data.Height / 8;
            this.Depth = data.Int("depth");
            this.alpha = data.Float("alpha", 0.2f);
            this.appear = data.Attr("appear", "All the time");

            mass_colors = new Color[6];
            mass_colors[0] = Calc.HexToColor(data.Attr("colorUp",    "FFFFFF"));   // White
            mass_colors[1] = Calc.HexToColor(data.Attr("colorDown",  "FFFF00"));   // Yellow
            mass_colors[2] = Calc.HexToColor(data.Attr("colorLeft",  "FF8800"));   // Orange
            mass_colors[3] = Calc.HexToColor(data.Attr("colorRight", "FF0000"));   // Red
            mass_colors[4] = Calc.HexToColor(data.Attr("colorFront", "00FF00"));   // Green
            mass_colors[5] = Calc.HexToColor(data.Attr("colorBack",  "0000FF"));   // Blu
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            Level level = scene as Level;
            UpdateGolden(level);
            mass_pixels = new CustomRubikBackgroundPixel[w, h];
            for (int x = 0; x < w; x++)
                for (int y = 0; y < h; y++)
                {
                    Vector2 cell_center = this.Position + new Vector2(8 * x + 4, 8 * y + 4);
                    if (!level.CollideCheck<Solid>(cell_center))
                    {
                        mass_pixels[x, y] = new CustomRubikBackgroundPixel(
                            pos: this.Position + new Vector2(8 * x + 1, 8 * y + 1),
                            color: Calc.Random.Choose(mass_colors) * (alpha * (0.8f + Calc.Random.NextFloat(0.4f)))
                            );
                    }
                }
        }

        public override void Render()
        {
            base.Render();
            foreach (CustomRubikBackgroundPixel pixel in mass_pixels)
                if (pixel != null)
                    pixel.Render();
        }

        public void UpdateGolden(Level level)
        {
            Visible = !(appear == "Golden only" && !level.Session.GrabbedGolden || appear == "Not golden only" && level.Session.GrabbedGolden);
        }


        class CustomRubikBackgroundPixel
        {
            Vector2 pos;
            Color color;
            bool is_using;

            public CustomRubikBackgroundPixel(Vector2 pos, Color color)
            {
                this.pos = pos;
                this.color = color;
                this.is_using = false;
            }

            public void Render()
            {
                Draw.Rect(pos, 6, 6, color);
            }
        }
    }
}
