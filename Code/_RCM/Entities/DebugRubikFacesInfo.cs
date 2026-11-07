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
    [CustomEntity("MeliHelper/RubikDebugFacesInfo")]
    class RubikDebugFacesInfo : Entity
    {
        Level level;
        RubikCube cube;

        public RubikDebugFacesInfo(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = scene as Level;
            Depth = -9999999;
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            cube = (scene as Level).Entities.FindFirst<RubikCube>();
        }

        public override void Render()
        {
            base.Render();
            if (cube != null)
                cube.RenderDebugFaces(this.Center);
        }
    }
}
