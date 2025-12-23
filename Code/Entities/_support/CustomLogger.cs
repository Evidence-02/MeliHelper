using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class CustomLogger
    {
        public static void Log(string action, string message)
        {
            if (false)
            {
                System.IO.StreamWriter write2 = new System.IO.StreamWriter(@"D:\abcd.txt", true);
                write2.WriteLine($"[{DateTime.Now}] ({action}) {message}");
                write2.Close();
            }
        }
    }
}
