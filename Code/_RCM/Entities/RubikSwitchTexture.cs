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
    [CustomEntity("MeliHelper/RubikSwitchTexture")]
    class RubikSwitchTexture : Entity
    {
        Level level;
        VirtualButton button;
        MTexture texture;
        bool saved_check, is_inverted;

        public RubikSwitchTexture(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            button = Input.Talk;
            texture = GFX.Game[data.Attr("texture")];
            is_inverted = data.Bool("inverted", false);
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = scene as Level;
        }

        public override void Update()
        {
            base.Update();
            saved_check = button.Check ^ is_inverted;
            Visible = saved_check;
        }

        public override void Render()
        {
            base.Render();
            texture.DrawCentered(this.Center, Color.White);
        }
    }
}
