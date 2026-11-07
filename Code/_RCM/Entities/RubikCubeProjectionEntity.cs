using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;

namespace Celeste.Mod.MeliHelper
{
    [CustomEntity("MeliHelper/RubikCubeProjectionEntity")]
    class RubikCubeProjectionEntity : Entity
    {
        CubeProjection projection;
        Vector2 proj_center;
        float dx, dy, size;
        bool is_show_borders;

        public RubikCubeProjectionEntity(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            proj_center = Position;
            dx = (float)Math.PI * data.Float("dx",   0) / 180;
            dy = (float)Math.PI * data.Float("dy", 180) / 180;
            size = data.Float("size", 1);
            is_show_borders = data.Bool("showBorders", false);
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            Level level = scene as Level;
            RubikCube cube = level.Entities.FindAll<RubikCube>().FirstOrDefault();
            if (cube != null)
                projection = new CubeProjection1(proj_center, cube.GetListStickers, dx, dy, size, is_show_borders: is_show_borders);
            Methods.StayBehindPlayer(level, this);
        }

        public override void Render()
        {
            base.Render();
            projection.Render();
        }

        public CubeProjection GetProjection
        {
            get
            {
                return projection;
            }
        }
    }
}
