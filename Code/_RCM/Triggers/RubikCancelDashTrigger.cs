using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    [CustomEntity("MeliHelper/RubikCancelDashTrigger")]
    class RubikCancelDashTrigger : Trigger
    {
        Vector2 dir_only;

        public RubikCancelDashTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            switch (data.Attr("dirOnly", "All"))
            {
                case "Left":  dir_only = new Vector2(-1, 0); break;
                case "Right": dir_only = new Vector2(+1, 0); break;
                case "Up":    dir_only = new Vector2(0, -1); break;
                case "Down":  dir_only = new Vector2(0, +1); break;
                default: dir_only = Vector2.Zero; break;
            }
        }

        public bool isCancelDash(Vector2 dir_player)
        {
            return dir_only == Vector2.Zero || dir_only == dir_player;
        }
    }
}
