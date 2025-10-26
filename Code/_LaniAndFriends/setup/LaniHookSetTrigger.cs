using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._Lani
{
    [CustomEntity("MeliHelper/LaniHookSetTrigger")]
    class LaniHookSetTrigger : Trigger
    {
        LaniHookParams hook_params;
        bool is_one_use, is_clear_on_leave;

        public LaniHookSetTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            this.hook_params = LaniController.GetHookParamsFromData(data);
            this.is_one_use = data.Bool("oneUse", true);
            this.is_clear_on_leave = data.Bool("clearOnLeave");
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            LaniController.SetHook(hook_params);
            if (is_one_use && !is_clear_on_leave)
                RemoveSelf();
        }

        public override void OnLeave(Player player)
        {
            base.OnLeave(player);
            if (is_clear_on_leave)
            {
                LaniController.ClearHook();
                if (is_one_use)
                    RemoveSelf();
            }
        }
    }
}
