using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    abstract class FieldCell : Entity
    {
        MTexture texture;
        BCEnum_CellType type;

        public FieldCell(Vector2 position, BCEnum_CellType type, MTexture texture = null) : base(position)
        {
            this.type = type;
            this.texture = (texture == null) ? TextureController.GetCellTypeTexture(type) : texture;
        }

        public override void Render()
        {
            base.Render();
            if (texture != null)
                texture.Draw(Position);
        }

        public BCEnum_CellType GetCellType
        {
            get
            {
                return type;
            }
        }
    }
}
