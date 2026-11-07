using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Celeste.Mod.MeliHelper
{
    class Sticker : Component
    {
        List<StickerProjection> list_projections;
        //StickerFlickering flick;
        int length;
        
        public Vector3[] GetVertices { get; private set; }
        public Vector3[] GetVerticesSaved { get; set; }
        public Color GetColor { get; private set; }
        public Color GetColorBorder { get; private set; }
        public bool isFaceSticker { get; set; }

        public Sticker(Color color, params Vector3[] vectors) : base(true, true)
        {
            this.GetColor = color;
            this.GetColorBorder = new Color(0.92f * color.R, 0.92f * color.G, 0.92f * color.B);
            this.isFaceSticker = (color != Color.Black);
            //this.flick = new StickerFlickering(color, 36);

            // DEBUG
            //this.GetColor = new Color(color.R, color.G, color.B, (byte)64);
            GetColorBorder = Color.Black;
            // -----

            list_projections = new List<StickerProjection>();
            length = vectors.Length;
            GetVertices = new Vector3[length];
            GetVerticesSaved = new Vector3[length];
            for (int i = 0; i < length; i++)
                GetVertices[i] = vectors[i];
            Save();
            RecalculateAllProjections();
        }

        public void SetMargin(float margin)
        {
            Vector3 vc = GetCentralPoint();
            for (int i = 0; i < length; i++)
                GetVertices[i] -= margin * Vector3.Normalize(GetVertices[i] - vc);
            Save();
            RecalculateAllProjections();
        }

        public void SetColor(Color clr)
        {
            this.GetColor = clr;
            //this.flick = new StickerFlickering(clr, 36);
        }

        public void Rotate(Axis axis, double angle)
        {
            for (int i = 0; i < length; i++)
                GetVertices[i] = axis.RotatePointAroundAxis(GetVerticesSaved[i], angle);
            RecalculateAllProjections();
        }

        public void Save()
        {
            for (int i = 0; i < length; i++)
                GetVerticesSaved[i] = GetVertices[i];
        }

        public void RegisterProjection(StickerProjection proj)
        {
            list_projections.Add(proj);
        }

        public void RecalculateAllProjections()
        {
            foreach (var item in list_projections)
                item.Recalculate();
        }

        public override void Update()
        {
            base.Update();
            //flick.Update();
            //this.GetColor = flick.GetCurrentColor();
        }

        public Vector3 GetCentralPoint()
        {
            return Methods.GetCentralPoint(GetVertices);
        }

        public string GetFaceCode(float koef)
        {
            if (length < 3) return "";

            float A = 0, B = 0, C = 0, D = 0;
            Methods.GetSurfaceEquation(GetVertices[0], GetVertices[1], GetVertices[2], ref A, ref B, ref C, ref D);
            return (Math.Abs(A) < koef ? "0" : A > 0 ? "+" : "-")
                 + (Math.Abs(B) < koef ? "0" : B > 0 ? "+" : "-")
                 + (Math.Abs(C) < koef ? "0" : C > 0 ? "+" : "-")
                 ;
        }
    }
}
