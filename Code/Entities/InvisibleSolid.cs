using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    [CustomEntity("MeliHelper/InvisibleSolid")]
    class InvisibleSolid : Solid
    {
        public InvisibleSolid(EntityData data, Vector2 offset) : base(data.Position + offset, data.Width, data.Height, true) 
        {
        }
    }
}
