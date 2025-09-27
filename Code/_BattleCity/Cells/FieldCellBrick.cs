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
    class FieldCellBrick : FieldCell
    { 
        Solid solid;

        public FieldCellBrick(Vector2 position, MTexture texture) 
            : base(position, BCEnum_CellType.Brick, texture)
        {
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            SceneAs<Level>().Add(solid = new Solid(Position, 4, 4, true));
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
            Rectangle rect = Field.Instance.CheckWallCollisionsByDash(player.Center, dir, depth: 5);
            if (rect == Rectangle.Empty)
                rect = Field.Instance.CheckWallCollisions(this.Center, dir, false); // double check

            if (rect != Rectangle.Empty)
                Field.Instance.DestroyCells(rect);
            return DashCollisionResults.NormalCollision;
        }




    }
}
