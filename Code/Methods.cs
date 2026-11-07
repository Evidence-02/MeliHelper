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

        /// <summary>
        /// returns value from 0 to 360
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static float GetHueFromColor(Color color)
        {
            float R = color.R / 255f;
            float G = color.G / 255f;
            float B = color.B / 255f;
            float min = Math.Min(Math.Min(R, G), B);
            float max = Math.Max(Math.Max(R, G), B);
            if (min == max)
                return 0;

            float delta = max - min;
            if (R == max)
                return 120 * (G - B) / delta;
            else if (G == max)
                return 120 + 120 * (B - R) / delta;
            else
                return 240 + 120 * (R - G) / delta;
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

        public static void PlayerFixSubpixels(Player player)
        {
            player.Position = new Vector2((int)player.Position.X, (int)player.Position.Y);
        }

        public static void StayBehindPlayer(Level level, Entity entity)
        {
            Player player = level.Tracker.GetEntity<Player>();
            if (player != null && !player.Dead && entity.Depth < player.Depth + 1)
                entity.Depth = player.Depth + 1;
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

        public static Vector2 CoordsToHUDwithFloating(Level level, Vector2 pos, float koef_floating)
        {
            // Copied code from an old project again. But this time I modified it!
            Vector2 cam = level.Camera.Position;
            float zoom_koef = GetZoomKoefHUD(level);
            Vector2 posTmp = cam + new Vector2(960, 540) / zoom_koef;
            Vector2 res = (pos - cam + (pos - posTmp) * koef_floating) * zoom_koef;
            if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
                res.X = 1920f - res.X;
            return res;
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

        public static TileGrid CreateTiles(Level level, Solid entity, char tiletype, bool blendIn)
        {
            TileGrid tileGrid;
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
            return tileGrid;
        }

        public static ButtonBinding GetButtonBinding(string button)
        {
            switch (button)
            {
                case "BattleCity_Shoot": return MeliHelperModule.Settings.BattleCity_Shoot;
                case "Minesweeper_ChangeDashMode": return MeliHelperModule.Settings.Minesweeper_ChangeDashMode;
                case "BadelinePower_Switch": return MeliHelperModule.Settings.BadelinePower_Switch;
            }
            return null;
        }

        public static bool isKeyboardConnected()
        {
            return !Input.GuiInputController();
        }

        public static void DeleteSpikesAroundRect(Level level, Rectangle rect)
        {
            List<Spikes> list_spikes = level.Entities.FindAll<Spikes>();
            Rectangle rect_up = new Rectangle(rect.Left, rect.Top - 8, rect.Width, 8);
            foreach (Entity spike in list_spikes.FindAll(t => t.Direction == Spikes.Directions.Up && t.CollideRect(rect_up)))
                spike.RemoveSelf();

            Rectangle rect_down = new Rectangle(rect.Left, rect.Bottom, rect.Width, 8);
            foreach (Entity spike in list_spikes.FindAll(t => t.Direction == Spikes.Directions.Down && t.CollideRect(rect_down)))
                spike.RemoveSelf();

            Rectangle rect_left = new Rectangle(rect.Left - 8, rect.Top, 8, rect.Height);
            foreach (Entity spike in list_spikes.FindAll(t => t.Direction == Spikes.Directions.Left && t.CollideRect(rect_left)))
                spike.RemoveSelf();

            Rectangle rect_right = new Rectangle(rect.Right, rect.Top, 8, rect.Height);
            foreach (Entity spike in list_spikes.FindAll(t => t.Direction == Spikes.Directions.Right && t.CollideRect(rect_right)))
                spike.RemoveSelf();
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

        public static IEnumerator SpinnerShatterTempCoroutine(Entity entity, Vector2 accet, float radius, float delay = 1f)
        {
            SpinnerShatterComponent shatter = new SpinnerShatterComponent(accet, radius);
            entity.Add(shatter);
            yield return delay;
            entity.Components.Remove(shatter);
        }

        private static MethodInfo strawberryOnDash = typeof(Strawberry).GetMethod("OnDash", BindingFlags.NonPublic | BindingFlags.Instance);
        private static MethodInfo strawberryFlyAway = typeof(Strawberry).GetMethod("FlyAwayRoutine", BindingFlags.NonPublic | BindingFlags.Instance);
        public static void RegisterDash(Level level)
        {
            level.Session.Dashes++;

            Player player = level.Tracker.GetEntity<Player>();
            List<Strawberry> list_strawberries_winged = level.Entities.FindAll<Strawberry>()
                .FindAll(t => t.Winged);
            foreach (Strawberry berry in list_strawberries_winged)
            {
                strawberryOnDash.Invoke(berry, new object[] { player.Speed });

                // just flying away animation without actually flying away, funny
                //IEnumerator flyAwayRoutine = (IEnumerator)(strawberryFlyAway.Invoke(berry, new object[] { }));
                //berry.Add(new Coroutine(flyAwayRoutine));
            }
        }


        #region Vector3

        public static void GenerateCubeVectors(Vector3 v_center, float dist,
            ref Vector3 v000, ref Vector3 v001, ref Vector3 v010, ref Vector3 v011,
            ref Vector3 v100, ref Vector3 v101, ref Vector3 v110, ref Vector3 v111)
        {
            v000 = v_center + new Vector3(-dist, -dist, -dist);
            v001 = v_center + new Vector3(-dist, -dist, dist);
            v010 = v_center + new Vector3(-dist, dist, -dist);
            v011 = v_center + new Vector3(-dist, dist, dist);
            v100 = v_center + new Vector3(dist, -dist, -dist);
            v101 = v_center + new Vector3(dist, -dist, dist);
            v110 = v_center + new Vector3(dist, dist, -dist);
            v111 = v_center + new Vector3(dist, dist, dist);
        }

        public static Vector3[,,] GenerateVectorArray(Vector3 v_center, float dist, int length)
        {
            Vector3[,,] res = new Vector3[length, length, length];
            float xyz_start = -dist * (length - 1) / 2;
            for (int i = 0; i < length; i++)
                for (int j = 0; j < length; j++)
                    for (int k = 0; k < length; k++)
                        res[i, j, k] = new Vector3(v_center.X + xyz_start + i * dist, v_center.Y + xyz_start + j * dist, v_center.Z + xyz_start + k * dist);
            return res;
        }


        public static Vector3[,,] GenerateVectorArray(Vector3 v_center, float dist, int lenx, int leny, int lenz)
        {
            Vector3[,,] res = new Vector3[lenx, leny, lenz];
            float x_start = v_center.X - dist * (lenx - 1) / 2;
            float y_start = v_center.Y - dist * (leny - 1) / 2;
            float z_start = v_center.Z - dist * (lenz - 1) / 2;
            for (int x = 0; x < lenx; x++)
                for (int y = 0; y < leny; y++)
                    for (int z = 0; z < lenz; z++)
                        res[x, y, z] = new Vector3(x_start + x * dist, y_start + y * dist, z_start + z * dist);
            return res;
        }

        public static Vector3[,,] GenerateVectorArray(float[] coords_x, float[] coords_y, float[] coords_z)
        {
            int lenx = coords_x.Length;
            int leny = coords_y.Length;
            int lenz = coords_z.Length;

            Vector3[,,] res = new Vector3[lenx, leny, lenz];
            for (int x = 0; x < lenx; x++)
                for (int y = 0; y < leny; y++)
                    for (int z = 0; z < lenz; z++)
                        res[x, y, z] = new Vector3(coords_x[x], coords_y[y], coords_z[z]);
            return res;
        }


        public static void GetSurfaceEquation(Vector3 p1, Vector3 p2, Vector3 p3, ref float A, ref float B, ref float C, ref float D)
        {
            A = p1.Y * (p2.Z - p3.Z) + p2.Y * (p3.Z - p1.Z) + p3.Y * (p1.Z - p2.Z);
            B = p1.Z * (p2.X - p3.X) + p2.Z * (p3.X - p1.X) + p3.Z * (p1.X - p2.X);
            C = p1.X * (p2.Y - p3.Y) + p2.X * (p3.Y - p1.Y) + p3.X * (p1.Y - p2.Y);
            D = -(p1.X * (p2.Y * p3.Z - p3.Y * p2.Z) + p2.X * (p3.Y * p1.Z - p1.Y * p3.Z) + p3.X * (p1.Y * p2.Z - p2.Y * p1.Z));

            //A = y1 * (z2 - z3) + y2 * (z3 - z1) + y3 * (z1 - z2);
            //B = z1 * (x2 - x3) + z2 * (x3 - x1) + z3 * (x1 - x2);
            //C = x1 * (y2 - y3) + x2 * (y3 - y1) + x3 * (y1 - y2);
            //D = -(x1 * (y2 * z3 - y3 * z2) + x2 * (y3 * z1 - y1 * z3) + x3 * (y1 * z2 - y2 * z1));
        }

        public static bool isPointInsideOfThePolygon(Vector2[] points, Vector2 p)
        {
            if (points.Length < 3)
                return false;

            Vector2 p1, p2;
            bool inside = false;
            var oldPoint = new Vector2(
                points[points.Length - 1].X, points[points.Length - 1].Y);

            for (int i = 0; i < points.Length; i++)
            {
                var newPoint = new Vector2(points[i].X, points[i].Y);
                if (newPoint.X > oldPoint.X)
                {
                    p1 = oldPoint;
                    p2 = newPoint;
                }
                else
                {
                    p1 = newPoint;
                    p2 = oldPoint;
                }

                if ((newPoint.X < p.X) == (p.X <= oldPoint.X)
                    && (p.Y - (long)p1.Y) * (p2.X - p1.X)
                    < (p2.Y - (long)p1.Y) * (p.X - p1.X))
                {
                    inside = !inside;
                }

                oldPoint = newPoint;
            }

            return inside;
        }

        public static Vector3 GetCentralPoint(params Vector3[] vectors)
        {
            Vector3 central_point = Vector3.Zero;
            for (int i = 0; i < vectors.Length; i++) central_point += vectors[i] / vectors.Length;
            return central_point;
        }


        #endregion

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
