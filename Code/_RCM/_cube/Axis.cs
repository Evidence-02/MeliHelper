using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Celeste.Mod.MeliHelper
{
    abstract class Axis
    {
        protected Vector3 p1, p2, p3, rotate_point;
        protected float A, B, C, D;

        public RCMEnum_AxisType GetAxisType { get; private set; }
        public string GetDescription { get; private set; }
        public int GetCountRotates { get; private set; }
        public bool isRegisteredInJournal { get; protected set; }


        public Axis(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 rotate_point, RCMEnum_AxisType type, int cnt_turns, 
            string description = "")
        {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            this.rotate_point = rotate_point;
            Methods.GetSurfaceEquation(p1, p2, p3, ref A, ref B, ref C, ref D);
            
            if (description == "")
            {
                switch (type)
                {
                    case RCMEnum_AxisType.Front:   description = "F"; break;
                    case RCMEnum_AxisType.Back:    description = "B"; break;
                    case RCMEnum_AxisType.Up:      description = "U"; break;
                    case RCMEnum_AxisType.Down:    description = "D"; break;
                    case RCMEnum_AxisType.Left:    description = "L"; break;
                    case RCMEnum_AxisType.Right:   description = "R"; break;
                    case RCMEnum_AxisType.UpLeft:    description = "UL"; break;
                    case RCMEnum_AxisType.UpRight:   description = "UR"; break;
                    case RCMEnum_AxisType.DownLeft:  description = "DL"; break;
                    case RCMEnum_AxisType.DownRight: description = "DR"; break;
                    case RCMEnum_AxisType.AxisX: description = "X"; break;
                    case RCMEnum_AxisType.AxisY: description = "Y"; break;
                    case RCMEnum_AxisType.AxisZ: description = "Z"; break;
                    case RCMEnum_AxisType.RotateCenter: description = "C"; break;
                    case RCMEnum_AxisType.None:         description = "-"; break;
                    default: description = "?"; break;
                }
            }
            this.GetDescription = description;
            this.GetCountRotates = cnt_turns;
            this.GetAxisType = type;
        }

        public virtual bool isStickerOnAxis(Sticker sticker)
        {
            return false;
        }

        public int GetVectorPos(Vector3 v)
        {
            return Math.Sign(A * v.X + B * v.Y + C * v.Z + D);
        }
        
        public virtual Vector3 RotatePointAroundAxis(Vector3 p, double angle)
        {
            Vector3 axisDir = Vector3.Cross(p2 - p1, p3 - p1);
            Vector3 u = Vector3.Normalize(axisDir);
            Vector3 v = p - rotate_point;
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);
            Vector3 term1 = v * cos;
            Vector3 term2 = Vector3.Cross(u, v) * sin;
            Vector3 term3 = u * (Vector3.Dot(u, v) * (1 - cos));
            return rotate_point + term1 + term2 + term3;
        }


    }
}
