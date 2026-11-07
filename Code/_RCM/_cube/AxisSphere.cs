using Microsoft.Xna.Framework;

namespace Celeste.Mod.MeliHelper
{
    class AxisSphere : Axis
    {
        float dist;

        public AxisSphere(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 rotate_point, float dist, RCMEnum_AxisType type, int cnt_turns,
            string description = "") 
            : base(p1, p2, p3, rotate_point, type, cnt_turns, description: description)
        {
            this.dist = dist;
            this.isRegisteredInJournal = true;
        }

        public override bool isStickerOnAxis(Sticker sticker)
        {
            return Vector3.Distance(sticker.GetCentralPoint(), rotate_point) <= dist;
        }

    }
}
