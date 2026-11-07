using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class AxisSurface : Axis
    {
        RCMEnum_AxisDirection dir;

        public AxisSurface(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 rotate_point, RCMEnum_AxisType type, int cnt_turns, 
            RCMEnum_AxisDirection dir = RCMEnum_AxisDirection.Normal, string description = "") 
            : base(p1, p2, p3, rotate_point, type, cnt_turns, description: description)
        {
            this.dir = dir;
            this.isRegisteredInJournal = (dir != RCMEnum_AxisDirection.All);
        }

        public override bool isStickerOnAxis(Sticker sticker)
        {
            return dir == RCMEnum_AxisDirection.All 
                || dir == RCMEnum_AxisDirection.Normal && GetVectorPos(sticker.GetCentralPoint()) == -1;
        }
    }
}
