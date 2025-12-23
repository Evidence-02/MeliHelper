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
    [CustomEntity("MeliHelper/StaticText")]
    class StaticText : Entity
    {
        Level level;
        Color color_text, color_stroke;
        string text;
        float size, koef_floating, alpha;
        int stroke;
        string text_type;

        public StaticText(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            string param_text = data.Attr("text");
            text = Dialog.Has(param_text) ? Dialog.Get(param_text) : param_text;    // Clear or actual text
            size = data.Float("size", 1f);
            color_text = Methods.GetColorFromString(data.Attr("colorText", "FFFFFF"));
            color_stroke = Methods.GetColorFromString(data.Attr("colorOutline", "000000"));
            stroke = data.Int("strokeThickness", 2);
            koef_floating = data.Float("koefFloating", 0.2f);
            alpha = data.Float("opacity", 1f);
            text_type = data.Attr("textType", "CoreMessage");
            Tag = Tags.HUD;
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            level = scene as Level;
        }

        public override void Render()
        {
            base.Render();

            // Copied code from MeliHelperButtonGUI. Made a new function in Methods?
            Vector2 pos = Methods.CoordsToHUDwithFloating(level, Position, koef_floating);
            switch (text_type)
            {
                case "CoreMessage":
                    ActiveFont.Draw(text: text, position: pos, justify: new Vector2(0.5f), scale: new Vector2(size),
                        color: color_text * alpha);
                    break;

                case "CoreMessageOutline":
                    ActiveFont.DrawOutline(text: text, position: pos, justify: new Vector2(0.5f), scale: new Vector2(size),
                        color: color_text * alpha, stroke: stroke, strokeColor: color_stroke * alpha);
                    break;

                case "NES": FontControllerNES.ShowTextNES(text, pos, color_text * alpha, TextAlignment.Center, size); break;

                case "Classic": 
                    FontControllerOutline.DrawTextWhite(pos, text, color_text * alpha, (int)size); 
                    break;

                case "ClassicOutline":
                    FontControllerOutline.DrawText(pos, text, color_text * alpha, (int)size);
                    break;
            }
        }


    }
}
