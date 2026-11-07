using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class CustomTextController
    {
        static Dictionary<char, CharInfo> dict_chars;
        
        public static void Initialize()
        {
            dict_chars = new Dictionary<char, CharInfo>();

            #region symbols

            dict_chars[' '] = new CharInfo(new string[] {
                ""
            }, length: 2);

            dict_chars['.'] = new CharInfo(new string[] {
                " ",
                " ",
                " ",
                " ",
                "*"
            }, length: 2);

            dict_chars[','] = new CharInfo(new string[] {
                " ",
                " ",
                " ",
                " ",
                " *",
                "*"
            }, length: 2);

            dict_chars['!'] = new CharInfo(new string[] {
                "*",
                "*",
                "*",
                " ",
                "*"
            }, length: 2);

            dict_chars['?'] = new CharInfo(new string[] {
                " ** ",
                "*  *",
                "  * ",
                "    ",
                "  * "
            }, length: 4);

            dict_chars['('] = new CharInfo(new string[] {
                " *",
                "* ",
                "* ",
                "* ",
                " *"
            }, length: 2);

            dict_chars[')'] = new CharInfo(new string[] {
                "* ",
                " *",
                " *",
                " *",
                "* "
            }, length: 2);

            dict_chars[':'] = new CharInfo(new string[] {
                " ",
                "*",
                " ",
                "*",
                " "
            }, length: 2);

            dict_chars[';'] = new CharInfo(new string[] {
                "  ",
                " *",
                "  ",
                " *",
                "* "
            }, length: 2);

            dict_chars['-'] = new CharInfo(new string[] {
                "   ",
                "   ",
                "***",
                "   ",
                "   "
            });

            dict_chars['+'] = new CharInfo(new string[] {
                "   ",
                " * ",
                "***",
                " * ",
                "   "
            });

            dict_chars['/'] = new CharInfo(new string[] {
                "   ",
                "  *",
                " * ",
                "*  ",
                "   "
            });

            dict_chars['\\'] = new CharInfo(new string[] {
                "   ",
                "*  ",
                " * ",
                "  *",
                "   "
            });

            dict_chars['\''] = new CharInfo(new string[] {
                "*",
                "*",
                " ",
                " ",
                " "
            }, length: 1);

            dict_chars['\"'] = new CharInfo(new string[] {
                "* *",
                "* *",
                "   ",
                "   ",
                "   "
            }, length: 3);

            //dict_chars['%'] = new CharInfo(new string[] {
            //    "**  *",
            //    "** * ",
            //    "  *  ",
            //    " * **",
            //    "*  **"
            //}, length: 5);

            dict_chars['%'] = new CharInfo(new string[] {
                "*   *",
                "   * ",
                "  *  ",
                " *   ",
                "*   *"
            }, length: 5);

            dict_chars['['] = new CharInfo(new string[] {
                "**",
                "* ",
                "* ",
                "* ",
                "**"
            }, length: 2);

            dict_chars[']'] = new CharInfo(new string[] {
                "**",
                " *",
                " *",
                " *",
                "**"
            }, length: 2);

            #endregion

            #region alphabet (rus)

            dict_chars['А'] = new CharInfo(new string[] {
                "***",
                "* *",
                "***",
                "* *",
                "* *"
            });

            dict_chars['Б'] = new CharInfo(new string[] {
                "***",
                "*  ",
                "***",
                "* *",
                "***"
            });

            dict_chars['В'] = new CharInfo(new string[] {
                "** ",
                "* *",
                "** ",
                "* *",
                "** "
            });

            dict_chars['Г'] = new CharInfo(new string[] {
                "***",
                "*  ",
                "*  ",
                "*  ",
                "*  "
            });

            dict_chars['Д'] = new CharInfo(new string[] {
                " *** ",
                " * * ",
                " * * ",
                "*****",
                "*   *"
            }, length: 5);

            dict_chars['Е'] = new CharInfo(new string[] {
                "***",
                "*  ",
                "***",
                "*  ",
                "***"
            });

            dict_chars['Ё'] = new CharInfo(new string[] {
                "* *",
                "   ",
                "***",
                "*  ",
                "***",
                "*  ",
                "***"
            }, dy: -2);

            dict_chars['Ж'] = new CharInfo(new string[] {
                "* * *",
                "* * *",
                " *** ",
                "* * *",
                "* * *"
            }, length: 5);

            dict_chars['З'] = new CharInfo(new string[] {
                "***",
                "  *",
                " **",
                "  *",
                "***"
            });

            dict_chars['И'] = new CharInfo(new string[] {
                "*  *",
                "* **",
                "** *",
                "*  *",
                "*  *"
            }, length: 4);

            dict_chars['Й'] = new CharInfo(new string[] {
                " ** ",
                "    ",
                "*  *",
                "* **",
                "** *",
                "*  *",
                "*  *"
            }, length: 4, dy: -2);

            dict_chars['К'] = new CharInfo(new string[] {
                "* *",
                "* *",
                "** ",
                "* *",
                "* *"
            });

            dict_chars['Л'] = new CharInfo(new string[] {
                " ***",
                " * *",
                " * *",
                " * *",
                "*  *"
            }, length: 4);

            dict_chars['М'] = new CharInfo(new string[] {
                "*   *",
                "** **",
                "* * *",
                "*   *",
                "*   *"
            }, length: 5);

            dict_chars['Н'] = new CharInfo(new string[] {
                "* *",
                "* *",
                "***",
                "* *",
                "* *"
            });

            dict_chars['О'] = new CharInfo(new string[] {
                "***",
                "* *",
                "* *",
                "* *",
                "***"
            });

            dict_chars['П'] = new CharInfo(new string[] {
                "***",
                "* *",
                "* *",
                "* *",
                "* *"
            });

            dict_chars['Р'] = new CharInfo(new string[] {
                "***",
                "* *",
                "***",
                "*  ",
                "*  "
            });

            dict_chars['С'] = new CharInfo(new string[] {
                "***",
                "*  ",
                "*  ",
                "*  ",
                "***"
            });

            dict_chars['Т'] = new CharInfo(new string[] {
                "***",
                " * ",
                " * ",
                " * ",
                " * "
            });

            dict_chars['У'] = new CharInfo(new string[] {
                "* *",
                "* *",
                "***",
                "  *",
                "***"
            });

            dict_chars['Ф'] = new CharInfo(new string[] {
                " *** ",
                "* * *",
                "* * *",
                " ***",
                "  *"
            }, length: 5);

            dict_chars['Х'] = new CharInfo(new string[] {
                "* *",
                "* *",
                " * ",
                "* *",
                "* *"
            });

            dict_chars['Ц'] = new CharInfo(new string[] {
                "* * ",
                "* * ",
                "* * ",
                "****",
                "   *"
            }, length: 4);

            dict_chars['Ч'] = new CharInfo(new string[] {
                "* *",
                "* *",
                "***",
                "  *",
                "  *"
            });

            dict_chars['Ш'] = new CharInfo(new string[] {
                "* * *",
                "* * *",
                "* * *",
                "* * *",
                "*****"
            }, length: 5);

            dict_chars['Щ'] = new CharInfo(new string[] {
                "* * *",
                "* * *",
                "* * *",
                "* * *",
                "******",
                "     *"
            }, length: 6);

            dict_chars['Ъ'] = new CharInfo(new string[] {
                "**  ",
                " *  ",
                " ***",
                " * *",
                " ***"
            }, length: 4);

            dict_chars['Ы'] = new CharInfo(new string[] {
                "*   *",
                "*   *",
                "*** *",
                "* * *",
                "*** *"
            }, length: 5);

            dict_chars['Ь'] = new CharInfo(new string[] {
                "*  ",
                "*  ",
                "***",
                "* *",
                "***"
            });

            dict_chars['Э'] = new CharInfo(new string[] {
                "***",
                "  *",
                "***",
                "  *",
                "***"
            });

            dict_chars['Ю'] = new CharInfo(new string[] {
                "* ***",
                "* * *",
                "*** *",
                "* * *",
                "* ***"
            }, length: 5);

            dict_chars['Я'] = new CharInfo(new string[] {
                "***",
                "* *",
                "***",
                " **",
                "* *"
            });

            #endregion

            #region alphabet (eng)

            dict_chars['A'] = new CharInfo(new string[] {
                "***",
                "* *",
                "***",
                "* *",
                "* *"
            });

            dict_chars['B'] = new CharInfo(new string[] {
                "** ",
                "* *",
                "** ",
                "* *",
                "** "
            });

            dict_chars['C'] = new CharInfo(new string[] {
                "***",
                "*  ",
                "*  ",
                "*  ",
                "***"
            });

            dict_chars['D'] = new CharInfo(new string[] {
                "** ",
                "* *",
                "* *",
                "* *",
                "** "
            });

            dict_chars['E'] = new CharInfo(new string[] {
                "***",
                "*  ",
                "***",
                "*  ",
                "***"
            });

            dict_chars['F'] = new CharInfo(new string[] {
                "***",
                "*  ",
                "***",
                "*  ",
                "*  "
            });

            dict_chars['G'] = new CharInfo(new string[] {
                "***",
                "*  ",
                "* *",
                "* *",
                "***"
            });

            dict_chars['H'] = new CharInfo(new string[] {
                "* *",
                "* *",
                "***",
                "* *",
                "* *"
            });

            dict_chars['I'] = new CharInfo(new string[] {
                "***",
                " * ",
                " * ",
                " * ",
                "***"
            });

            dict_chars['J'] = new CharInfo(new string[] {
                "***",
                "  *",
                "  *",
                "  *",
                "** "
            });

            dict_chars['K'] = new CharInfo(new string[] {
                "* *",
                "* *",
                "** ",
                "* *",
                "* *"
            });

            dict_chars['L'] = new CharInfo(new string[] {
                "*  ",
                "*  ",
                "*  ",
                "*  ",
                "***"
            });

            dict_chars['M'] = new CharInfo(new string[] {
                "*   *",
                "** **",
                "* * *",
                "*   *",
                "*   *"
            }, length: 5);

            dict_chars['N'] = new CharInfo(new string[] {
                "*  *",
                "** *",
                "* **",
                "*  *",
                "*  *"
            }, length: 4);

            dict_chars['O'] = new CharInfo(new string[] {
                "***",
                "* *",
                "* *",
                "* *",
                "***"
            });

            dict_chars['P'] = new CharInfo(new string[] {
                "***",
                "* *",
                "***",
                "*  ",
                "*  "
            });

            dict_chars['Q'] = new CharInfo(new string[] {
                "*** ",
                "* * ",
                "* * ",
                "* * ",
                "** *"
            }, length: 4);

            dict_chars['R'] = new CharInfo(new string[] {
                "***",
                "* *",
                "** ",
                "* *",
                "* *"
            });

            dict_chars['S'] = new CharInfo(new string[] {
                "***",
                "*  ",
                "***",
                "  *",
                "***"
            });

            dict_chars['T'] = new CharInfo(new string[] {
                "***",
                " * ",
                " * ",
                " * ",
                " * "
            });

            dict_chars['U'] = new CharInfo(new string[] {
                "* *",
                "* *",
                "* *",
                "* *",
                "***"
            });

            dict_chars['V'] = new CharInfo(new string[] {
                "* *",
                "* *",
                "* *",
                "* *",
                " * "
            });

            dict_chars['W'] = new CharInfo(new string[] {
                "*   *",
                "*   *",
                "* * *",
                "** **",
                "*   *"
            }, length: 5);

            dict_chars['X'] = new CharInfo(new string[] {
                "* *",
                "* *",
                " * ",
                "* *",
                "* *"
            });

            dict_chars['Y'] = new CharInfo(new string[] {
                "* *",
                "* *",
                " * ",
                " * ",
                " * "
            });

            dict_chars['Z'] = new CharInfo(new string[] {
                "***",
                "  *",
                " * ",
                "*  ",
                "***"
            });

            dict_chars['x'] = new CharInfo(new string[] {
                "   ",
                "* *",
                " * ",
                "* *",
                "* *"
            });

            dict_chars['y'] = new CharInfo(new string[] {
                "   ",
                "* *",
                "* *",
                " * ",
                " * "
            });

            dict_chars['z'] = new CharInfo(new string[] {
                "  ",
                "**",
                " *",
                "* ",
                "**"
            }, length: 2);

            #endregion

            #region numbers

            dict_chars['1'] = new CharInfo(new string[] {
                "  *",
                " **",
                "  *",
                "  *",
                "  *"
            });

            dict_chars['2'] = new CharInfo(new string[] {
                "***",
                "  *",
                "***",
                "*  ",
                "***"
            });

            dict_chars['3'] = new CharInfo(new string[] {
                "***",
                "  *",
                "***",
                "  *",
                "***"
            });

            dict_chars['4'] = new CharInfo(new string[] {
                "* *",
                "* *",
                "***",
                "  *",
                "  *"
            });

            dict_chars['5'] = new CharInfo(new string[] {
                "***",
                "*  ",
                "***",
                "  *",
                "***"
            });

            dict_chars['6'] = new CharInfo(new string[] {
                "***",
                "*  ",
                "***",
                "* *",
                "***"
            });

            dict_chars['7'] = new CharInfo(new string[] {
                "***",
                "  *",
                "  *",
                "  *",
                "  *"
            });

            dict_chars['8'] = new CharInfo(new string[] {
                "***",
                "* *",
                "***",
                "* *",
                "***"
            });

            dict_chars['9'] = new CharInfo(new string[] {
                "***",
                "* *",
                "***",
                "  *",
                "***"
            });

            dict_chars['0'] = new CharInfo(new string[] {
                "***",
                "* *",
                "* *",
                "* *",
                "***"
            });


            #endregion

        }

        public static int GetTextLength(string text, int size = 1)
        {
            int res = 0;
            foreach (char ch in text)
                res += dict_chars[(dict_chars.ContainsKey(ch) ? ch : '?')].GetLength + 1;
            return (res - 1) * size;

        }

        public static void DrawText(string text, Vector2 pos, Color color, int size = 1)
        {
            int dx = 0, dy = 0;
            foreach (char ch in text)
                dict_chars[(dict_chars.ContainsKey(ch) ? ch : '?')].DrawChar(pos + new Vector2(dx, dy), color, size, ref dx);
        }

        //public static void DrawJournal(Vector2 pos, Color color_reg, Color color_unreg, int max_dx, Journal journal, int size = 1)
        //{
        //    int dx = 0, dy = 0;
        //    foreach (JournalMove move in journal.GetListMoves)
        //    {
        //        string descr = move.GetMoveDescription();
        //        int len = GetTextLength(descr, size);
        //        if (dx + len >= max_dx) 
        //        {
        //            // move to the next row
        //            dx = 0;
        //            dy += 6 * size;
        //        }

        //        Color clr = (move.GetAxis.isRegisteredInJournal ? color_reg : color_unreg);
        //        foreach (char ch in descr)
        //            dict_chars[(dict_chars.ContainsKey(ch) ? ch : '?')].DrawChar(pos + new Vector2(dx, dy), clr, size, ref dx);

        //        dx += size;
        //    }
        //}



        class CharInfo
        {
            List<Vector2> list_pixels;
            int length;

            public CharInfo(string[] map, int length = 3, int dy = 0)
            {
                list_pixels = new List<Vector2>();
                for (int y = 0; y < map.Length; y++)
                    for (int x = 0; x < map[y].Length; x++)
                        if (map[y][x] != ' ')
                            list_pixels.Add(new Vector2(x, y + dy));
                        
                this.length = length;
            }

            public void DrawChar(Vector2 pos, Color color, int size, ref int dx)
            {
                foreach (var del in list_pixels)
                    Draw.Rect(pos + size * del, size, size, color);
                
                dx += (length + 1) * size;
                //if (dx >= max_dx)
                //{
                //    dx = 0;
                //    dy += 6 * size;
                //}
            }

            public int GetLength
            {
                get
                {
                    return length;
                }
            }
        }
    }
}
