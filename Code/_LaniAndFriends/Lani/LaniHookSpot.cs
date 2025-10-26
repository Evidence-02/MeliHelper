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
    [CustomEntity("MeliHelper/LaniHookSpot")]
    class LaniHookSpot : Entity
    {
        int radius;

        public LaniHookSpot(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            this.Center = data.Position + offset;
            this.radius = data.Int("radiusHook", 11);
            Add(GFX.SpriteBank.Create(data.Attr("sprite", "MeliHelper_LaniHookSpot")));

            int light_radius = data.Int("radiusLight", 48);
            if (light_radius > 0)
                Add(new VertexLight(Color.White, 1f, light_radius / 6, light_radius));
        }

        public int GetRadius
        {
            get
            {
                return radius;
            }
        }

    }
}
