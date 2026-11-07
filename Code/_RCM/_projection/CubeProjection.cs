using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    // HANK, DON'T ABBREVIATE CubeProjection!!! HAAAAANK!!!!!!
    abstract class CubeProjection
    {
        protected List<StickerProjection> list_projections;
        Vector2 center;
        Vector3 v_rotate, v_offset, v_sum;
        float size_koef, rotate_koef;
        public bool is_show_borders;

        public CubeProjection(Vector2 center, List<Sticker> list_stickers, float dx = 0, float dy = 0, float size = 1, bool is_show_borders = true)
        {
            v_rotate = Vector3.Zero;
            v_offset = new Vector3(dx, dy, 0);
            this.center = center;
            this.size_koef = size;
            this.rotate_koef = 1;
            this.is_show_borders = is_show_borders;
        }

        public void AddVectorRotate(Vector3 v)
        {
            SetVectorRotate(v_rotate + v);
        }

        public void SetVectorRotate(Vector3 v)
        {
            v_rotate = (rotate_koef >= 1) ? v : (rotate_koef * v + (1 - rotate_koef) * v_rotate);
            v_sum = v_rotate + v_offset;
            foreach (var item in list_projections)
                item.Recalculate();
        }

        public void SetRotateKoef(float val)
        {
            rotate_koef = val;
        }

        public virtual void Render()
        {
        }

        public List<StickerProjection> GetStickerProjections
        {
            get
            {
                return list_projections;
            }
        }

        public Vector2 CalculateProjection(Vector3 v)
        {
            double X = v.X;
            double Y = v.Y;
            double Z = v.Z;
            double Yt = Y * Math.Cos((v_sum.X)) - Z * Math.Sin((v_sum.X));
            double Zt = Y * Math.Sin((v_sum.X)) + Z * Math.Cos((v_sum.X));
            Y = Yt;
            Z = Zt;
            double Xt = X * Math.Cos((v_sum.Y)) - Z * Math.Sin((v_sum.Y));
            Zt = X * Math.Sin((v_sum.Y)) + Z * Math.Cos((v_sum.Y));
            X = Xt;
            Z = Zt;
            Xt = X * Math.Cos((v_sum.Z)) - Y * Math.Sin((v_sum.Z));
            Yt = X * Math.Sin((v_sum.Z)) + Y * Math.Cos((v_sum.Z));
            return center + size_koef * new Vector2((float)Xt, -(float)Yt);
        }

        public double CalculateZ(Vector3 v)
        {
            double X = v.X;
            double Y = v.Y;
            double Z = v.Z;
            double Yt = Y * Math.Cos((v_sum.X)) - Z * Math.Sin((v_sum.X));
            double Zt = Y * Math.Sin((v_sum.X)) + Z * Math.Cos((v_sum.X));
            Y = Yt;
            Z = Zt;
            double Xt = X * Math.Cos((v_sum.Y)) - Z * Math.Sin((v_sum.Y));
            return X * Math.Sin((v_sum.Y)) + Z * Math.Cos((v_sum.Y));
        }
    }
}
