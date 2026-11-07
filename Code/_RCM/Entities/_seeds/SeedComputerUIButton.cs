using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class SeedComputerUIButton : SeedComputerUIElement
    {
        UIButtonActionEnum action;
        string text;
        float scale;
        
        public SeedComputerUIButton(Vector2 position, UIButtonActionEnum action, string text, float scale = 1.2f) : base(position)
        {
            this.action = action;
            this.text = text;
            this.scale = scale;
        }

        public override UIButtonActionEnum PressUp()
        {
            return UIButtonActionEnum.GoPrev;
        }

        public override UIButtonActionEnum PressDown()
        {
            return (id < 6) ? UIButtonActionEnum.GoNext : UIButtonActionEnum.None;
        }

        public override UIButtonActionEnum PressDash()
        {
            return action;
        }

        public override UIButtonActionEnum PressTalk()
        {
            return action;
        }

        public override void RenderUI()
        {
            ActiveFont.Draw(text,
                position: position,
                justify: new Vector2(0.5f, 0.5f),
                scale: new Vector2(scale),
                color: isUnderCursor ? COLOR_TEXT_ACTIVE : COLOR_TEXT_INACTIVE);
        }
    }
}
