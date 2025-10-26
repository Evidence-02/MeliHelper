using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class BaddyVisualShadow : Entity
    {
        Vector2 center;
        Sprite spriteShadow;
        float alpha, charge_delay;
        
        public BaddyVisualShadow(Player player, Vector2 center) : base()
        {
            this.center = center;
            Depth = player.Depth + 1;
            
            Add(spriteShadow = GFX.SpriteBank.Create("MeliHelper_BadelineShadow"));
            spriteShadow.Color = Color.Transparent;
            alpha = 0;
            charge_delay = 1.4f;
        }

        public override void Update()
        {
            base.Update();

            spriteShadow.Scale.X = (Input.MoveX.Value > 0) ? 1 : -1;
            if (charge_delay > 0)
            {
                charge_delay -= Engine.DeltaTime;
                if (alpha < 0.33f)
                {
                    alpha += 2f * Engine.DeltaTime / 0.33f;
                    spriteShadow.Color = Color.DarkGray * alpha;
                }
            }
            else
            {
                alpha -= Engine.DeltaTime / 0.16f;
                if (alpha <= 0)
                    RemoveSelf();
                else
                    spriteShadow.Color = Color.DarkGray * alpha;
            }
        }

        public override void Render()
        {
            spriteShadow.RenderPosition = center;
            spriteShadow.Render();
        }
    }
}
