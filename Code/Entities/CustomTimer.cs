using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class CustomTimer
    {
        float timer, period;

        public CustomTimer(float period, float start_value = 0)
        {
            this.period = period;
            this.timer = start_value;
        }

        public void Reset(float period)
        {
            this.period = period;
            this.timer = 0;
        }

        public bool Tick()
        {
            timer += Engine.DeltaTime;
            if (timer >= period)
            {
                timer -= period;
                return true;
            }
            return false;
        }
    }
}
