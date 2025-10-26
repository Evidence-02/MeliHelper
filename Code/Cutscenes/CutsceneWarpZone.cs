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
    class CutsceneWarpZone : CutsceneEntity
    {
        Player player;
        WarpZoneTexture[] mass_textures;
        WarpZone warp_zone;
        Vector2 room_spawnpoint;
        Player.IntroTypes intro_type;
        Color texture_color;
        string room_teleport, sound;
        int texture_type;
        float alpha_back, alpha_hole;
        bool is_update_canadian, is_show_cutscene;

        MTexture player_texture, hair_texture;
        Vector2 player_pos;
        Vector2[] mass_hair_hodes;
        float pcenter_angle, pcenter_dist;
        float player_rotation, player_scale;
        float player_sprite_rotation, player_sprite_rotation_del;

        public CutsceneWarpZone(Player player, WarpZone warp_zone, 
            string room_teleport, Vector2 room_spawnpoint, Player.IntroTypes intro_type,
            Color color, string sound, int texture_type, bool is_show_cutscene = true)
        {
            this.player = player;
            this.warp_zone = warp_zone;
            this.room_teleport = room_teleport;
            this.room_spawnpoint = room_spawnpoint;
            this.intro_type = intro_type;
            this.sound = sound;
            this.texture_color = color;
            this.texture_type = texture_type;
            this.is_show_cutscene = is_show_cutscene;
            Tag = Tags.HUD;
        }

        public override void OnBegin(Level level)
        {
            player = level.Tracker.GetEntity<Player>();
            if (Methods.PlayerIsAlive(player))
                Methods.PlayerLock(player);

            if (is_show_cutscene)
            {
                MTexture actual_texture = player.Sprite.Animations[player.Sprite.CurrentAnimationID].Frames[player.Sprite.CurrentAnimationFrame];
                player_texture = new MTexture(actual_texture, new Rectangle((actual_texture.Width - 20) / 2, actual_texture.Height - 20, 20, 20));

                hair_texture = GFX.Game["Evidence02/objects_melihelper/warpzone/playerhair"];
                //player_texture = GFX.Game["Evidence02/objects_melihelper/warpzone/player"];
                pcenter_angle = 3 * MathExt.PI2 / 16;
                pcenter_dist = 630;
                player_scale = 16;
                player_rotation = 7 * MathExt.PI2 / 4;
                player_pos = new Vector2(920, 540) + Calc.AngleToVector(pcenter_angle, pcenter_dist);

                mass_hair_hodes = new Vector2[Math.Max(1, player.Hair.Nodes.Count)];
                for (int i = 0; i < mass_hair_hodes.Length; i++)
                    mass_hair_hodes[i] = player.Center;

                mass_textures = new WarpZoneTexture[5];
                for (int i = 0; i < mass_textures.Length; i++)
                    mass_textures[i] = new WarpZoneTexture(GFX.Gui["Evidence02/bc/warpZone" + texture_type],
                        texture_color,
                        new Vector2(920, 540) + Calc.AngleToVector((5 + i) * MathExt.PI2 / 8, 40),
                        8f - 1.1f * i,
                        MathExt.DegreesToRadians * 40 / (8f - 1.1f * i));

                Audio.SetMusic(null);
            }

            Add(new Coroutine(Cutscene(level)));
        }

        private IEnumerator Cutscene(Level level)
        {
            // Stun her!
            player.StateMachine.State = Player.StDummy;
            player.Hair.Visible = false;

            // suck Canadian into the warp zone
            float delay_max = 1.2f;
            float delay = delay_max;
            while (delay > 0)
            {
                player.Speed = Vector2.Zero;
                player.Position += 0.2f * (warp_zone.Center - player.Center);
                player_sprite_rotation_del += 0.8f * Engine.DeltaTime;
                player_sprite_rotation += player_sprite_rotation_del;
                player.Sprite.Rotation = player_sprite_rotation;
                //player.Sprite.Origin = new Vector2(20, 20);

                //player.Sprite.Origin = new Vector2(0.5f, 0.75f);
                //player.Sprite.Origin = new Vector2(player.Sprite.Width / 2, 3 * player.Sprite.Height / 4);

                //player.Sprite.Color = Color.White * (delay / delay_max);
                //player.Hair.Color = Color.White * (delay / delay_max);

                delay -= Engine.DeltaTime;
                yield return null;
            }
            player.Visible = false;

            // Black screen
            while (alpha_back < 1)
            {
                alpha_back += Engine.DeltaTime / 0.3f;
                yield return null;
            }

            if (is_show_cutscene)
            {
                // Delay
                yield return 0.2f;

                // Warpzone appears
                if (sound != "") Audio.Play(sound);
                while (alpha_hole < 1)
                {
                    alpha_hole += Engine.DeltaTime / 0.6f;
                    yield return null;
                }

                // Silly goober got warped
                is_update_canadian = true;
                yield return 3.5f;

                // Black screen disappears
                while (alpha_hole > 0)
                {
                    alpha_hole -= Engine.DeltaTime / 0.4f;
                    yield return null;
                }
            }


            // cutscene end
            EndCutscene(level);
        }

        public override void Update()
        {
            base.Update();
            if (!is_show_cutscene)
                return;

            player.Sprite.Origin = new Vector2(20, 20);
            if (alpha_back > 0)
            {
                foreach (var item in mass_textures)
                    item.Update();
            }

            if (is_update_canadian)
            {
                // 5 seconds to update!
                pcenter_angle += 0.8f * MathExt.PI2 * Engine.DeltaTime;
                pcenter_dist *= 0.988f;
                player_scale *= 0.988f;
                player_rotation += 2.4f * MathExt.PI2 * Engine.DeltaTime;
                player_pos = new Vector2(920, 540) + Calc.AngleToVector(pcenter_angle, pcenter_dist);
                
                // It's called hair, Strax...
                mass_hair_hodes[0] = player_pos + Calc.AngleToVector(player_rotation + 268 * MathExt.DegreesToRadians, player_scale * 3.6f);
                for (int i = 1; i < mass_hair_hodes.Length; i++)
                    mass_hair_hodes[i] = mass_hair_hodes[i - 1]
                        + (hair_texture.Width * player_scale * GetHairScale(i) / 32) * Vector2.Normalize(mass_hair_hodes[i] - mass_hair_hodes[i-1]);
            }
        }

        public override void Render()
        {
            base.Render();
            if (alpha_back > 0)
                Draw.Rect(new Rectangle(-2, -2, 1924, 1084), Color.Black * alpha_back);

            if (alpha_hole > 0)
                foreach (var item in mass_textures)
                    item.Render(alpha_hole);

            if (is_update_canadian)
            {
                for (int i = 0; i < mass_hair_hodes.Length; i++)                    // Black border
                    hair_texture.DrawCentered(mass_hair_hodes[i], Color.Black,
                        scale: 1.2f * player_scale * GetHairScale(i) / 16);
                for (int i = 0; i < mass_hair_hodes.Length; i++)
                    hair_texture.DrawCentered(mass_hair_hodes[i], player.Hair.GetHairColor(i),
                        scale: player_scale * GetHairScale(i) / 16);
                player_texture.DrawCentered(player_pos, Color.White, scale: player_scale, rotation: player_rotation);
            }
        }

        float GetHairScale(int i)
        {
            return Math.Max(1f - 0.8f * (i + 1) / mass_hair_hodes.Length, 0.4f);
        }

        public override void OnEnd(Level level)
        {
            //Methods.PlayerLock(player, false);
            level.OnEndOfFrame += (Action)(() => {
                level.Remove(player);
                level.UnloadLevel();
                level.Session.Dreaming = false;
                level.Session.Level = room_teleport;

                //Leader.RestoreStrawberries(player.Leader);

                level.Session.RespawnPoint = level.GetSpawnPoint(room_spawnpoint);

                // Unlock player moves
                player.StateMachine.Locked = false;
                player.StateMachine.State = 0;
                player.ForceCameraUpdate = false;

                level.LoadLevel(intro_type);
                //Leader.RestoreStrawberries(level.Tracker.GetEntity<Player>().Leader);
            });
        }

        class WarpZoneTexture
        {
            MTexture texture;
            Color color;
            Vector2 center;
            float size, angle, angle_del;

            public WarpZoneTexture(MTexture texture, Color color, Vector2 center, float size, float angle_del)
            {
                this.texture = texture;
                this.color = color;
                this.center = center;
                this.size = size;
                this.angle_del = angle_del;
            }

            public void Update()
            {
                angle += angle_del;
            }

            public void Render(float alpha)
            {
                texture.DrawCentered(center, color * alpha, scale: size, rotation: angle);
            }
        }
    }
}
