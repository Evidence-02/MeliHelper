using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    public static class Extensions
    {
        public static bool Between(this int value, int v1, int v2)
        {
            return (v1 <= value && value <= v2
                 || v2 <= value && value <= v1);
        }


        public static string Before(this string str, string context)
        {
            int pos = str.IndexOf(context);
            if (pos == -1 || str == "") return "";
            return str.Substring(0, pos);
        }

        public static string BeforeOrFull(this string str, string context)
        {
            int pos = str.IndexOf(context);
            if (pos == -1 || str == "") return str;
            return str.Substring(0, pos);
        }

        public static string After(this string str, string context)
        {
            int pos = str.IndexOf(context);
            if (pos == -1 || str == "") return "";
            return str.Substring(pos + context.Length);
        }

        public static string AfterOrFull(this string str, string context)
        {
            int pos = str.IndexOf(context);
            if (pos == -1 || str == "") return str;
            return str.Substring(pos + context.Length);
        }

        public static string Between(this string str, string context1, string context2)
        {
            int index1 = str.IndexOf(context1) + context1.Length;
            int index2 = str.IndexOf(context2, index1);
            if (index1 == -1 || index2 == -1) return "";
            return str.Substring(index1, index2 - index1);
        }

        public static string LikeName(this string str)
        {
            if (str.Length == 0) return "";
            return Char.ToUpper(str[0]) + str.Substring(1).ToLower();
        }
    }
}
