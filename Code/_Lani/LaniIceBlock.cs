using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._Lani
{
    [CustomEntity("MeliHelper/LaniIceBlock")]
    [Tracked(true)]
    class LaniIceBlock : Solid
    {
        TileGrid tiles;

        public LaniIceBlock(EntityData data, Vector2 offset) : base(data.Position + offset, data.Width, data.Height, false)
        {
            char tiletype = data.Char("tiletype", '3');
            Add(tiles = GFX.FGAutotiler.GenerateBox(tiletype, data.Width / 8, data.Height / 8).TileGrid);
        }
    }
}
