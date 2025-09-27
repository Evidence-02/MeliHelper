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
    class FieldCellGrass : FieldCell
    {
        public FieldCellGrass(Vector2 position, MTexture texture = null) 
            : base(position, BCEnum_CellType.Grass, texture)
        {
            Depth = DepthController.BC_CELL_GRASS;
        }
    }
}
