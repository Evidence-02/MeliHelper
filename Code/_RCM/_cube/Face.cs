using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;

namespace Celeste.Mod.MeliHelper
{
    class Face
    {
        Color color;
        List<Sticker> list_stickers;
        float koef_sticker_shift;

        public Face(Color color, float koef_sticker_shift)
        {
            this.color = color;
            this.koef_sticker_shift = koef_sticker_shift;
            list_stickers = new List<Sticker>();
        }

        public void AddSticker(Sticker sticker)
        {
            list_stickers.Add(sticker);
        }

        public bool isSolved()
        {
            string code = list_stickers[0].GetFaceCode(koef_sticker_shift);
            for (int i = 1; i < list_stickers.Count; i++)
                if (list_stickers[i].GetFaceCode(koef_sticker_shift) != code)
                    return false;
            return true;
        }

        public void RenderDebug(Vector2 pos)
        {
            Draw.Rect(pos, 6, 6, color);
            CustomTextController.DrawText(isSolved() ? "SOLVED" : "SCRAMBLED", pos + new Vector2(8, 0), Color.White);
        }

        public void RenderDebugDetail(Vector2 pos)
        {
            Draw.Rect(pos, 6, 6, color);
            foreach (var item in list_stickers)
            {
                CustomTextController.DrawText(item.GetFaceCode(koef_sticker_shift), pos + new Vector2(8, 0), Color.White);
                pos.Y += 8;
            }
        }
    }
}
