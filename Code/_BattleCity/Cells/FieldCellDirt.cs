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
    class FieldCellDirt : FieldCell
    {
        Rectangle rect;

        public FieldCellDirt(Vector2 position, MTexture texture = null) 
            : base(position, BCEnum_CellType.Dirt, texture)
        {
            Depth = DepthController.BC_CELL_DIRT;
            rect = new Rectangle((int)Position.X, (int)Position.Y, 8, 8);
        }

        public bool isCollideEntity(Entity entity)
        {
            return entity.CollideRect(rect);
        }
    }
}
