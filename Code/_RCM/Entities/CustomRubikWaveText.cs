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
    class CustomRubikWaveText : Entity
    {
        Player player;
        Vector2 del;
        CustomTimer timer;
        Color clr_normal;
        Color[] mass_color_wave;
        string text;
        int id_symbol;
        float delay;
        float ttl, alpha;

        public CustomRubikWaveText(Player player, string text, float ttl = 7f)
        {
            this.player = player;
            this.del = new Vector2(-CustomTextController.GetTextLength(text) / 2, 0);
            this.clr_normal = Color.White;
            this.mass_color_wave = new Color[5];
            mass_color_wave[0] = Calc.HexToColor("FF0000");   // Red
            mass_color_wave[1] = Calc.HexToColor("FF8800");   // Orange
            mass_color_wave[2] = Calc.HexToColor("FFFF00");   // Yellow
            mass_color_wave[3] = Calc.HexToColor("00FF00");   // Green
            mass_color_wave[4] = Calc.HexToColor("0000FF");   // Blu

            this.text = text;
            this.timer = new CustomTimer(0.06f);
            this.ttl = ttl;
            this.alpha = 1f;
            Depth = -9999999;
            Restart();
        }

        void Restart()
        {
            id_symbol = -3;
            delay = 0.7f;
        }

        public override void Update()
        {
            base.Update();
            if (delay > 0)
                delay -= Engine.DeltaTime;
            else if (timer.Tick() && ++id_symbol >= text.Length + 1)
                Restart();


            if (player == null || player.Dead)
                RemoveSelf();
            if (ttl > 0)
                ttl -= Engine.DeltaTime;
            else if (alpha > 0)
                alpha -= Engine.DeltaTime / 0.4f;
            else
                RemoveSelf();
        }

        public override void Render()
        {
            base.Render();

            Vector2 temp = player.Center + new Vector2(0, -24) + this.del;
            Vector2 del = Vector2.Zero;
            for (int i = 0; i < text.Length; i++)
            {
                del = (i == id_symbol - 1) ? new Vector2(0, -1) 
                    : (i == id_symbol    ) ? new Vector2(0, +1) 
                    : Vector2.Zero;
                CustomTextController.DrawText(text[i].ToString(), temp + del, ((i == id_symbol - 1 || i == id_symbol) ? mass_color_wave[i % mass_color_wave.Length] : clr_normal) * alpha);
                temp.X += CustomTextController.GetTextLength(text[i].ToString()) + 1;
            }

        }
    }
}
