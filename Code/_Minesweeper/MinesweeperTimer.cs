using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._Minesweeper
{
    [CustomEntity("MeliHelper/MinesweeperTimer")]
    class MinesweeperTimer : Entity
    {
        CustomTimer timer;
        int seconds, digits, max;
        bool is_stopped;

        public MinesweeperTimer(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            timer = new CustomTimer(1);
            digits = data.Int("digits", 3);
            max = (int)Math.Pow(10, digits) - 2;
        }

        public override void Update()
        {
            base.Update();
            if (!is_stopped && seconds <= max && timer.Tick())
                seconds++;
        }

        public override void Render()
        {
            base.Render();

            Vector2 pos = Position + new Vector2(13 * digits - 13, 0);
            int temp = seconds;
            for (int i = 0; i < digits; i++)
            {
                GFX.Game["Evidence02/objects_melihelper/minesweeper/digit" + (temp % 10).ToString("00")].Draw(pos);
                pos -= new Vector2(13, 0);
                temp /= 10;
            }
        }

        public void Stop()
        {
            is_stopped = true;
        }

    }
}
