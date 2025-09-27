using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    class EventShovel : EventBC
    {
        CustomTimer timer;
        bool is_steel;

        public EventShovel(int ttl) 
            : base(BCEnum_BonusEvent.Shovel, ttl)
        {
            timer = new CustomTimer(0.4f);
        }

        public override void Update()
        {
            base.Update();
            if (seconds <= 2 && timer.Tick())
                SetBorder(is_steel ? BCEnum_CellType.Brick : BCEnum_CellType.Steel);
        }

        public override void Added(Entity entity)
        {
            base.Added(entity);
            SetBorder(BCEnum_CellType.Steel);
        }

        public override void TtlUpdated()
        {
            base.TtlUpdated();
            SetBorder(BCEnum_CellType.Steel);
        }

        public override void Clear()
        {
            base.Clear();
            SetBorder(BCEnum_CellType.Brick);
        }

        public void SetBorder(BCEnum_CellType cell_type)
        {
            Field field = Field.Instance;
            foreach (Flag flag in field.SceneAs<Level>().Entities.FindAll<Flag>())
            {
                Vector2 topleft = flag.Position - new Vector2(8, 8);
                for (int dx = 0; dx < 4; dx++)
                    for (int dy = 0; dy < 4; dy++)
                        if (dx == 0 || dx == 3 || dy == 0 || dy == 3)
                        {
                            Vector2 pos = topleft + new Vector2(dx * 8, dy * 8);
                            if (field.isInField(pos))
                            {
                                field.RemoveCell(pos);
                                field.AddCell(cell_type, pos);
                            }    
                        }
            }
            is_steel = (cell_type == BCEnum_CellType.Steel);
        }
    }

}
