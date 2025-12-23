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
    [CustomEntity("MeliHelper/BaddyPowerSetTrigger")]
    class BaddyPowerSetTrigger : Trigger
    {
        BadelinePowerParams power_params;
        bool is_one_use, is_clear_on_leave;

        public BaddyPowerSetTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            this.power_params = BaddyController.GetHookParamsFromData(data);
            this.is_one_use = data.Bool("oneUse", true);
            this.is_clear_on_leave = data.Bool("clearOnLeave");
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            BaddyController.SetPower(player.SceneAs<Level>(), power_params);
            if (is_one_use && !is_clear_on_leave)
                RemoveSelf();
        }

        public override void OnLeave(Player player)
        {
            base.OnLeave(player);
            if (is_clear_on_leave)
            {
                BaddyController.ClearPower();
                if (is_one_use)
                    RemoveSelf();
            }
        }
    }
}
