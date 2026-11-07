using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    [CustomEntity("MeliHelper/CustomRubikPointsDoor")]
    class CustomRubikPointsDoor : Entity
    {
        static string SPRITE_SPIRAL      = "00;10;20;30;40;50;60;70;80;81;82;83;84;85;86;87;77;67;57;47;37;36;35;34;44;55;65;64;63;62;52;42;32;22;12;13;14;15;16;17;18;19;29;39;49;59;69;79;89;99";
        static string SPRITE_HEART_EMPTY = "21;31;42;52;61;71;82;83;84;85;76;67;58;48;37;26;15;14;13;12";
        static string SPRITE_HEART_FULL  = "21;31;61;71;12;22;32;42;52;62;72;82;13;23;33;43;53;63;73;83;14;24;34;44;54;64;74;84;15;25;35;45;55;65;75;85;26;36;46;56;66;76;37;47;57;67;48;58";
        static string SPRITE_HEART_BIG   = "6,2;7,2;8,2;9,2;14,2;15,2;16,2;17,2;5,3;6,3;7,3;8,3;9,3;10,3;13,3;14,3;15,3;16,3;17,3;18,3;4,4;5,4;6,4;7,4;8,4;9,4;10,4;11,4;12,4;13,4;14,4;15,4;16,4;17,4;18,4;19,4;3,5;4,5;5,5;6,5;7,5;8,5;9,5;10,5;11,5;12,5;13,5;14,5;15,5;16,5;17,5;18,5;19,5;20,5;2,6;3,6;4,6;5,6;6,6;7,6;8,6;9,6;10,6;11,6;12,6;13,6;14,6;15,6;16,6;17,6;18,6;19,6;20,6;21,6;2,7;3,7;4,7;5,7;6,7;7,7;8,7;9,7;10,7;11,7;12,7;13,7;14,7;15,7;16,7;17,7;18,7;19,7;20,7;21,7;2,8;3,8;4,8;5,8;6,8;7,8;8,8;9,8;10,8;11,8;12,8;13,8;14,8;15,8;16,8;17,8;18,8;19,8;20,8;21,8;2,9;3,9;4,9;5,9;6,9;7,9;8,9;9,9;10,9;11,9;12,9;13,9;14,9;15,9;16,9;17,9;18,9;19,9;20,9;21,9;2,10;3,10;4,10;5,10;6,10;7,10;8,10;9,10;10,10;11,10;12,10;13,10;14,10;15,10;16,10;17,10;18,10;19,10;20,10;21,10;2,11;3,11;4,11;5,11;6,11;7,11;8,11;9,11;10,11;11,11;12,11;13,11;14,11;15,11;16,11;17,11;18,11;19,11;20,11;21,11;2,12;3,12;4,12;5,12;6,12;7,12;8,12;9,12;10,12;11,12;12,12;13,12;14,12;15,12;16,12;17,12;18,12;19,12;20,12;21,12;3,13;4,13;5,13;6,13;7,13;8,13;9,13;10,13;11,13;12,13;13,13;14,13;15,13;16,13;17,13;18,13;19,13;20,13;4,14;5,14;6,14;7,14;8,14;9,14;10,14;11,14;12,14;13,14;14,14;15,14;16,14;17,14;18,14;19,14;5,15;6,15;7,15;8,15;9,15;10,15;11,15;12,15;13,15;14,15;15,15;16,15;17,15;18,15;6,16;7,16;8,16;9,16;10,16;11,16;12,16;13,16;14,16;15,16;16,16;17,16;7,17;8,17;9,17;10,17;11,17;12,17;13,17;14,17;15,17;16,17;8,18;9,18;10,18;11,18;12,18;13,18;14,18;15,18;9,19;10,19;11,19;12,19;13,19;14,19;10,20;11,20;12,20;13,20;11,21;12,21";
        static string SPRITE_STRAWBERRY  = "61;71;32;42;62;72;82;43;53;63;73;34;44;54;64;74;84;25;35;45;55;65;75;85;95;26;36;46;56;66;76;86;96;27;37;47;57;67;77;87;97;28;38;48;58;68;78;88;98;39;49;59;69;79;89;4,10;5,10;6,10;7,10;5,11;6,11";

        //static string SPIRAL_CENTER_UPDOWN = "44;34;35;36;37;47;57;67;77;87;86;85;84;83;82;81;80;70;60;50;40;30;20;10;00;55;65;64;63;62;52;42;32;22;12;13;14;15;16;17;18;19;29;39;49;59;69;79;89;99";
        //static string SPIRAL_CENTER_DOWNUP = "55;65;64;63;62;52;42;32;22;12;13;14;15;16;17;18;19;29;39;49;59;69;79;89;99;44;34;35;36;37;47;57;67;77;87;86;85;84;83;82;81;80;70;60;50;40;30;20;10;00";
        //static string SPRITE_STRAWBERRY_CENTER = "56;66;67;57;47;46;45;55;65;75;76;77;78;68;58;48;38;37;36;35;34;44;54;64;74;84;85;86;87;88;89;79;69;59;49;39;28;27;26;25;43;53;63;73;95;96;97;98;7,10;6,10;5,10;4,10;32;42;62;72;82;6,11;5,11;61;71";

        Level level;
        Player player;
        Solid solidTop, solidBottom;
        Rectangle rect_draw, rect_open;
        MTexture texture_mist, texture_edge, texture_top;

        CustomTimer timer_points, timer_close;
        Vector2 pos_text, pos_points;
        int points_show, points_required, points_current, length_points, speed_open, speed_close;
        string appear, flag;

        Color color;
        CustomHeartDoorSprite[] mass_sprites;
        int id, w, h, rows, sprite_size, sprite_points;
        float edge_shift, flashbang_alpha, height, mist_x, mist_y;
        bool is_opened, is_opened_visual;

        public CustomRubikPointsDoor(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            id = data.ID;
            w = data.Width;
            h = data.Height;
            color = Calc.HexToColor(data.Attr("color"));
            timer_points = new CustomTimer(data.Float("delay",      0.03f));
            timer_close  = new CustomTimer(data.Float("delayClose", 0.01f));
            points_required = data.Int("requires");
            points_current  = MeliHelperModule.Instance.Session.Points;
            //points_current = 10 * MeliHelperModule.Settings.HeartDoorStart10;
            points_show = 0;
            length_points = points_required.ToString().Length;
            sprite_points = data.Int("spritePoints", 100);
            speed_open  = data.Int("speedOpen",  2);
            speed_close = data.Int("speedClose", 3);
            this.appear = data.Attr("appear", "All the time");

            texture_mist = GFX.Game["objects/heartdoor/mist"];
            texture_edge = GFX.Game["objects/heartdoor/edge"];
            texture_top  = GFX.Game["objects/heartdoor/top"];


            height = h / 2;
            rect_draw = new Rectangle((int)Position.X, (int)Position.Y, w,  h);
            rect_open = new Rectangle((int)Position.X + (data.Attr("direction", "LeftToRight") == "LeftToRight" ? -24 : w), (int)Position.Y, 24, h);


            Color color_fill  = Calc.HexToColor(data.Attr("colorSpriteFill",  "0037FF"));
            Color color_empty = Calc.HexToColor(data.Attr("colorSpriteEmpty", "91A5FF"));
            string sprite_type = data.Attr("sprite", "Spiral");
            string sprite_code = (sprite_type == "Spiral"     ) ? SPRITE_SPIRAL :
                                 (sprite_type == "Heart full" ) ? SPRITE_HEART_FULL :
                                 (sprite_type == "Heart empty") ? SPRITE_HEART_EMPTY :
                                 (sprite_type == "Heart big"  ) ? SPRITE_HEART_BIG :
                                 (sprite_type == "Strawberry" ) ? SPRITE_STRAWBERRY :
                                 "";

            int cols = data.Int("columns", 2);
            int sprites = points_required / sprite_points;
            int sprites_dy = data.Int("spritesDY");
            int text_dy = data.Int("textDY", 8);
            sprite_size = data.Int("spriteSize", 12);

            List<int> mass_rows = new List<int>();
            int temp = sprites;
            while (temp > 0)
            {
                int val = (temp > cols) ? cols : temp;
                mass_rows.Add(val);
                temp -= val;
            }

            int r = 0, c = 0;
            mass_sprites = new CustomHeartDoorSprite[sprites];
            for (int i = 0; i < mass_sprites.Length; i++)
            {
                mass_sprites[i] = new CustomHeartDoorSprite(
                    new Vector2(
                        (int)Position.X + w / 2 + sprite_size * (c - 0.5f * mass_rows[r]),
                        (int)Position.Y + h / 2 + sprite_size * r + text_dy + sprites_dy
                        ),
                    sprite_code, color_fill, color_empty,
                    -text_dy - sprites_dy - sprite_size / 2 + h / 2 - Math.Abs(sprite_size * r));

                if (++c >= cols)
                {
                    r++;
                    c = 0;
                }
            }

            pos_text   = Position + new Vector2(w / 2 - CustomTextController.GetTextLength("POINTS") / 2, h / 2 - 4 - text_dy + sprites_dy);
            pos_points = Position + new Vector2(w / 2 - CustomTextController.GetTextLength("/" + new string('0', 2 * length_points)) / 2, h / 2 + 4 - text_dy + sprites_dy);
        }

        public void UpdateGolden(Level level)
        {
            bool val_new = !(appear == "Golden only" && !level.Session.GrabbedGolden || appear == "Not golden only" && level.Session.GrabbedGolden);
            bool val_old = Visible;
            
            if (val_new && !val_old)
            {
                level.Add(solidTop);
                level.Add(solidBottom);
            }
            else if (!val_new && val_old)
            {
                level.Remove(solidTop);
                level.Remove(solidBottom);
            }
            Visible = val_new;
            Active  = val_new;
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            level = scene as Level;
            player = level.Tracker.GetEntity<Player>();
            //level.Add(solidTop = new Solid(Position, w, height, true));
            //level.Add(solidBottom = new Solid(Position + new Vector2(0, h - height), w, height, true));
            
            solidTop    = new Solid(Position, w, height, true);
            solidBottom = new Solid(Position + new Vector2(0, h - height), w, height, true);
            Visible = false; Active = false;
            UpdateGolden(level);

            flag = "MeliHelper_CustomRubikPointsDoorOpened_" + id;
            if (level.Session.GetFlag(flag))
                OpenInstantly();
        }

        public override void Update()
        {
            base.Update();
            mist_x += Engine.DeltaTime * 17;
            mist_y += Engine.DeltaTime * 29;
            if (mist_x >= 160) mist_x -= 160;
            if (mist_y >= 160) mist_y -= 160;
            if (!is_opened)
            {
                edge_shift += 12 * Engine.DeltaTime;
                if (edge_shift >= 8)
                    edge_shift -= 8;
            }


            if (!is_opened && player != null && !player.Dead)
            {
                if (points_show < points_current && player.CollideRect(rect_open) && timer_points.Tick())
                {
                    for (int i = 0; i < speed_open; i++)
                        if (points_show < points_current && ++points_show % 10 == 0)
                            Audio.Play(SFX.game_09_frontdoor_heartfill);

                    UpdateSprites();
                    if (points_show >= points_required)
                    {
                        is_opened = true;
                        points_show = points_required;
                        level.Session.SetFlag(flag);
                        Audio.Play(SFX.game_09_frontdoor_unlock);
                        Add(new Coroutine(CoroutineTryOpen()));
                    }
                }
                else if (points_show > 0 && !player.CollideRect(rect_open) && timer_close.Tick())
                {
                    //for (int i = 0; i < speed_close; i++)
                    //    if (points_show > 0 && --points_show % 20 == 0)
                    //        Audio.Play(SFX.char_tutghost_disappear);

                    points_show -= speed_close;
                    if (points_show < 0) points_show = 0;
                    UpdateSprites();
                }
            }
        }

        void UpdateSprites()
        {
            float temp_koef = 1f * points_show / sprite_points;
            foreach (var item in mass_sprites)
            {
                if (temp_koef >= 1)      item.SetCountFill(1);
                else if (temp_koef <= 0) item.SetCountFill(0);
                else                     item.SetCountFill(temp_koef);
                temp_koef--;
            }
        }

        IEnumerator CoroutineTryOpen()
        {
            yield return 0.6f;

            Audio.Play(SFX.game_09_frontdoor_unlock);
            bool is_have_center = (rows % 2 == 1);
            foreach (CustomHeartDoorSprite sprite in mass_sprites)
            {
                //sprite.GetSprite.Color = Color.White * 0.5f;
                float accet = 8;
                //if (is_have_center)
                //    accet += (sprite.GetDirection.Y < 0) ? 8 : -4;
                sprite.SetAccet(accet);
            }
            is_opened_visual = true;
            Depth = -9999999;


            // Flashbang time: 0.6 / 5 + 0.6 / 2 = 0.12 + 0.3 = 0.42 seconds;
            while (flashbang_alpha < 0.6f)
            {
                flashbang_alpha += 5 * Engine.DeltaTime;
                yield return null;
            }
            while (flashbang_alpha > 0)
            {
                flashbang_alpha -= 2 * Engine.DeltaTime;
                yield return null;
            }
            yield return 0.6f;


            for (int i = -1440; i <= 1440; i += 2)
            {
                level.ParticlesFG.Emit(HeartGemDoor.P_Shimmer, Position + new Vector2(i, h / 2));
                level.ParticlesFG.Emit(HeartGemDoor.P_Slice,   Position + new Vector2(i, h / 2));
            }

            while (height > 8)
            {
                height -= 120 * Engine.DeltaTime;
                foreach (CustomHeartDoorSprite sprite in mass_sprites)
                    sprite.AddAccet(120 * Engine.DeltaTime);

                solidTop   .Collider.Height = height;
                solidBottom.Collider.Height = height;
                solidBottom.Y = Position.Y + h - height;
                yield return null;
            }
            height = 8;

            is_opened = true;
        }

        void OpenInstantly()
        {
            is_opened = true;
            is_opened_visual = true;
            edge_shift = 0;
            height = 8;
            Depth = -9999999;
            level.Session.SetFlag(flag);

            // Change solids
            solidTop   .Collider.Height = height;
            solidBottom.Collider.Height = height;
            solidBottom.Y = Position.Y + h - height;
            foreach (CustomHeartDoorSprite sprite in mass_sprites)
                sprite.SetAccet(int.MaxValue);
        }

        public override void Render()
        {
            //base.Render();

            if (!is_opened)
            {
                Draw.Rect(rect_draw, color);
                texture_mist.GetSubtexture((int)mist_x, (int)mist_y, w, h).Draw(Position);
            }
            else
            {
                Draw.Rect(new Rectangle((int)Position.X, (int)Position.Y, w, (int)height), color);
                Draw.Rect(new Rectangle((int)Position.X, (int)Position.Y + h - (int)height, w, (int)height), color);

                MTexture texture = texture_mist.GetSubtexture((int)mist_x, (int)mist_y, w, (int)height);
                texture.Draw(Position);
                texture.Draw(Position + new Vector2(0, h - height));


            }
            //Draw.Rect(new Rectangle(0, 0, w, h), Color.Red);
            //Draw.Rect(new Rectangle((int)Position.X, (int)Position.Y, w, h), Color.Blue);
            //Draw.Rect(rect_draw, color);



            if (!is_opened_visual)
            {
                // Left and right edges
                for (int i = -1; i < height / 4; i++)
                {
                    texture_edge.Draw(Position + new Vector2(8, edge_shift + 8 * i), Vector2.Zero, Color.White, new Vector2(-1, 1));
                    texture_edge.Draw(Position + new Vector2(w - 8, 8 - edge_shift + 8 * i));
                }
            }
            else
            {
                // Edges for each corner
                Color clr = Color.White * 0.7f;
                for (int i = 0; i < height / 8; i++)
                {
                    texture_edge.Draw(Position + new Vector2(8, 8 * i), Vector2.Zero, clr, new Vector2(-1, 1));
                    texture_edge.Draw(Position + new Vector2(8, h - 8 * i - 8), Vector2.Zero, clr, new Vector2(-1, 1));
                    texture_edge.Draw(Position + new Vector2(w - 8, 8 * i), Vector2.Zero, clr);
                    texture_edge.Draw(Position + new Vector2(w - 8, h - 8 * i - 8), Vector2.Zero, clr);
                }

                // Top and bottom
                for (int i = 0; i < w / 8; i++)
                {
                    texture_top.Draw(Position + new Vector2(8 * i, height), Vector2.Zero, clr, new Vector2(1, -1));
                    texture_top.Draw(Position + new Vector2(8 * i, h - height), Vector2.Zero, clr, new Vector2(1, 1));
                }
            }

            // Sprites
            foreach (CustomHeartDoorSprite sprite in mass_sprites)
                sprite.Render();

            // Text
            if (!is_opened)
            {
                CustomTextController.DrawText(Dialog.Get("EVIDENCE02_RCM_MESSAGE_DOORPOINTS"), pos_text, Color.White);
                CustomTextController.DrawText(points_show.ToString(new string('0', length_points)) + "/" + points_required, pos_points, Color.White);
            }


            if (flashbang_alpha > 0)
                Draw.Rect((int)Position.X - 2000, (int)Position.Y - 1000, 4000, 2000, color * flashbang_alpha);
        }
    }


    class CustomHeartDoorSprite
    {
        Vector2[] mass_pixels;
        Vector2 start, pos;
        Color color_fill, color_empty;
        float accet, accet_max;
        int cnt_fill;

        public CustomHeartDoorSprite(Vector2 pos, string list_pixels, Color color_fill, Color color_empty, float accet_max)
        {
            this.color_fill = color_fill;
            this.color_empty = color_empty;
            this.start = pos;
            this.pos = pos;
            this.accet = 0;
            this.accet_max = accet_max;

            string[] mass_pixels_str = list_pixels.Split(';');
            mass_pixels = new Vector2[mass_pixels_str.Length];
            for (int i = 0; i < mass_pixels_str.Length; i++)
            {
                string px = mass_pixels_str[i];
                if (px.Contains(','))
                {
                    int len1 = px.IndexOf(',');
                    int len2 = px.Length - len1 - 1;
                    mass_pixels[i] = new Vector2(int.Parse(px.Substring(0, len1)), int.Parse(px.Substring(px.Length - len2)));
                }
                else
                    mass_pixels[i] = new Vector2(px[0] - 48, px[1] - 48);
            }
        }

        public void SetAccet(float accet)
        {
            this.accet = accet;
            this.pos = start + new Vector2(0, accet);
        }

        public void AddAccet(float add)
        {
            SetAccet(accet + add);
        }

        public void SetCountFill(float koef)
        {
            this.cnt_fill = (int)(koef * mass_pixels.Length);
        }

        public void Render()
        {
            if (accet < accet_max)
            {
                for (int i = 0; i < mass_pixels.Length; i++)
                    Draw.Rect(pos + mass_pixels[i], 1, 1, i < cnt_fill ? color_fill : color_empty);
            }
        }
    }
}
