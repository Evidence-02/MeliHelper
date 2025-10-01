using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class TextOutlineEntity : Entity
    {
		protected Color color;
        MTexture[,] mass_textures;
        MTexture[] mass_textures_init;
        int[] mass_lengths;
        int id_frame, state, length;
        float state_timer, state_timer_max, frame_timer;
        static int COUNT_FRAMES = 7;

        public TextOutlineEntity(Vector2 position, string word, Color color) : base(position)
        {
            this.color = color;
            this.length = word.Length;

            mass_textures_init = new MTexture[2];
            for (int i = 0; i < mass_textures_init.Length; i++)
                mass_textures_init[i] = GFX.Game[FontControllerOutline.GetFolder() + "init" + i.ToString("00")];

            mass_textures = new MTexture[length, COUNT_FRAMES];
            mass_lengths = new int[length];
            int sum_length = 0;
            for (int i = 0; i < length; i++)
            {
                char ch = word[i];
                int len = FontControllerOutline.GetCharLength(ch);
                mass_lengths[i] = sum_length;
                for (int j = 0; j < COUNT_FRAMES; j++)
                    if (ch != ' ')
                        mass_textures[i, j] = GFX.Game[FontControllerOutline.GetTexturePath(ch) + j];
                sum_length += len;
            }
            

            state_timer_max = 0.2f;
            Position -= new Vector2(sum_length / 2, 10);
            Depth = DepthController.MELI_TEXT;
        }

        public override void Update()
        {
            base.Update();
            switch (state)
            {
                // Initiating!
                case 0:
                    frame_timer += Engine.DeltaTime;
                    if (frame_timer >= 0.12f)
                    {
                        frame_timer = 0;
                        if (++id_frame >= mass_textures_init.Length)
                        {
                            id_frame = 0;
							state = 1;
							state_timer = state_timer_max;
                        }
                    }
                    break;

                // Stay still some time
                case 1:
                    state_timer -= Engine.DeltaTime;
                    if (state_timer <= 0)
                        state++;
                    break;

                // Move 
                case 2:
                    //Position.Y -= ;
                    frame_timer += Engine.DeltaTime;
                    if (frame_timer >= 0.14f)
                    {
                        frame_timer = 0;
                        if (++id_frame >= COUNT_FRAMES)
                            RemoveSelf(); 
                    }
                    break;
            }
        }

        public override void Render()
        {
            base.Render();
            if (state == 0)
            {
                mass_textures_init[id_frame].Draw(Position, Vector2.Zero, color);
            }
            else
            {
                if (id_frame < COUNT_FRAMES)
                    for (int i = 0; i < length; i++)
                        if (mass_textures[i, id_frame] != null)
                            mass_textures[i, id_frame].Draw(Position + new Vector2(mass_lengths[i], 0), Vector2.Zero, color);
            }
        }
    }
}
