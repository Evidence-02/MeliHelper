using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class FontControllerNES
    {
        const int LETTER_WIDTH = 28;
        const int LETTER_HEIGHT = 28;
        const int LETTER_SPACING = 5;

        public static void ShowTextNES(string text, Vector2 pos, Color color, TextAlignment alignment_horiz, float koef_size = 1)
        {
            switch (alignment_horiz)
            {
                case TextAlignment.Center: pos.X -= koef_size * GetTextWidth(text) / 2; break;
                case TextAlignment.Right: pos.X -= koef_size * GetTextWidth(text); break;
            }

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] != ' ')
                {
                    MTexture texture = GFX.Gui["Evidence02/bc/NESABC/" + GetTextureName(text[i])];
                    texture.Draw(pos, Vector2.Zero, color, scale: koef_size);
                }
                pos.X += (LETTER_WIDTH + LETTER_SPACING) * koef_size;
            }
        }

        public static int GetTextWidth(string text)
        {
            return text.Length * LETTER_WIDTH + (text.Length - 1) * LETTER_SPACING;
        }

        public static int GetTextHeight()
        {
            return LETTER_HEIGHT;
        }

        static string GetTextureName(char ch)
        {
            if (ch >= '0' && ch <= '9') return "number0" + ch;

            switch (ch)
            {
                case '-': return "symbolDASH";
                case '!': return "symbolEXCL";
                case '?': return "symbolQUEST";
                case '.': return "symbolDOT";
                case ',': return "symbolCOMMA";
                case '#': return "symbolNUM";
                case '\'': return "symbolQUOTE";
                case '\"': return "symbolQUOTEDBL";

                case '<': return "arrowLeft";
                case '>': return "arrowRight";
                default: return "letter" + ch;
            }
        }

    }
}
