using Celeste;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;
using Celeste.Mod.Entities;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    class FieldCellSteel : FieldCell
    {
        Solid solid;

        public FieldCellSteel(Vector2 position, MTexture texture = null) 
            : base(position, BCEnum_CellType.Steel, texture)
        {
        }

        public override void Awake(Scene scene)
        {
            SceneAs<Level>().Add(solid = new Solid(Position, 8, 8, true));
            solid.Add(new MeliHelperActualParentComponent(this));
            solid.OnDashCollide += onDashCollide;
        }

        public override void Removed(Scene scene)
        {
            base.Removed(scene);
            solid.RemoveSelf();
        }

        protected virtual DashCollisionResults onDashCollide(Player player, Vector2 dir)
        {
            if (ProgressController.isPlayerCanDestroySteel())
            {
                Rectangle rect = Field.Instance.CheckWallCollisionsByDash(player.Center, dir, depth: 5);
                if (rect == Rectangle.Empty)
                    rect = Field.Instance.CheckWallCollisions(this.Center, dir, true); // double check

                if (rect != Rectangle.Empty)
                    Field.Instance.DestroyCells(rect);
                return DashCollisionResults.NormalCollision;
            }

            return DashCollisionResults.Ignore;
        }




    }
}
