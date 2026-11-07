using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class StickerProjection2 : StickerProjection
    {
        int slices_w, slices_h;
        
        public StickerProjection2(CubeProjection parent, Sticker sticker) : base(parent, sticker)
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
            if (slices_w <= 1 && slices_h <= 1)
            {
                AddPolygon(sticker.GetVertices);
                if (parent.is_show_borders)
                {
                    double z = parent.CalculateZ(sticker.GetCentralPoint()) + 0.01f;
                    for (int i = 0; i < length; i++)
                        list_elements.Add(new StickerProjectionLine(sticker, z, 
                            parent.CalculateProjection(sticker.GetVertices[i]), 
                            parent.CalculateProjection(sticker.GetVertices[(i + 1) % length])
                            ));
                }
            }
            else
            {
                Vector3 v0 = sticker.GetVertices[0];
                Vector3 dw = (sticker.GetVertices[1] - v0) / slices_w;
                Vector3 dh = (sticker.GetVertices[3] - v0) / slices_h;
                for (int i = 0; i < slices_w; i++)
                    for (int j = 0; j < slices_h; j++)
                    {
                        Vector3 vc = v0 + i * dw + j * dh;
                        AddPolygon(new Vector3[] { vc, vc + dw, vc + dw + dh, vc + dh });
                    }

                if (parent.is_show_borders)
                {
                    for (int i = 0; i < slices_w; i++)
                    {
                        Vector3 vc = v0 + i * dw;
                        double Z = parent.CalculateZ(Methods.GetCentralPoint(vc, vc + dw, vc + dw + dh, vc + dh));
                        list_elements.Add(new StickerProjectionLine(sticker, Z, parent.CalculateProjection(vc), parent.CalculateProjection(vc + dw)));
                    }

                    for (int i = 0; i < slices_w; i++)
                    {
                        Vector3 vc = v0 + i * dw + slices_h * dh;
                        double Z = parent.CalculateZ(Methods.GetCentralPoint(vc, vc + dw, vc + dw + dh, vc + dh));
                        list_elements.Add(new StickerProjectionLine(sticker, Z, parent.CalculateProjection(vc), parent.CalculateProjection(vc + dw)));
                    }

                    for (int j = 0; j < slices_h; j++)
                    {
                        Vector3 vc = v0 + j * dh;
                        double Z = parent.CalculateZ(Methods.GetCentralPoint(vc, vc + dw, vc + dw + dh, vc + dh));
                        list_elements.Add(new StickerProjectionLine(sticker, Z, parent.CalculateProjection(vc), parent.CalculateProjection(vc + dh)));
                    }

                    for (int j = 0; j < slices_h; j++)
                    {
                        Vector3 vc = v0 + j * dh + slices_w * dw;
                        double Z = parent.CalculateZ(Methods.GetCentralPoint(vc, vc + dw, vc + dw + dh, vc + dh));
                        list_elements.Add(new StickerProjectionLine(sticker, Z, parent.CalculateProjection(vc), parent.CalculateProjection(vc + dh)));
                    }
                }
            }

        }


        void AddPolygon(Vector3[] vertices)
        {
            Vector2[] projection_points = new Vector2[length];
            for (int i = 0; i < length; i++)
                projection_points[i] = parent.CalculateProjection(vertices[i]);
            double Z = parent.CalculateZ(Methods.GetCentralPoint(vertices));

            // Find all the points inside of the polygon
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
                        list_elements.Add(new StickerProjectionPoint(sticker, Z, new Vector2(x, y)));
        }
    }

    class StickerProjectionElement
    {
        public Sticker sticker;
        public double GetZ;
        public StickerProjectionElement(Sticker sticker, double Z) { this.sticker = sticker; this.GetZ = Z; }
        public virtual void Render() { }
    }

    class StickerProjectionPoint : StickerProjectionElement
    {
        Vector2 point;
        public StickerProjectionPoint(Sticker sticker, double Z, Vector2 point) : base(sticker, Z) { this.point = point; }
        public override void Render() { Draw.Point(point, sticker.GetColor); }
    }

    class StickerProjectionLine : StickerProjectionElement
    {
        Vector2 p1, p2;
        public StickerProjectionLine(Sticker sticker, double Z, Vector2 p1, Vector2 p2) : base(sticker, Z) { this.p1 = p1; this.p2 = p2; }
        public override void Render() { Draw.Line(p1, p2, sticker.GetColorBorder, thickness: 1.0f); }
    }
}
