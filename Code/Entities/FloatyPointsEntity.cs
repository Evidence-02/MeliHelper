using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class FloatyPointsEntity : Entity
    {
        Color color;
        int[] mass_indexes;
        int id_frame, count_points, current_points, state, length, point_multiplyer;
        float del_y, state_timer, state_timer_max, frame_timer;

        public FloatyPointsEntity(Vector2 position, int points, int point_multiplyer = 1, bool is_increase = false, float time_stay = 0.2f) : base(position)
        {
            FloatyPointsEntity.Initialize();
            this.count_points = points * Math.Max(1, point_multiplyer);
            this.length = (int)Math.Log10(this.count_points) + 1;
            this.mass_indexes = new int[length];
            this.state_timer_max = time_stay;
            this.point_multiplyer = point_multiplyer;
            this.del_y = (point_multiplyer <= 1) ? 0 : (4 * point_multiplyer - 4);
            this.color = (point_multiplyer >= 0 && point_multiplyer < mass_colors.Length) 
                ? mass_colors[point_multiplyer] : mass_colors[mass_colors.Length - 1];
            if (is_increase)
            {
                state_timer = 0.1f;
                state = 1;
            }
            else
            {
                current_points = this.count_points;
                state_timer = state_timer_max;
            }

            Position -= new Vector2(DEPTH * length / 2, 10);
            Depth = DepthController.MELI_TEXT;
            UpdateTextures();
        }

        public override void Update()
        {
            base.Update();            
            switch (state)
            {
                // Initiating! (only if permanent...)
                case 0:
                    frame_timer += Engine.DeltaTime;
                    if (frame_timer >= 0.12f)
                    {
                        frame_timer = 0;
                        if (++id_frame >= mass_textures_init.Length)
                        {
                            id_frame = 0;
                            GoToState2();
                        }
                    }
                    break;

                // ...or visual increase of points (only not permanent)
                case 1:
                    if (state_timer > 0)
                        state_timer -= Engine.DeltaTime;
                    else
                    {
                        //frame_timer += Engine.DeltaTime;
                        //if (frame_timer >= 0.08f)
                        {
                            //frame_timer = 0;
                            current_points += (int)(0.1f * (count_points - current_points) + 1);
                            if (current_points >= count_points)
                                GoToState2();
                            UpdateTextures();
                        }
                    }
                    break;

                // Stay still some time
                case 2:
                    state_timer -= Engine.DeltaTime;
                    if (state_timer <= 0)
                        state++;
                    break;

                // Move 
                case 3:
                    Position.Y -= del_y * Engine.DeltaTime;
                    frame_timer += Engine.DeltaTime;
                    if (frame_timer >= 0.14f)
                    {
                        frame_timer = 0;
                        if (++id_frame >= COUNT_FRAMES)
                            RemoveSelf(); 
                    }
                    break;

                default:
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
                        if (mass_indexes[i] != -1)
                            mass_textures[mass_indexes[i], id_frame].Draw(Position + new Vector2(DEPTH * i, 0), Vector2.Zero, color);
            }
        }

        void GoToState2()
        {
            current_points = count_points;
            state_timer = state_timer_max;
            state = 2;
        }

        void UpdateTextures()
        {
            for (int i = 0; i < length; i++)
                mass_indexes[i] = -1;

            int cp = current_points;
            int index = mass_indexes.Length - 1;
            while (cp > 0)
            {
                mass_indexes[index--] = cp % 10;
                cp /= 10; 
            }
        }


        #region StaticLoad
        
        static MTexture[,] mass_textures;
        static MTexture[] mass_textures_init;
        static Color[] mass_colors;
        static int COUNT_FRAMES = 7;
        static int DEPTH = 4;

        public static void Initialize()
        {
            if (mass_textures_init != null) 
                return;

            mass_textures_init = new MTexture[2];
            for (int i = 0; i < mass_textures_init.Length; i++)
                mass_textures_init[i] = GFX.Game["Evidence02/objects_melihelper/floaty_points/idleInit" + i.ToString("00")];

            mass_textures = new MTexture[10, COUNT_FRAMES];
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < COUNT_FRAMES; j++)
                    mass_textures[i, j] = GFX.Game["Evidence02/objects_melihelper/floaty_points/idle" + i.ToString() + j.ToString()];

            mass_colors = new Color[] { 
                Color.Gray,
                Color.White,
                // too invisible colors
                //BomberController.GetColorBetween(Color.White, Color.Pink, 0.5f),
                //BomberController.GetColorBetween(Color.White, Color.CornflowerBlue, 0.5f),
                Color.Pink,
                Color.CornflowerBlue,
                Color.OrangeRed,
                Color.RoyalBlue,
                Color.Red };
        }

        #endregion
    }
}
