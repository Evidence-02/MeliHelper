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
    [CustomEntity("MeliHelper/BattleCityCellBrickCustomTexture")]
    class CellBrickCustomTexture : Entity
    {
        MTexture texture;

        public CellBrickCustomTexture(EntityData data, Vector2 offset) : base(data.Position + offset - new Vector2(8, 8)) 
        {
            texture = GFX.Game[data.Attr("texture")];
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);

            Field field = Field.Instance;
            int tx = field.GetTileCX(Position);
            int ty = field.GetTileCY(Position);

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    field.AddCellBrickTile(tx + i, ty + j, new MTexture(texture, 4 * i, 4 * j, 4, 4));

            RemoveSelf();
        }
    }
}
