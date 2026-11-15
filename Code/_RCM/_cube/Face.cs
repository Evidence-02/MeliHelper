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

        public void RenderUI(ref float dx, ref float dy)
        {
            Draw.Rect(new Vector2(dx + 10, dy + 10), 180, 40, color);

            bool is_solved = isSolved();
            if (is_solved)
                ActiveFont.DrawOutline(text: "SOLVED", position: new Vector2(dx + 100, dy + 25), justify: new Vector2(0.5f), scale: new Vector2(0.3f), color: Color.White, stroke: 2, strokeColor: Color.Black);
            else
                ActiveFont.Draw(text: "SCRAMBLED", position: new Vector2(dx + 100, dy + 25), justify: new Vector2(0.5f), scale: new Vector2(0.3f), color: Methods.GetColorBetween(color, Color.Black, 0.3f));

            if (   MeliHelperModule.Settings.DebugTools.RubiksShowDebugUI != RCMEnum_ShowUI.None 
                && MeliHelperModule.Settings.DebugTools.RubiksShowDebugUI != RCMEnum_ShowUI.Short)
            {
                bool is_extended = (MeliHelperModule.Settings.DebugTools.RubiksShowDebugUI == RCMEnum_ShowUI.Extended);
                foreach (Sticker sticker in list_stickers)
                {
                    ActiveFont.DrawOutline(text: sticker.GetFaceCode(koef_sticker_shift, is_extended: is_extended), position: new Vector2(dx + 200, dy),
                        justify: Vector2.Zero, scale: new Vector2(0.4f), color: Color.White, stroke: 2, strokeColor: Color.Black);
                    dy += 16;
                    if (dy >= 1080)
                    {
                        dy = 0;
                        dx += 300;
                    }
                }
                dy += 24;
            }
            else
                dy += 50;


            if (dy >= 1080)
            {
                dy = 0;
                dx += 300;
            }
        }
    }
}
