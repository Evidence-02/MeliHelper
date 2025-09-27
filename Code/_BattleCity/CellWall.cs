using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    [CustomEntity("MeliHelper/BattleCityCellWall")]
    class CellWall : Entity
    {
        BCEnum_CellType type;
        int w, h;

        public CellWall(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            type = (BCEnum_CellType)Enum.Parse(typeof(BCEnum_CellType), data.Attr("cellType"));
            w = data.Width;
            h = data.Height;
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);

            Field field = Field.Instance;
            for (int i = 0; i < w; i += 8)
                for (int j = 0; j < h; j += 8)
                    field.AddCell(type, Position + new Vector2(i, j));

            RemoveSelf();
        }

    }
}
