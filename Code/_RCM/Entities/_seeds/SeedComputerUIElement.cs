using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    abstract class SeedComputerUIElement
    {
        protected static Color COLOR_BACK = Color.FromNonPremultiplied(26, 51, 76, 255);
        protected static Color COLOR_RECTS = Color.FromNonPremultiplied(23, 22, 44, 255);
        protected static Color COLOR_TEXT_ACTIVE = Color.White;
        protected static Color COLOR_TEXT_INACTIVE = Color.Gray;
        protected static Color COLOR_LETTER_ACTIVE = Color.FromNonPremultiplied(25, 130, 255, 255);
        protected static Color COLOR_LETTER_INACTIVE = Color.FromNonPremultiplied(12, 65, 127, 255);
        //protected static Color COLOR_LETTER_INACTIVE = Color.FromNonPremultiplied(19, 103, 198, 255);

        protected const int RW = 2 * 72;
        protected const int RH = 2 * 88;

        protected Delegate del_up, del_down, del_left, del_right, del_dash;
        protected Vector2 position;
        protected int id;
        public bool isUnderCursor { get; set; }

        public SeedComputerUIElement(Vector2 position)
        {
            this.position = position;
        }

        public void SetID(int id)
        {
            this.id = id;
        }

        public virtual UIButtonActionEnum PressDash()
        {
            return UIButtonActionEnum.None;
        }

        public virtual UIButtonActionEnum PressTalk()
        {
            return UIButtonActionEnum.None;
        }

        public virtual UIButtonActionEnum PressUp()
        {
            return UIButtonActionEnum.None;
        }

        public virtual UIButtonActionEnum PressDown()
        {
            return UIButtonActionEnum.None;
        }

        public virtual UIButtonActionEnum PressLeft()
        {
            return UIButtonActionEnum.GoPrev;
        }

        public virtual UIButtonActionEnum PressRight()
        {
            return UIButtonActionEnum.GoNext;
        }

        public virtual void Update()
        {
        }

        public virtual void Render()
        {
        }

        public virtual void RenderUI()
        {
        }
    }
}
