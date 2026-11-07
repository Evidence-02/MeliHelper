using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class StickerProjection3 : StickerProjection
    {
        int slices_w, slices_h;
        
        public StickerProjection3(CubeProjection parent, Sticker sticker) : base(parent, sticker)
        {
            list_elements = new List<StickerProjectionElement>();
            sticker.RegisterProjection(this);
        }

        public override void SetSlices(float length)
        {
            Vector3 v0 = sticker.GetVertices[0];
            Vector3 dw = (sticker.GetVertices[1] - v0);
            Vector3 dh = (sticker.GetVertices[3] - v0);
            slices_w = Math.Max(1, (int)(dw.Length() / length));
            slices_h = Math.Max(1, (int)(dh.Length() / length));
        }

        public override void Recalculate()
        {
            list_elements.Clear();

            Vector3 v0 = sticker.GetVertices[0];
            Vector3 dw = (sticker.GetVertices[1] - v0) / slices_w;
            Vector3 dh = (sticker.GetVertices[3] - v0) / slices_h;
            for (int w = 0; w < slices_w; w++)
                for (int h = 0; h < slices_h; h++)
                {
                    Vector3 vc = v0 + w * dw + h * dh;
                    list_elements.Add(new StickerProjectionPolygon(parent, sticker, new Vector3[] { vc, vc + dw, vc + dw + dh, vc + dh }, w, h, slices_w, slices_h));
                }
        }
    }
    
    class StickerProjectionPolygon : StickerProjectionElement
    {
        Vector2[] vertices;
        Vector2[] points;
        bool is_border_top, is_border_bottom, is_border_left, is_border_right;

        public StickerProjectionPolygon(CubeProjection parent, Sticker sticker, Vector3[] v3_vertices, int w, int h, int slices_w, int slices_h) 
            : base(sticker, parent.CalculateZ(Methods.GetCentralPoint(v3_vertices)))
        {
            vertices = new Vector2[v3_vertices.Length];
            for (int i = 0; i < v3_vertices.Length; i++)
                vertices[i] = parent.CalculateProjection(v3_vertices[i]);
            
            // Find all the points inside of the polygon
            int minx = int.MaxValue, miny = int.MaxValue, maxx = int.MinValue, maxy = int.MinValue;
            for (int i = 0; i < vertices.Length; i++)
            {
                if (vertices[i].X < minx) minx = (int)vertices[i].X;
                if (vertices[i].X > maxx) maxx = (int)vertices[i].X;
                if (vertices[i].Y < miny) miny = (int)vertices[i].Y;
                if (vertices[i].Y > maxy) maxy = (int)vertices[i].Y;
            }

            List<Vector2> list_points = new List<Vector2>();
            for (int x = minx; x <= maxx; x++)
                for (int y = miny; y <= maxy; y++)
                    if (Methods.isPointInsideOfThePolygon(this.vertices, new Vector2(x, y)))
                        list_points.Add(new Vector2(x, y));
            points = list_points.ToArray();



            this.is_border_top    = (h == 0);
            this.is_border_bottom = (h == slices_h - 1);
            this.is_border_left   = (w == 0);
            this.is_border_right  = (w == slices_w - 1);
        }

        public override void Render()
        {
            foreach (var point in points)
                Draw.Point(point, sticker.GetColor);
            if (is_border_top   ) Draw.Line(vertices[0], vertices[1], sticker.GetColorBorder);
            if (is_border_right ) Draw.Line(vertices[1], vertices[2], sticker.GetColorBorder);
            if (is_border_bottom) Draw.Line(vertices[2], vertices[3], sticker.GetColorBorder);
            if (is_border_left  ) Draw.Line(vertices[3], vertices[0], sticker.GetColorBorder);
        }
    }
}
