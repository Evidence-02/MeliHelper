using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    class BCController
    {
        private static FieldInfo playerLastAim = typeof(Player).GetField("lastAim", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo playerCalledDashEvents = typeof(Player).GetField("calledDashEvents", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo playerBeforeDashSpeed = typeof(Player).GetField("beforeDashSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo playerLastDashes = typeof(Player).GetField("lastDashes", BindingFlags.NonPublic | BindingFlags.Instance);
        static bool is_hooks_loaded;
        static bool is_shoot_pressed_old;
        
        public static void Load()
        {
            On.Celeste.Level.LoadLevel += onLevelLoad;
            On.Celeste.Player.Update += onPlayerUpdate;
            On.Celeste.Player.Die += onPlayerDie;
            On.Celeste.Strawberry.CollectRoutine += onStrawberryCollect;


        }

        public static void Unload()
        {
            On.Celeste.Level.LoadLevel -= onLevelLoad;
            On.Celeste.Player.Update -= onPlayerUpdate;
            On.Celeste.Player.Die -= onPlayerDie;
            On.Celeste.Strawberry.CollectRoutine -= onStrawberryCollect;
        }

        public static void SetHooksLoaded(bool value = true)
        {
            is_hooks_loaded = value;
        }

        private static void onLevelLoad(On.Celeste.Level.orig_LoadLevel orig, Level self, Player.IntroTypes playerIntro, bool isFromLoader)
        {
            SetHooksLoaded(false);
            orig(self, playerIntro, isFromLoader);
        }

        private static void onPlayerUpdate(On.Celeste.Player.orig_Update orig, Player self)
        {
            orig(self);
            if (!is_hooks_loaded)
                return;

            if (MeliHelperModule.Settings.BattleCity_Shoot.Pressed && !is_shoot_pressed_old && Methods.PlayerCanMove(self))
            {
                Field field = Field.Instance;
                if (field != null && field.GetItemComponent.TryDisconnect())
                {
                    // Shoot custom item
                }
                else if (MeliHelperModule.Instance.Session.BattleCity_CustomRules != null && MeliHelperModule.Instance.Session.BattleCity_CustomRules.PlayerCustomShooting)
                    PlayerShoot(self, 
                        MeliHelperModule.Instance.Session.BattleCity_CustomRules.PlayerShotSpeed, 
                        MeliHelperModule.Instance.Session.BattleCity_CustomRules.PlayerShotsAtOnce,
                        MeliHelperModule.Instance.Session.BattleCity_CustomRules.PlayerCanDestroySteel
                        );
                else
                {
                    int power = ProgressController.GetPlayerPower();
                    int max_count_bullets = (power >= 5) ? 999 : (power >= 4) ? 3 : (power >= 2) ? 2 : 1;
                    float speed = (power >= 4) ? 300 : (power >= 1) ? 240 : 180;
                    PlayerShoot(self, speed, max_count_bullets, power >= 3);
                }
            }
            is_shoot_pressed_old = MeliHelperModule.Settings.BattleCity_Shoot.Pressed;
        }

        static void PlayerShoot(Player player, float speed, int count_bullets_at_once, bool is_can_destroy_steel)
        {
            int count_bullets = player.Scene.Entities.FindAll<Bullet>().FindAll(t => t.GetParent == player && !t.isShadowBullet).Count;
            if (count_bullets >= count_bullets_at_once
                && !MeliHelperModule.Settings.Debug_UnlimitedShooting
                && !Field.Instance.GetEventUI.isEventExists(BCEnum_BonusEvent.UnlimitedShooting))
                return;

            int dy = (Input.MenuUp.Check ? -1 : 0);
            int dx = (dy != 0) ? 0 : (player.Facing == Facings.Left) ? -1 : 1;
            Vector2 dir = new Vector2(dx, dy);

            // Locate bullet
            Vector2 center = player.Center + 4 * dir;
            if (dir.Y == 0)
                center.Y -= 2;

            Field field = Field.Instance;
            if (field != null)
            {
                int tx = 0, ty = 0, counter = 3;
                center += 4 * dir;
                do
                {
                    center -= 4 * dir;
                    tx = field.GetTileCX(center);
                    ty = field.GetTileCY(center);
                }
                while (field.isInField(tx, ty) && field.isActualSolid(tx, ty) && --counter >= 0);
            }


            Audio.Play(SoundController.BC_PLAYER_TANK_FIRING);
            if (Field.Instance != null && Field.Instance.GetEventUI.isEventExists(BCEnum_BonusEvent.Duality))
            {
                Vector2 perp = (dir.X != 0) ? new Vector2(0, 1) : new Vector2(1, 0);
                player.Scene.Add(new Bullet(player, center - 2 * perp, speed * dir, Color.OrangeRed, can_break_steel: is_can_destroy_steel, is_player_bullet: true));
                player.Scene.Add(new Bullet(player, center + 2 * perp, speed * dir, Color.Violet, can_break_steel: is_can_destroy_steel, is_player_bullet: true, is_shadow_bullet: true));
            }
            else
                player.Scene.Add(new Bullet(player, center, speed * dir, Color.White, can_break_steel: is_can_destroy_steel, is_player_bullet: true));

            //delay_shoot = 0.6f - 0.1f * power;
        }
        
        static PlayerDeadBody onPlayerDie(On.Celeste.Player.orig_Die orig, Player self, Vector2 direction, bool evenIfInvincible, bool registerDeathInStats)
        {
            if (!evenIfInvincible 
                && is_hooks_loaded
                && !(MeliHelperModule.Instance.Session.BattleCity_CustomRules != null && MeliHelperModule.Instance.Session.BattleCity_CustomRules.isVanillaDeaths)
                //&& !(MeliHelperModule.Instance.Session.BattleCity_CustomRules?.isVanillaDeaths)
                ) // press on Retry button or something
            {
                // Shield or assist mode
                if (Field.Instance != null && Field.Instance.GetEventUI.isEventExists(BCEnum_BonusEvent.Shield) || SaveData.Instance.Assists.Invincible)
                {
                    self.SceneAs<Level>().Displacement.AddBurst(self.Center, 0.4f, 8, 64, 0.5f, Ease.QuadOut, Ease.QuadOut);
                    return null;
                }


                //Field field = Field.Instance;
                //if (field != null) field.GetEventUI.SetInfo("i'm ded");

                if (Field.Instance != null && Field.Instance.GetGameState != BCEnum_GameState.Gameover)
                {
                    ProgressController.GetCurrentPlayerInfo().PlayerFakeDeath();
                    if (Field.Instance != null)
                        Field.Instance.GetItemComponent.Clear();
                    self.Add(new Coroutine(ImitateDeath(self)));
                }
                return null;
            }
            
            return orig(self, direction, evenIfInvincible, registerDeathInStats);
        }

        static IEnumerator ImitateDeath(Player player)
        {
            Level level = player.SceneAs<Level>();
            Methods.PlayerLock(player);

            // Show 
            Entity entity_temp = new Entity();
            entity_temp.Position = player.Center;
            level.Add(entity_temp);


            Audio.Play(SFX.char_mad_death);
            player.Sprite.Visible = false;
            player.Hair.Visible = false;
            player.Add(new DeathEffect(player.Hair.Color));
            if (Field.Instance != null)
                Field.Instance.GetEventUI.Clear();

            // Oops, you're dead!
            if (ProgressController.GetCurrentPlayerInfo().Lifes < 0)
            {
                Field.Instance.SetState(BCEnum_GameState.Gameover);
                yield break;
            }

            // Add temporal invincibility
            bool set_invincibility = SaveData.Instance.Assists.Invincible;
            if (!set_invincibility)
                SaveData.Instance.Assists.Invincible = true;

            Vector2 pos_saved = player.Center;
            float delay = 1.6f;
            while (delay > 0)
            {
                player.Center = pos_saved;
                player.Speed = Vector2.Zero;
                delay -= Engine.DeltaTime;
                yield return null;
            }
            //Vector2 safe_position = Vector2.Zero;
            //Field field = level.Entities.FindFirst<Field>();
            //if (field != null) safe_position = field.Center + new Vector2(16, -32); // topleft point
            //                                                                        // point of the field, actually
            //                                                 // I needed the point inside of the camera triggers but not inside of the actual field
            //player.Position = safe_position;
            //player.Position = Vector2.Zero;
            //yield return 1f;
            
            
            // Spawn player on spawnpoint
            player.Facing = Facings.Right;
            player.Position = level.DefaultSpawnPoint;
            player.Sprite.Visible = true;
            player.Hair.Visible = true;
            Methods.PlayerLock(player, false);

            // Kill all the dangerous thing near the spawnpoint
            foreach (Enemy tank in level.Entities.FindAll<Enemy>().FindAll(t => Vector2.Distance(t.Center, player.Center) <= 24))
                tank.Die(true);

            // Imitate respawn
            Audio.Play(SFX.char_mad_revive);
            player.StateMachine.State = Player.StIntroRespawn;
            player.JustRespawned = true;
            player.IntroType = Player.IntroTypes.Transition;
            player.Hair.Facing = player.Facing;
            player.Hair.Start();
            player.UpdateHair(true);
            if (!set_invincibility)
                SaveData.Instance.Assists.Invincible = false;

        }
        
        private static IEnumerator onStrawberryCollect(On.Celeste.Strawberry.orig_CollectRoutine orig, Strawberry self, int collectIndex)
        {
            yield return new SwapImmediately(orig(self, collectIndex));
            //yield return orig(self, collectIndex);
            if (is_hooks_loaded && Field.Instance != null)
                Field.Instance.CheckFinish(BCEnum_Goal.CollectStorby);
        }

        /*
        public static string GetSolidRegistration(Solid solid)
        {
            return MeliHelperModule.Instance.Session.RegisteredSolid.ContainsKey(solid) ?
                MeliHelperModule.Instance.Session.RegisteredSolid[solid] : "";
        }
        */





    }
}
