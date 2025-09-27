using Celeste.Mod.MeliHelper._BattleCity._Bonuses;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    class BCCutsceneWin : CutsceneEntity
    {
        const int XPTS = 700;
        const float Y1 = 72;
        const float DY = 68;
        const float DD = 108;

        Field field;
        Player player;
        Dictionary<BCEnum_EnemyType, BCCutsceneWinStat> mass_stats;
        string current_level, next_level, hiscore_message;
        int total_points, total_enemies, state_show;
        float label_gameover_dy;
        bool is_show_screen, is_show_gameover_bricks, is_game_over;

        public BCCutsceneWin(Field field, Player player, string current_level, string next_level, bool is_game_over = false)
        {
            this.field = field;
            this.player = player;
            this.current_level = current_level;
            this.next_level = next_level;
            this.is_game_over = is_game_over;
            this.label_gameover_dy = 1120;
            //this.font = FontController.GetBCFont();
            //Depth = DepthController.BC_FIELD_BACKGROUND_INTRO;
            Tag = Tags.HUD;

            mass_stats = new Dictionary<BCEnum_EnemyType, BCCutsceneWinStat>();
            int sprite_id = 0;
            foreach (BCEnum_EnemyType type in Enum.GetValues(typeof(BCEnum_EnemyType)))
                mass_stats[type] = new BCCutsceneWinStat() { sprite_id = sprite_id++ };
        }

        public override void OnBegin(Level level)
        {
            Methods.PlayerLock(player);
            Add(new Coroutine(Cutscene(level)));
            if (!is_game_over) ProgressController.SaveProgress();
            
            // Total scoring
            total_enemies = 0;
            total_points = 0;
            foreach (BCEnum_EnemyType type in Enum.GetValues(typeof(BCEnum_EnemyType)))
            {
                List<int> list_points = field.GetEnemiesComponent.GetListCollectedPoints(type);
                total_points += list_points.Sum();
                total_enemies += list_points.Count;
            }

            // Hiscore
            string campaign = ProgressController.CampaignName();
            hiscore_message =
                (MeliHelperModule.Instance.SaveData.BattleCity_HiScores is null) ? "ERROR#BRUH-1" :
                (campaign is null) ? "ERROR--CAMPAIGN-IS-NULL" :
                (campaign == "") ? "ERROR--CANT-FIND-CAMPAIGN" :
                (!MeliHelperModule.Instance.SaveData.BattleCity_HiScores.ContainsKey(campaign)) ? "ERROR#BRUH-2" :
                MeliHelperModule.Instance.SaveData.BattleCity_HiScores[campaign].ToString();

            // Clear field
            foreach (var item in level.Entities.FindAll<StarHUD>()) item.RemoveSelf();
            foreach (var item in level.Entities.FindAll<Shield>()) item.RemoveSelf();
            foreach (var item in level.Entities.FindAll<Item>()) item.RemoveSelf();
            if (is_game_over)
                Audio.SetMusic(null);
        }

        private IEnumerator Cutscene(Level level)
        {
            if (is_game_over)
            {
                //Audio.Play(SoundController.BC_GAMEOVER);
                yield return 1.6f;

                while (label_gameover_dy > 540)
                {
                    label_gameover_dy -= 180 * Engine.DeltaTime;
                    yield return null;
                }

                yield return 2.4f;
            }


            yield return 1.6f;
            is_show_screen = true;

            // Params appears
            for (int i = 1; i <= 7; i++)
            {
                yield return 0.04f;
                state_show++;
            }



            // 2. Scoring
            foreach (BCEnum_EnemyType type in Enum.GetValues(typeof(BCEnum_EnemyType)))
            {
                BCCutsceneWinStat stat = mass_stats[type];
                stat.show = true;
                yield return 0.1f;

                List<int> list_points = field.GetEnemiesComponent.GetListCollectedPoints(type);
                foreach (int points in list_points)
                {
                    Audio.Play(SoundController.BC_SCORING_TANK);
                    stat.count_enemies++;
                    stat.points += points;
                    yield return 0.16f;
                }
                yield return 0.3f;
            }
            state_show = 8;
            yield return 0.8f;


            if (is_game_over)
            {
                yield return 1.2f;
                state_show = 0;
                foreach (BCEnum_EnemyType type in mass_stats.Keys)
                    mass_stats[type].show = false;

                yield return 0.3f;
                is_show_gameover_bricks = true;
                Audio.Play(SoundController.BC_GAMEOVER);
                yield return 3f;
                is_show_gameover_bricks = false;
                yield return 0.2f;
            }


            // cutscene end
            EndCutscene(level);
        }

        public override void Render()
        {
            base.Render();
            if (is_game_over && !is_show_screen)
                FontController.ShowTextNES("GAME OVER", new Vector2(960, label_gameover_dy), Color.Red, TextAlignment.Center);
            if (is_show_screen) Draw.Rect(-2, -2, 1924, 1084, Color.Black);

            float y = Y1;

            // HI-SCORE, STAGE
            if (state_show >= 1)
            {
                // 960 + 40
                FontController.ShowTextNES("HI-SCORE", new Vector2(920, y), Color.Red, TextAlignment.Right);
                FontController.ShowTextNES(hiscore_message, new Vector2(1000, y), Color.Orange, TextAlignment.Left);
                y += DY;
            }

            if (state_show >= 2) FontController.ShowTextNES(current_level, new Vector2(960, y), Color.White, TextAlignment.Center);
            y += DY;
            if (state_show >= 3) FontController.ShowTextNES("I-PLAYER", new Vector2(XPTS, y), Color.Red, TextAlignment.Right);
            y += DY;
            if (state_show >= 3) FontController.ShowTextNES(total_points.ToString(), new Vector2(XPTS, y), Color.Orange, TextAlignment.Right);
            y += DD;


            foreach (BCEnum_EnemyType type in mass_stats.Keys)
            {
                if (mass_stats[type].show)
                    FontController.ShowTextNES(mass_stats[type].points.ToString(), new Vector2(565, y), Color.White, TextAlignment.Right);
                if (state_show >= 5)
                    FontController.ShowTextNES("PTS", new Vector2(XPTS, y), Color.White, TextAlignment.Right);
                if (mass_stats[type].show)
                    FontController.ShowTextNES(mass_stats[type].count_enemies.ToString(), new Vector2(880, y), Color.White, TextAlignment.Right);
                if (state_show >= 4)
                    FontController.ShowTextNES("<", new Vector2(920, y), Color.White, TextAlignment.Right);
                if (state_show >= 6)
                    TextureController.GetEnemyTextureUI(type).DrawCentered(new Vector2(960, y + 14), Color.White);
                y += DD;
            }

            y += 1.2f * DY - DD;
            if (state_show >= 7)
            {
                Draw.Line(new Vector2(XPTS + 10, y - 15), new Vector2(1920 - XPTS - 10, y - 15), Color.White, 4);
                FontController.ShowTextNES("TOTAL", new Vector2(XPTS, y), Color.White, TextAlignment.Right);
            }
            
            if (state_show >= 8)
                FontController.ShowTextNES(total_enemies.ToString(), new Vector2(880, y), Color.White, TextAlignment.Right);

            if (is_show_gameover_bricks)
            {
                // GAME
                string[] mass_bricks = {
                    "0011111-0011100-1100011-1111111",
                    "0110000-0110110-1110111-1100000",
                    "1100000-1100011-1111111-1100000",
                    "1100111-1100011-1111111-1111100",
                    "1100011-1111111-1101011-1100000",
                    "0110011-1100011-1100011-1100000",
                    "0011111-1100011-1100011-1111111"
                };
                for (int j = 0; j < mass_bricks.Length; j++)
                    for (int i = 0; i < mass_bricks[j].Length; i++)
                        if (mass_bricks[j][i] == '1')
                        {
                            MTexture texture = TextureController.GetBrickTile((i + j) % 2 == 0);
                            texture.DrawCentered(new Vector2(665 + 5 * i * texture.Width, 340 + 5f * j * texture.Height), Color.White, 5f);
                        }

                // OVER
                mass_bricks = new string[] {
                    "0111110-1100011-1111111-1111110",
                    "1100011-1100011-1100000-1100011",
                    "1100011-1100011-1100000-1100011",
                    "1100011-1110111-1111100-1100111",
                    "1100011-0111110-1100000-1111100",
                    "1100011-0011100-1100000-1101110",
                    "0111110-0001000-1111111-1100111"
                };
                for (int j = 0; j < mass_bricks.Length; j++)
                    for (int i = 0; i < mass_bricks[j].Length; i++)
                        if (mass_bricks[j][i] == '1')
                        {
                            MTexture texture = TextureController.GetBrickTile((i + j) % 2 == 0);
                            texture.DrawCentered(new Vector2(665 + 5f * i * texture.Width, 540 + 5f * j * texture.Height), Color.White, 5f);
                        }
            }
        }


        public override void OnEnd(Level level)
        {
            if (is_game_over)
            {
                MeliHelperModule.Instance.Session.BattleCity_StartedLevelsID.Clear();
                MeliHelperModule.Instance.Session.BattleCity_PlayerInfo.BruhGameover();
                MeliHelperModule.Instance.Session.BattleCity_PlayerInfoSaved.BruhGameover();
            }

            //Methods.PlayerLock(player, false);
            level.OnEndOfFrame += (Action)(() => {
                level.Remove(player);
                level.UnloadLevel();
                level.Session.Dreaming = false;
                level.Session.Level = next_level;

                //Leader.RestoreStrawberries(player.Leader);

                //There's only 1 spawnpoint on every level anyway
                //level.Session.RespawnPoint = level.GetSpawnPoint(new Vector2(level.Bounds.Left, level.Bounds.Top));
                level.Session.RespawnPoint = level.GetSpawnPoint(Vector2.Zero);

                // Unlock player moves
                player.StateMachine.Locked = false;
                player.StateMachine.State = 0;
                player.ForceCameraUpdate = false;

                level.LoadLevel(is_game_over ? Player.IntroTypes.Respawn : Player.IntroTypes.None);
                //Leader.RestoreStrawberries(level.Tracker.GetEntity<Player>().Leader);
            });
        }

        class BCCutsceneWinStat
        {
            public int sprite_id, points, count_enemies;
            public bool show;
        }
    }
}
