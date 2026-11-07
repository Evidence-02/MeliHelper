using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class CustomVirtualButtonChecker
    {
        VirtualButton button;
        bool is_pressed_old;

        public CustomVirtualButtonChecker(VirtualButton button)
        {
            this.button = button;
        }

        public bool OhItsReallyFuckingPressedIsntIt()
        {
            if (button.Pressed && !is_pressed_old)
            {
                is_pressed_old = true;
                return true;
            }
            is_pressed_old = button.Pressed;
            return false;
        }

        public void Clear()
        {
            is_pressed_old = true;
        }
    }
}
