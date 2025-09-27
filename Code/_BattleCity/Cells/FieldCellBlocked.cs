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
    class FieldCellBlocked : FieldCell
    { 
        //Solid solid;

        public FieldCellBlocked(Vector2 position) 
            : base(position, BCEnum_CellType.Blocked, null)
        {
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            //SceneAs<Level>().Add(solid = new Solid(Position, 4, 4, true));
        }

        public override void Removed(Scene scene)
        {
            base.Removed(scene);
            //solid.RemoveSelf();
        }




    }
}
