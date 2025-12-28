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
    [CustomEntity("MeliHelper/VirtualButtonRebindingTrigger")]
    class VirtualButtonRebindingTrigger : Trigger
    {
        ButtonBinding binding_button;
        string button_name;

        public VirtualButtonRebindingTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            button_name = data.Attr("button", "BattleCity_Shoot");
            binding_button = Methods.GetButtonBinding(button_name);
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            player.SceneAs<Level>().Add(new VirtualButtonRebindingUI(button_name, binding_button.Binding));
        }
    }
}
