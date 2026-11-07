using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class SeedComputerUICheckbox : SeedComputerUIElement
    {
        MTexture texture, texture_on, texture_off;
        string text;
        float scale;
        bool is_enabled;
        
        public SeedComputerUICheckbox(Vector2 position, string text, bool value, float scale = 1.2f) : base(position)
        {
            this.texture_on  = GFX.Gui["Evidence02/rcm/checkBoxOn"];
            this.texture_off = GFX.Gui["Evidence02/rcm/checkBoxOff"];
            this.text = text;
            this.scale = scale;
            this.is_enabled = value;
            UpdateTexture();
        }

        public override UIButtonActionEnum PressUp()
        {
            return (id == 4) ? UIButtonActionEnum.GoTo0 : UIButtonActionEnum.GoPrev;
        }

        public override UIButtonActionEnum PressDown()
        {
            return UIButtonActionEnum.GoNext;
        }

        public override UIButtonActionEnum PressDash()
        {
            is_enabled = !is_enabled;
            UpdateTexture();
            return UIButtonActionEnum.Another;
        }

        public override UIButtonActionEnum PressTalk()
        {
            is_enabled = !is_enabled;
            UpdateTexture();
            return UIButtonActionEnum.Another;
        }

        public override void RenderUI()
        {
            texture.DrawCentered(position + new Vector2(-60, 0), isUnderCursor ? COLOR_TEXT_ACTIVE : COLOR_TEXT_INACTIVE, scale: 2f);
            ActiveFont.Draw(text,
                position: position,
                justify: new Vector2(0, 0.5f),
                scale: new Vector2(scale),
                color: isUnderCursor ? COLOR_TEXT_ACTIVE : COLOR_TEXT_INACTIVE);
        }
        
        void UpdateTexture()
        {
            this.texture = is_enabled ? texture_on : texture_off;
        }

        public bool GetValue
        {
            get
            {
                return is_enabled;
            }
            set
            {
                is_enabled = true;
            }
        }
    }
}
