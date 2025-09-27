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
    [CustomEntity("MeliHelper/BattleCityCellBrickRow")]
    class CellBrickRow : Entity
    {
        string[] mass_rows;

        public CellBrickRow(EntityData data, Vector2 offset) : base(data.Position + offset) 
        {
            mass_rows = new string[] {
                data.Attr("row1"),
                data.Attr("row2"),
                data.Attr("row3"),
                data.Attr("row4"),
            };
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);

            Field field = Field.Instance;
            int tx = field.GetTileCX(Position);
            int ty = field.GetTileCY(Position);
            for (int j = 0; j < mass_rows.Length; j++)
            {
                string row = mass_rows[j];
                for (int i = 0; i < row.Length; i++)
                    if (row[i] == '1')
                        field.AddCellBrickTile(tx + i, ty + j);
            }

            RemoveSelf();
        }
    }
}
