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
    [CustomEntity("MeliHelper/MinesweeperBombCounter")]
    class MinesweeperBombCounter : Entity
    {
        public int GetCountBombs { get; set; }
        int digits;

        public MinesweeperBombCounter(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            digits = data.Int("digits", 3);
        }

        public override void Render()
        {
            base.Render();
            int digits = this.digits;

            Vector2 pos = Position + new Vector2(13 * digits - 13, 0);
            int temp = GetCountBombs;
            if (temp < 0)
            {
                GFX.Game["Evidence02/objects_melihelper/minesweeper/digitMinus"].Draw(Position);
                temp *= -1;
                digits--;
            }

            for (int i = 0; i < digits; i++)
            {
                GFX.Game["Evidence02/objects_melihelper/minesweeper/digit" + (temp % 10).ToString("00")].Draw(pos);
                pos -= new Vector2(13, 0);
                temp /= 10;
            }
        }

    }
}



/*
 using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    [CustomEntity("MeliHelper/MinesweeperBombCounter")]
    class MinesweeperBombCounter : Entity
    {
        public int GetCountBombs { get; set; }
        int digits;

        public MinesweeperBombCounter(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            digits = data.Int("digits", 3);
        }

        public override void Render()
        {
            base.Render();

            Vector2 pos = Position + new Vector2(16 * digits - 16, 0);
            int temp = GetCountBombs;
            if (temp < 0)
            {
                GFX.Game["Evidence02/objects_melihelper/minesweeper/digitMinus"].Draw(Position);
                temp *= -1;
            }

            for (int i = 0; i < digits; i++)
            {
                GFX.Game["Evidence02/objects_melihelper/minesweeper/digit" + (temp % 10).ToString("00")].Draw(pos);
                pos -= new Vector2(16, 0);
                temp /= 10;
            }
        }

    }
}

 */
