using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class CustomLogger
    {
        public static void Log(string message)
        {
            System.IO.StreamWriter write2 = new System.IO.StreamWriter(@"D:\abcd.txt", true);
            write2.WriteLine(message);
            write2.Close();

        }
    }
}
