using Microsoft.Xna.Framework;
using Monocle;
using System;

namespace Celeste.Mod.MeliHelper
{
    class StickerFlickering
    {
        CustomTimer timer;
        Color[] mass_colors;
        int id;
        
        public StickerFlickering(Color color, int length)
        {
            id = Calc.Random.Next(0, length);
            timer = new CustomTimer(0.02f + Calc.Random.NextFloat(0.10f));

            mass_colors = new Color[length];
            for (int i = 0; i < length; i++)
            {
                float koef = 1f + 0.1f * (float)Math.Sin(2 * i * Math.PI / length);
                int R = Math.Min(255, (int)(koef * color.R));
                int G = Math.Min(255, (int)(koef * color.G));
                int B = Math.Min(255, (int)(koef * color.B));
                mass_colors[i] = Color.FromNonPremultiplied(R, G, B, 255);
            }
        }

        public void Update()
        {
            if (timer.Tick() && ++id >= mass_colors.Length)
                id = 0;
        }

        public Color GetCurrentColor()
        {
            return mass_colors[id];
        }

    }
}
