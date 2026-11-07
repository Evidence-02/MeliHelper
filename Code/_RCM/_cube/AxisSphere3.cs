using Microsoft.Xna.Framework;

namespace Celeste.Mod.MeliHelper
{
    class AxisSphere3 : Axis
    {
        Vector3 dist_point;
        float dist;

        public AxisSphere3(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 rotate_point, Vector3 dist_point, float dist, RCMEnum_AxisType type, int cnt_turns,
            string description = "") 
            : base(p1, p2, p3, rotate_point, type, cnt_turns, description: description)
        {
            this.dist_point = dist_point;
            this.dist = dist;
            this.isRegisteredInJournal = true;
        }

        public override bool isStickerOnAxis(Sticker sticker)
        {
            return Vector3.Distance(sticker.GetCentralPoint(), dist_point) <= dist;
        }

    }
}
