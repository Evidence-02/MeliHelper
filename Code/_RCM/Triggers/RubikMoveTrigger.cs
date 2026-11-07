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
    [CustomEntity("MeliHelper/RubikMoveTrigger")]
    class RubikMoveTrigger : Trigger
    {
        List<CubeProjection> list_projections;
        float minx, miny, dx, dy, rotate_koef;
        float old_px, old_py;

        public RubikMoveTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            list_projections = new List<CubeProjection>();
            minx = data.Float("minX", -0.6f);
            miny = data.Float("minY", -0.3f);
            dx = data.Float("maxX",  0.6f) - minx;
            dy = data.Float("maxY",  0.3f) - miny;
            rotate_koef = data.Float("koefRotate", 1f);
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);

            Level level = scene as Level;
            foreach (RubikCube cube in level.Entities.FindAll<RubikCube>())
                list_projections.Add(cube.GetProjection);
            foreach (RubikCubeProjectionEntity proj_entity in level.Entities.FindAll<RubikCubeProjectionEntity>())
                list_projections.Add(proj_entity.GetProjection);
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            foreach (CubeProjection proj in list_projections)
                proj.SetRotateKoef(rotate_koef);
        }

        public override void OnStay(Player player)
        {
            base.OnStay(player);
            //if (Math.Abs(player.Center.X - old_px) >= 0.5f || Math.Abs(player.Center.Y - old_py) >= 0.5f)
            {
                float koef_x = (player.Center.X - this.Left) / this.Width;
                float koef_y = (player.Center.Y - this.Top) / this.Height;
                foreach (CubeProjection proj in list_projections)
                    proj.SetVectorRotate(new Vector3(-(miny + koef_y * dy), +(minx + koef_x * dx), 0));
            }
            old_px = player.Center.X;
            old_py = player.Center.Y; 
        }
    }
}
