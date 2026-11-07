using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class StickerProjection1 : StickerProjection
    {
        Vector2[] projection_points;
        List<Vector2> list_points_polygon;

        public StickerProjection1(CubeProjection parent, Sticker sticker) : base(parent, sticker)
        {
            this.parent = parent;
            this.sticker = sticker;
            this.length = sticker.GetVertices.Length;
            projection_points = new Vector2[length];
            list_points_polygon = new List<Vector2>();
            sticker.RegisterProjection(this);
        }

        public override void Recalculate()
        {
            GetZ = parent.CalculateZ(sticker.GetCentralPoint());
            for (int i = 0; i < length; i++)
                projection_points[i] = parent.CalculateProjection(sticker.GetVertices[i]);
            //projection_points[i] = this.parent.Position + ProjectionController.GetProjection(list_vectors[i]);


            // Find all the points inside of the polygon
            list_points_polygon.Clear();
            int minx = int.MaxValue, miny = int.MaxValue, maxx = int.MinValue, maxy = int.MinValue;
            for (int i = 0; i < length; i++)
            {
                if (projection_points[i].X < minx) minx = (int)projection_points[i].X;
                if (projection_points[i].X > maxx) maxx = (int)projection_points[i].X;
                if (projection_points[i].Y < miny) miny = (int)projection_points[i].Y;
                if (projection_points[i].Y > maxy) maxy = (int)projection_points[i].Y;
            }
            for (int x = minx; x <= maxx; x++)
                for (int y = miny; y <= maxy; y++)
                    if (Methods.isPointInsideOfThePolygon(projection_points, new Vector2(x, y)))
                        list_points_polygon.Add(new Vector2(x, y));
        }

        public override void Render()
        {
            //if (GetZ < 0) return;

            foreach (Vector2 point_inside in list_points_polygon)
                Draw.Point(point_inside, sticker.GetColor);

            if (parent.is_show_borders)
                for (int i = 0; i < length; i++)
                    Draw.Line(projection_points[i], projection_points[(i + 1) % length], sticker.GetColorBorder, thickness: 1.0f);

            //// ~ old ~
            ////for (int i = 0; i < length; i++)
            ////    Draw.Line(projection_points[i], projection_points[(i + 1) % length], Color.Black);
            //// somehow fill the polygon... aaaaaah
            //// Niko, we need to kill this man.
            ////GraphicsController.GetGraphics().FillPolygon(new SolidBrush(color), projection_points);
            //// ~ ~ ~ ~
        }


    }
}
