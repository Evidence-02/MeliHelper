using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework.Input;

namespace Celeste.Mod.MeliHelper
{
    [CustomEntity("MeliHelper/VirtualButtonRebindingBlock")]
    class VirtualButtonRebindingBlock : Solid
    {
        ButtonBinding binding_button;
        string button_name;

        public VirtualButtonRebindingBlock(EntityData data, Vector2 offset) 
            : base(data.Position + offset, data.Width, data.Height, false)
        {
            char tiletype = data.Char("tiletype", '3');
            Add(GFX.FGAutotiler.GenerateBox(tiletype, data.Width / 8, data.Height / 8).TileGrid);
            this.OnDashCollide += onDashCollide;
            button_name = data.Attr("button", "BattleCity_Shoot");
            binding_button = Methods.GetButtonBinding(button_name);
        }

        protected DashCollisionResults onDashCollide(Player player, Vector2 dir)
        {
            Level level = player.SceneAs<Level>();
            KeyboardConfigUI ui = new KeyboardConfigUI();
            Binding binding = binding_button.Binding;
            level.Add(new VirtualButtonRebindingUI(button_name, binding));

            //ui.AddRemap();
            //switch (MeliHelperModule.Settings.DebugBindingAction)
            //{
            //    case 0: ui.Clear(binding); break;
            //    case 1: ui.Remap(binding); break;
            //    case 2: ui.AddRemap(Keys.E); break;
            //    case 3: ui.AddMap("Battle City Shoot Button", binding); break;
            //    case 4: ui.AddMapForceLabel("Battle City Shoot Button", binding); break;
            //    case 5: ui.Reset(); break;
                
            //    case 10: binding_button.Keys.Add(Keys.E); break;
            //    case 11: binding_button.Keys.Remove(Keys.E); break;
            //    case 12: binding_button.Buttons.Add(Buttons.X); break;
            //    case 13: binding_button.Buttons.Remove(Buttons.X); break;
            //}
                        

            return DashCollisionResults.Bounce;
        }


    }
}
