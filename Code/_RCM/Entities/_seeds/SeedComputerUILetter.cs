using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class SeedComputerUILetter : SeedComputerUIElement
    {
        MTexture texture_arrow_top, texture_arrow_btm;
        string list_letters;
        int index, pos_dest;
        float index_koef, pos_cur;
        public char GetChar { get; set; }

        public SeedComputerUILetter(Vector2 position) : base(position)
        {
            this.texture_arrow_top = GFX.Gui["Evidence02/rcm/arrowTop"];
            this.texture_arrow_btm = GFX.Gui["Evidence02/rcm/arrowBottom"];
            this.list_letters = SeedController.SEED_LETTERS;
            SetChar(list_letters[0]);
        }

        public override UIButtonActionEnum PressUp()
        {
            pos_dest++;
            UpdateChar();
            return UIButtonActionEnum.Another;
        }

        public override UIButtonActionEnum PressDown()
        {
            pos_dest--;
            UpdateChar();
            return UIButtonActionEnum.Another;
        }

        public override void Update()
        {
            if (Math.Abs(pos_dest - pos_cur) > 0.01f)
            {
                pos_cur += 0.2f * (pos_dest - pos_cur);
                if (Math.Abs(pos_dest - pos_cur) <= 0.01f)
                    pos_cur = pos_dest;
                if (pos_cur < 0)
                {
                    pos_cur += list_letters.Length;
                    pos_dest += list_letters.Length;
                }
                if (pos_cur >= list_letters.Length)
                {
                    pos_cur -= list_letters.Length;
                    pos_dest -= list_letters.Length;
                }

                index = ((int)pos_cur + list_letters.Length) % list_letters.Length;
                index_koef = pos_cur - index;
                //UpdateChar();
            }
        }


        public override void Render()
        {
            // 1. Insides
            Draw.Rect(position - new Vector2(RW, RH) / 2, RW, RH, COLOR_RECTS);

            // 2. Letters
            int i1 = index;
            int i2 = (index + list_letters.Length + 1) % list_letters.Length;
            string ch1 = list_letters[i1].ToString();
            string ch2 = list_letters[i2].ToString();

            ActiveFont.Draw(ch1,
                position: position + new Vector2(0, index_koef * 160),
                justify: new Vector2(0.5f),
                scale: new Vector2(3f),
                color: isUnderCursor ? COLOR_LETTER_ACTIVE : COLOR_LETTER_INACTIVE);

            ActiveFont.Draw(ch2,
                position: position + new Vector2(0, (index_koef - 1) * 160),
                justify: new Vector2(0.5f),
                scale: new Vector2(3f),
                color: isUnderCursor ? COLOR_LETTER_ACTIVE : COLOR_LETTER_INACTIVE);


            //// 3. Top and bottom
            //Draw.Rect(position - new Vector2(rw, rh) / 2 + new Vector2(0, -0.75f * rh), rw, 0.75f * rh, COLOR_BACK);
            //Draw.Rect(position - new Vector2(rw, rh) / 2 + new Vector2(0, +rh), rw, 0.75f * rh, COLOR_BACK);
            ////Draw.Rect(position - new Vector2(rw, rh) / 2 + new Vector2(0, -0.75f * rh), rw, 0.75f * rh, Color.White * 0.3f);
            ////Draw.Rect(position - new Vector2(rw, rh) / 2 + new Vector2(0, +rh        ), rw, 0.75f * rh, Color.White * 0.3f);

            // 4. Letter border
            Draw.HollowRect(position - new Vector2(RW, RH) / 2, RW, RH, Color.Black);

            //// 5. Arrows
            //if (isUnderCursor)
            //{
            //    texture_arrow_top.DrawCentered(position + new Vector2(0, -0.9f * rh));
            //    texture_arrow_btm.DrawCentered(position + new Vector2(0, +0.9f * rh));
            //}

            //// debug //
            //ActiveFont.Draw("index="      + index,      position: position + new Vector2(0, 200), justify: new Vector2(0.5f), scale: new Vector2(0.6f), color: Color.White);
            //ActiveFont.Draw("index_koef=" + index_koef, position: position + new Vector2(0, 260), justify: new Vector2(0.5f), scale: new Vector2(0.6f), color: Color.White);
            //ActiveFont.Draw("pos_cur="    + pos_cur   , position: position + new Vector2(0, 320), justify: new Vector2(0.5f), scale: new Vector2(0.6f), color: Color.White);
            //ActiveFont.Draw("pos_dest="   + pos_dest  , position: position + new Vector2(0, 380), justify: new Vector2(0.5f), scale: new Vector2(0.6f), color: Color.White);
            //// debug //
        }

        public override void RenderUI()
        {
            // 5. Arrows
            if (isUnderCursor)
            {
                texture_arrow_top.DrawCentered(position + new Vector2(0, -0.72f * RH));
                texture_arrow_btm.DrawCentered(position + new Vector2(0, +0.72f * RH));
            }

            //// debug //
            //ActiveFont.Draw("index="      + index,      position: position + new Vector2(0, 200), justify: new Vector2(0.5f), scale: new Vector2(0.6f), color: Color.White);
            //ActiveFont.Draw("index_koef=" + index_koef, position: position + new Vector2(0, 260), justify: new Vector2(0.5f), scale: new Vector2(0.6f), color: Color.White);
            //ActiveFont.Draw("pos_cur="    + pos_cur   , position: position + new Vector2(0, 320), justify: new Vector2(0.5f), scale: new Vector2(0.6f), color: Color.White);
            //ActiveFont.Draw("pos_dest="   + pos_dest  , position: position + new Vector2(0, 380), justify: new Vector2(0.5f), scale: new Vector2(0.6f), color: Color.White);
            //// debug //
        }

        public void SetChar(char ch)
        {
            this.index    = list_letters.IndexOf(ch);
            this.pos_cur  = index;
            this.pos_dest = index;
            UpdateChar();
        }

        void UpdateChar()
        {
            int id = (pos_dest + list_letters.Length) % list_letters.Length;
            this.GetChar = list_letters[id];
        }
    }
}
