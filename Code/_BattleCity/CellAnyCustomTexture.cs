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
    [CustomEntity("MeliHelper/BattleCityCellAnyCustomTexture")]
    class CellAnyCustomTexture : Entity
    {
        MTexture texture;
        BCEnum_CellType type;

        public CellAnyCustomTexture(EntityData data, Vector2 offset) : base(data.Position + offset - new Vector2(8,8)) 
        {
            texture = GFX.Game[data.Attr("texture")];
            type = (BCEnum_CellType)Enum.Parse(typeof(BCEnum_CellType), data.Attr("cellType"));
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);

            Field field = Field.Instance;
            int tx = field.GetTileCX(Position);
            int ty = field.GetTileCY(Position);

            for (int i = 0; i < 2; i++)
                for (int j = 0; j < 2; j++)
                    field.AddCellAnyTile(type, tx + 2 * i, ty + 2 * j, new MTexture(texture, 8 * i, 8 * j, 8, 8));

            RemoveSelf();
        }
    }
}
