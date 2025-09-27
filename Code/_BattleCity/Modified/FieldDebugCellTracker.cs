using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    class FieldDebugCellTracker : Entity
    {
        Field field;
        float alpha;

        public FieldDebugCellTracker(Field field)
        {
            this.field = field;
            this.alpha = 1f;
            Depth = DepthController.DEFAULT_UI;
        }

        public override void Render()
        {
            base.Render();


            for (int i = 0; i < field.GetW; i++)
                for (int j = 0; j < field.GetH; j++)
                {
                    BCEnum_CellType type = field.GetCellType(i, j);
                    Vector2 pos = field.GetTilePosition(i, j);
                    Rectangle rect = new Rectangle((int)pos.X, (int)pos.Y, 4, 4);
                    switch (type)
                    {
                        case BCEnum_CellType.Brick: Draw.Rect(rect, Color.Red * alpha); break;
                        case BCEnum_CellType.Steel: Draw.Rect(rect, Color.Black * alpha); break;
                        case BCEnum_CellType.Water: Draw.Rect(rect, Color.Blue * alpha); break;
                        case BCEnum_CellType.Grass: Draw.Rect(rect, Color.Green * alpha); break;
                        case BCEnum_CellType.Dirt: Draw.Rect(rect, Color.Yellow * alpha); break;
                        case BCEnum_CellType.Blocked: Draw.Rect(rect, Color.White * alpha); break;
                    }
                }
        }
    }
}
