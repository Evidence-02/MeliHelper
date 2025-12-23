using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._Baddy
{
    [CustomEntity("MeliHelper/BaddyPowerClearTrigger")]
    class BaddyPowerClearTrigger : Trigger
    {
        bool is_one_use;

        public BaddyPowerClearTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            this.is_one_use = data.Bool("oneUse", true);
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            BaddyController.ClearPower();
            if (is_one_use)
                RemoveSelf();
        }
    }
}
