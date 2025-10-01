using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class FontControllerOutline
    {
        public static void DrawText(Vector2 pos, string text, Color color, int size = 1)
        {
            text = text.ToUpper();

            string folder = GetFolder();
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] != ' ')
                    GFX.Game[folder + GetTextureName(text[i]) + "1"].Draw(pos, Vector2.Zero, color, scale: size);
                pos.X += GetCharLength(text[i]) * size;
            }
        }


        public static string GetTexturePath(char ch)
        {
            return GetFolder() + GetTextureName(ch);
        }

        public static string GetTextureName(char ch)
        {
            if (ch >= '0' && ch <= '9') return "number" + ch;
            switch (ch)
            {
                case '.': return "symbolDOT0";
                case ',': return "symbolCOMMA0";
                case '+': return "symbolPLUS0";
                case '-': return "symbolDASH0";
                case '=': return "symbolEQUAL0";
                case '!': return "symbolEXCL0";
                case '?': return "symbolQUEST0";
                case '*': return "symbolMULT0";
                case ':': return "symbolCOLON0";
                case '(': return "symbolBRACEL0";
                case ')': return "symbolBRACER0";
                case '\'': return "symbolQUOTE0";
                case '\"': return "symbolQUOTE0";
                default: return "letter" + ch + "0";
            }
        }

        public static int GetCharLength(char ch)
        {
            switch (ch)
            {
                case 'I': return 2;
                case 'L': case '!': case ':': return 3;
                case 'M': case 'W': case ' ': return 6;
                default: return 4;
            }
        }

        public static int GetTextLength(string text)
        {
            text = text.ToUpper();

            int res = 0;
            for (int i = 0; i < text.Length; i++)
                res += GetCharLength(text[i]);
            return res;
        }

        public static string GetFolder()
        {
            return "Evidence02/objects_melihelper/text_outline/";
        }



        static string GetTextureNameWhite(char ch)
        {
            if (ch >= '0' && ch <= '9') return "number0" + ch;
            switch (ch)
            {
                case '.': return "symbolDOT";
                case ',': return "symbolCOMMA";
                case '+': return "symbolPLUS";
                case '-': return "symbolDASH";
                case '=': return "symbolEQUAL";
                case '!': return "symbolEXCL";
                case '?': return "symbolQUEST";
                case '*': return "symbolMULT";
                case ':': return "symbolCOLON";
                case '(': return "symbolBRACEL";
                case ')': return "symbolBRACER";
                case '\'': return "symbolQUOTE";
                case '\"': return "symbolQUOTE";
                default: return "letter" + ch;
            }
        }

        public static void DrawTextWhite(Vector2 pos, string text, Color color, int size = 1)
        {
            text = text.ToUpper();
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] != ' ')
                    GFX.Game["Evidence02/objects_melihelper/text_white/" + GetTextureNameWhite(text[i])].Draw(pos, Vector2.Zero, color, scale: size);
                pos.X += GetCharLength(text[i]) * size;
            }
        }
    }
}
