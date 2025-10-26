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
        char tiletype;
        bool blendin;

        public LaniIceBlock(EntityData data, Vector2 offset) : base(data.Position + offset, data.Width, data.Height, true)
        {
            tiletype = data.Char("tiletype", '3');
            blendin = data.Bool("blendin", false);
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            Methods.CreateTiles(scene as Level, this, tiletype, blendin);
        }
    }
}
