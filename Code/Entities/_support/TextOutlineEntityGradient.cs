using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class TextOutlineEntityGradient : TextOutlineEntity
    {
        float color_hsv;

        public TextOutlineEntityGradient(Vector2 position, string word) : base(position, word, Methods.GetColorHSV(0))
        {

        }

        public override void Update()
        {
            base.Update();

            color_hsv += 360 * Engine.DeltaTime;
            color = Methods.GetColorHSV(color_hsv);
        }
    }
}
