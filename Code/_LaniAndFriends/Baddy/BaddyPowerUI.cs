using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._Baddy
{
    class BaddyPowerUI : Entity
    {
        MTexture texture;
        int texture_left, texture_top, left, top;

        Color color_border, color_temp;
        string state_color_temp;
        float power_visual, color_temp_alpha, color_temp_deladd, color_temp_delsub;
        bool is_power_visible;
        static int HEIGHT = 20, POWER_BORDER = 3, CELL_WIDTH = 50;

        public BaddyPowerUI()
        {
            Tag = Tags.TransitionUpdate | Tags.PauseUpdate | TagsExt.SubHUD;
            color_border = Color.Black;
            power_visual = MeliHelperModule.Instance.Session.BadelinePower_Params.CurrentPower;
            is_power_visible = true;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);

            BadelinePowerParams _params = MeliHelperModule.Instance.Session.BadelinePower_Params;
            if (_params.uiTexture != "")
                texture = GFX.Gui[_params.uiTexture];
            switch (_params.uiLocation)
            {
                case "BottomLeft":     left = 170;  top = 980; break;
                case "BottomLeftMid":  left = 520;  top = 980; break;
                case "BottomRightMid": left = 1530; top = 980; break;
                case "BottomRight":    left = 1780; top = 980; break;
                case "TopLeft":        left = 170;  top = 95;  break;
                case "TopLeftMid":     left = 520;  top = 95;  break;
                case "TopRightMid":    left = 1530; top = 95;  break;
                case "TopRight":       left = 1780; top = 95;  break;
            }
            texture_left = left - 160;
            texture_top = top - 70;
        }

        public override void Update()
        {
            base.Update();

            power_visual += 0.16f * (MeliHelperModule.Instance.Session.BadelinePower_Params.CurrentPower - power_visual);
            if (power_visual < 0.4) is_power_visible = !is_power_visible;
            else if (!is_power_visible) is_power_visible = true;

            // Power border
            switch (state_color_temp)
            {
                case "00:go_up":
                    color_temp_alpha += Engine.DeltaTime / color_temp_deladd;
                    if (color_temp_alpha >= 1)
                    {
                        color_temp_alpha = 1f;
                        state_color_temp = "01:go_down";
                    }
                    color_border = Methods.GetColorBetween(Color.Black, color_temp, color_temp_alpha);
                    break;

                case "01:go_down":
                    color_temp_alpha -= Engine.DeltaTime / color_temp_delsub;
                    if (color_temp_alpha <= 0)
                    {
                        color_border = Color.Black;
                        color_temp_alpha = 0;
                        state_color_temp = "";
                    }
                    else
                        color_border = Methods.GetColorBetween(Color.Black, color_temp, color_temp_alpha);
                    break;
            }
        }

        public override void Render()
        {
            base.Render();
            if (texture != null)
                texture.Draw(new Vector2(texture_left, texture_top));

            // Screen Width:  1920
            // Screen Height: 1080
            ActiveFont.Draw(BaddyController.GetParams().isCurrentWeaponShoot ? "Standart shot" : "Laser",
                new Vector2(texture_left + 50, texture_top + 120), Vector2.Zero, new Vector2(0.6f, 0.6f),
                Color.White);

            // Override max (farewell double-dash refills)
            float val = MeliHelperModule.Instance.Session.BadelinePower_Params.CurrentPower;
            float max = MeliHelperModule.Instance.Session.BadelinePower_Params.FullPower;
            if (val > max)
                Draw.Rect(
                    left - POWER_BORDER,
                    top - POWER_BORDER,
                    CELL_WIDTH * val + 2 * POWER_BORDER,
                    HEIGHT + 2 * POWER_BORDER,
                    Color.DarkViolet);

            // Max value (black)
            Draw.Rect(
                left - POWER_BORDER,
                top - POWER_BORDER,
                CELL_WIDTH * max + 2 * POWER_BORDER,
                HEIGHT + 2 * POWER_BORDER,
                color_border);


            // I have a power!
            if (is_power_visible)
            {
                Color clr = (power_visual >= 1.4f ? Color.DarkRed :
                             power_visual < 0.4f ? Color.Green
                                                  : Methods.GetColorBetween(Color.Green, Color.DarkRed, power_visual - 0.4f));
                Draw.Rect(left, top, CELL_WIDTH * power_visual, HEIGHT, clr);
                for (int i = 1; i < max; i++)
                    Draw.Rect(left + CELL_WIDTH * i, top, 2, HEIGHT, Color.Black);
            }
        }

        public void SetColorTemp(Color color_temp, float alpha_add = 0.08f, float alpha_sub = 0.4f)
        {
            this.color_temp = color_temp;
            state_color_temp = "00:go_up";
            color_temp_alpha = 0;
            color_temp_deladd = alpha_add;
            color_temp_delsub = alpha_sub;
        }




    }
}
