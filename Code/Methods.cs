using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class Methods
    {
        static Random rand = new Random();
        
        public static Random GetRandomizer(bool is_c_sharp = true)
        {
            return is_c_sharp ? rand : Calc.Random;
        }

        public static Vector2 DirectionToVector(DirectionEnum dir)
        {
            Vector2 v = Vector2.Zero;
            switch (dir)
            {
                case DirectionEnum.Left: v.X--; break;
                case DirectionEnum.Right: v.X++; break;
                case DirectionEnum.Up: v.Y--; break;
                case DirectionEnum.Down: v.Y++; break;
            }
            return v;
        }

        public static void GetDirectionParams(DirectionEnum dir, ref int dx, ref int dy)
        {
            dx = 0; dy = 0;
            switch (dir)
            {
                case DirectionEnum.Left: dx--; break;
                case DirectionEnum.Right: dx++; break;
                case DirectionEnum.Up: dy--; break;
                case DirectionEnum.Down: dy++; break;
            }
        }
		
		
        public static Color GetColorBetween(Color colorFrom, Color colorTo, float alphaTo)
        {
            return new Color(
                (int)(colorTo.R * alphaTo + colorFrom.R * (1f - alphaTo)),
                (int)(colorTo.G * alphaTo + colorFrom.G * (1f - alphaTo)),
                (int)(colorTo.B * alphaTo + colorFrom.B * (1f - alphaTo))
                );
        }

        public static Color GetColorFromString(string color)
        {
            switch (color)
            {
                case "Red": return Color.Red;
                case "Orange": return Color.OrangeRed;
                case "Blue": return Color.RoyalBlue;
                case "Green": return Color.ForestGreen;
                case "Yellow": return Color.Yellow;
                case "Purple": return Color.DarkViolet;
                case "White": return Color.White;
                case "Black": return Color.Black;
                case "Gray": return Color.Gray;

                default:
                    if (color.Length != 6) return Color.Black;

                    int R = 0, G = 0, B = 0;
                    for (int i = 0; i < color.Length; i++)
                    {
                        char ch = Char.ToUpper(color[i]);
                        int val = 0;
                        if (ch >= 48 && ch <= 57)
                            val = ch - 48;
                        else if (ch >= 'A' && ch <= 'F')
                            val = 10 + ch - 'A';
                        else return Color.Black;

                        switch (i)
                        {
                            case 0: R += 16 * val; break;
                            case 1: R += val; break;
                            case 2: G += 16 * val; break;
                            case 3: G += val; break;
                            case 4: B += 16 * val; break;
                            case 5: B += val; break;
                        }
                    }
                    return new Color(R, G, B);
            }

        }

        public static Color GetColorHSV(float value)
        {
            //   0 red      (255,   0,   0)
            //  60 yellow   (255, 255,   0)
            // 120 green    (  0, 255,   0)
            // 180 cyan     (  0, 255, 255)
            // 240 blue     (  0,   0, 255)
            // 300 purple   (255,   0, 255)
            // 360 red      (255,   0,   0)
            value = value % 360;

            int R = 0, G = 0, B = 0;
            int koef = (int)(255f * (value % 60) / 60);
            switch ((int)value / 60)
            {
                case 0: R = 255; G = koef; break;
                case 1: G = 255; R = 255 - koef; break;
                case 2: G = 255; B = koef; break;
                case 3: B = 255; G = 255 - koef; break;
                case 4: B = 255; R = koef; break;
                case 5: R = 255; B = 255 - koef; break;
            }

            return new Color(R, G, B);
        }







        public static bool isAreaCompleted(Level level)
        {
            return isAreaCompleted(level.Session); 
        }

        public static bool isAreaCompleted(Session session)
        {
            return SaveData.Instance.CheatMode || SaveData.Instance.Areas_Safe[session.Area.ID].Modes[(int)session.Area.Mode].Completed;
        }
        
        public static bool PlayerHaveGolden(Level level)
        {
            if (level == null) return false;

            Player player = level.Tracker.GetEntity<Player>();
            return (player != null && PlayerHaveGolden(player));
        }

        public static bool PlayerHaveGolden(Player player)
        {
            foreach (Follower berry in player.Leader.Followers)
                if (berry.Entity is Strawberry && (berry.Entity as Strawberry).Golden && !(berry.Entity as Strawberry).Winged)
                    return true;

            return false;
        }

        public static Player GetPlayerOnScene(Scene scene)
        {
            return (scene as Level).Tracker.GetEntity<Player>();
        }

        public static void PlayerLock(Player player, bool value = true)
        {
            if (player == null) return;

            if (value)
            {
                player.StateMachine.State = Player.StDummy;
                player.StateMachine.Locked = true;
                player.ForceCameraUpdate = true;
            }
            else
            {
                player.StateMachine.Locked = false;
                player.StateMachine.State = 0;
                player.ForceCameraUpdate = false;
            }
        }

        public static bool PlayerIsAlive(Player player)
        {
            return player != null && !player.Dead;
        }

        public static bool PlayerCanMove(Player player)
        {
            return !(player == null
                    || player.StateMachine.State == Player.StDummy
                    || player.StateMachine.State == Player.StIntroRespawn
                    || player.StateMachine.State == Player.StIntroWakeUp);
        }

        public static Vector2 GetMouseCoords(Level level)
        {
            return CoordsFromHUD(level, MInput.Mouse.Position);
        }

        public static Vector2 CoordsFromHUD(Level level, Vector2 position)
        {
            float zoomKoef = GetZoomKoefHUD(level);
            return new Vector2(level.Camera.X + position.X / (zoomKoef * level.Camera.Zoom),
                               level.Camera.Y + position.Y / (zoomKoef * level.Camera.Zoom));
        }

        public static Vector2 CoordsToHUD(Level level, Vector2 position)
        {
            return GetZoomKoefHUD(level) * level.Camera.Zoom * (position - new Vector2(level.Camera.X, level.Camera.Y));
        }

        public static float GetZoomKoefHUD(Level level)
        {
            return 1080 / (level.Camera.Bottom - level.Camera.Top);
        }

        public static bool isInCamera(Level level, Vector2 position, int padding = 0)
        {
            return level.Camera.Left - padding <= position.X && position.X <= level.Camera.Right  + padding
                && level.Camera.Top  - padding <= position.Y && position.Y <= level.Camera.Bottom + padding;
        }

        public static void CreateTiles(Solid entity, char tiletype, bool blendIn)
        {
            TileGrid tileGrid;
            Level level = entity.SceneAs<Level>();
            if (!blendIn)
            {
                tileGrid = GFX.FGAutotiler.GenerateBox(tiletype, (int)entity.Width / 8, (int)entity.Height / 8).TileGrid;
                entity.Add(new LightOcclude());
            }
            else
            {
                Rectangle tileBounds = level.Session.MapData.TileBounds;
                VirtualMap<char> solidsData = level.SolidsData;
                int x = (int)(entity.X / 8f) - tileBounds.Left;
                int y = (int)(entity.Y / 8f) - tileBounds.Top;
                int tilesX = (int)entity.Width / 8;
                int tilesY = (int)entity.Height / 8;
                tileGrid = GFX.FGAutotiler.GenerateOverlay(tiletype, x, y, tilesX, tilesY, solidsData).TileGrid;
                entity.Add(new EffectCutout());
                entity.Depth = -10501;
            }
            entity.Add(tileGrid);
            entity.Add(new TileInterceptor(tileGrid, highPriority: true));
        }

        public static ButtonBinding GetButtonBinding(string button)
        {
            switch (button)
            {
                case "BattleCity_Shoot": return MeliHelperModule.Settings.BattleCity_Shoot; break;
                case "Minesweeper_ChangeDashMode": return MeliHelperModule.Settings.Minesweeper_ChangeDashMode; break;
            }
            return null;
        }




        #region old unused things ?

        public static Vector2 RectangleDirection(Rectangle rect, Vector2 point)
        {
            Vector2 res = Vector2.Zero;
            float dx = point.X - rect.Center.X;
            float dy = point.Y - rect.Center.Y;
            if (Math.Abs(dx / rect.Width) > Math.Abs(dy / rect.Height)) res.X = Math.Sign(dx);
            else res.Y = Math.Sign(dy);
            return res;
        }

        public static void RestoreSpriteAfterStop(Sprite sprite)
        {
            sprite.Play(sprite.CurrentAnimationID);
        }

        public static IEnumerator GlitchCoroutine(float value = 0.4f, float speed = 1f)
        {
            Glitch.Value = value;
            while (Glitch.Value > 0)
            {
                Glitch.Value -= speed * Engine.DeltaTime;
                yield return null;
            }
        }

        public static IEnumerator TrailCoroutine(Entity entity, Color color, float timer)
        {
            while (timer > 0)
            {
                TrailManager.Add(entity, color, 1);
                timer -= Engine.DeltaTime;
                yield return null;
            }
        }

        public static IEnumerator TrailGradientCoroutine(Entity entity, Color color1, Color color2, float timer)
        {
            float timer_max = timer;
            while (timer > 0)
            {
                TrailManager.Add(entity, GetColorBetween(color2, color1, timer / timer_max), 1);
                timer -= Engine.DeltaTime;
                yield return null;
            }
        }

        public static IEnumerator LightCoroutine(Entity entity, Color color, float timer,
            int startRadius = 32, int endRadius = 64)
        {
            VertexLight light = new VertexLight(color, 1f, startRadius, endRadius);
            entity.Add(light);

            float timer_max = timer;
            while (timer > 0)
            {
                light.StartRadius = startRadius * timer / timer_max;
                light.EndRadius = endRadius * timer / timer_max;
                timer -= Engine.DeltaTime;
                yield return null;
            }
            //if (entity != null)
            //    entity.Remove(light);
        }

        public static IEnumerator RumbleCoroutine(float timer)
        {
            while (timer > 0)
            {
                Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
                timer -= Engine.DeltaTime;
                yield return null;
            }
        }

        #endregion
    }
}
