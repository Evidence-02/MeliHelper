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
    [CustomEntity("MeliHelper/CustomCoreMessage")]
    class CustomCoreMessage : Entity
    {
        Level level;
        Player player;
        Color color_text, color_stroke;
        string text;
        float size, koef_floating, alpha, dist_appear, dist_disappear;
        int stroke;
        bool visibleAlways, outline;

        public CustomCoreMessage(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            text = Dialog.Get(data.Attr("text"));
            size = data.Float("size", 1f);
            color_text = Methods.GetColorFromString(data.Attr("colorText", "FFFFFF"));
            color_stroke = Methods.GetColorFromString(data.Attr("colorOutline", "000000"));
            stroke = data.Int("strokeThickness", 2);
            outline = data.Bool("outline");
            koef_floating = data.Float("koefFloating", 0.2f);
            dist_appear = data.Float("distanceAppear", 66);
            dist_disappear = data.Float("distanceDisappear", 114);
            visibleAlways = data.Bool("visibleAlways");
            alpha = data.Float("opacity", 1f);
            Tag = Tags.HUD;
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            level = scene as Level;
            player = level.Tracker.GetEntity<Player>();
        }

        public override void Update()
        {
            base.Update();

            if (!visibleAlways && Methods.PlayerIsAlive(player))
            {
                float dist = Math.Abs(this.Center.X - player.Center.X);
                alpha = (dist >= dist_disappear) ? 0 : (dist < dist_appear) ? 1f 
                    : (1f - (dist - dist_appear) / (dist_disappear - dist_appear));
            }
        }

        public override void Render()
        {
            base.Render();

            // Copied code from MeliHelperButtonGUI. Made a new function in Methods?
            Vector2 pos = Methods.CoordsToHUDwithFloating(level, Position, koef_floating);
            if (outline)
                ActiveFont.DrawOutline(text: text, position: pos, justify: new Vector2(0.5f), scale: new Vector2(size), 
                    color: color_text * alpha, stroke: stroke, strokeColor: color_stroke * alpha);
            else
                ActiveFont.Draw(text: text, position: pos, justify: new Vector2(0.5f), scale: new Vector2(size),
                    color: color_text * alpha);
        }


    }
}
