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
    [CustomEntity("MeliHelper/BattleCityCell")]
    class Cell : Entity
    {
        BCEnum_CellType type;
        int code;
        
        public Cell(EntityData data, Vector2 offset) : base(data.Position + offset) 
        {
            this.type = (BCEnum_CellType)Enum.Parse(typeof(BCEnum_CellType), data.Attr("cellType"));
            
            string fill = data.Attr("fill", "Full");
            switch (fill)
            {
                case "Full":   code = 15; break; // 1+2+4+8
                case "Left":   code =  5; break; // 1+4
                case "Right":  code = 10; break; // 2+8
                case "Top":    code =  3; break; // 1+2
                case "Bottom": code = 12; break; // 4+8
                    /*
                case "Custom":
                    this.code = (data.Bool("topLeft"    ) ? 1 : 0)
                              + (data.Bool("topRight"   ) ? 2 : 0)
                              + (data.Bool("bottomLeft" ) ? 4 : 0)
                              + (data.Bool("bottomRight") ? 8 : 0);
                    break;
                    */
            }
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);

            Field field = Field.Instance;
            if (field != null)
            {
                // bytes: 
                // 1 - top left
                // 2 - top right
                // 4 - bottom left
                // 8 - bottom right
                if (code % 2 >= 1) field.AddCell(type, Position + new Vector2(-8, -8));
                if (code % 4 >= 2) field.AddCell(type, Position + new Vector2( 0, -8));
                if (code % 8 >= 4) field.AddCell(type, Position + new Vector2(-8,  0));
                if (code     >= 8) field.AddCell(type, Position + new Vector2( 0,  0));
            }
            RemoveSelf();
        }
    }
}
