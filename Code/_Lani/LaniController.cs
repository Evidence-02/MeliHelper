using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._Lani
{
    class LaniController
    {
        private static FieldInfo playerLastAim = typeof(Player).GetField("lastAim", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo playerCalledDashEvents = typeof(Player).GetField("calledDashEvents", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo playerBeforeDashSpeed = typeof(Player).GetField("beforeDashSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo playerLastDashes = typeof(Player).GetField("lastDashes", BindingFlags.NonPublic | BindingFlags.Instance);
        static bool is_loaded;

        private static int dashCountBeforeDash;
        private static Vector2 dashDirectionBeforeDash;


        public static void Load()
        {
            On.Celeste.Level.LoadLevel += onLoadLevel;
        }

        public static void onLoadLevel(On.Celeste.Level.orig_LoadLevel orig, Level self, Player.IntroTypes playerIntro, bool isFromLoader)
        {
            orig(self, playerIntro, isFromLoader);
            if (MeliHelperModule.Instance.Session.LaniHook_Params != null && !is_loaded)
            {
                is_loaded = true;
                On.Celeste.Player.StartDash += onStartDash;
                On.Celeste.Player.BoostEnd += onBoostEnd;
                On.Celeste.Player.DashCoroutine += onDashCoroutine;
            }
        }



        public static LaniHookParams GetHookParamsFromData(EntityData data)
        {
            LaniHookParams hook_params = new LaniHookParams();
            hook_params.Direction = data.Attr("hookDirection", "Horizontal");
            hook_params.Length = data.Float("hookLength", 120);
            hook_params.Speed = data.Int("hookSpeed", 300);
            hook_params.Cooldown = data.Float("hookCooldown", 0f);
            hook_params.Color = Methods.GetColorFromString(data.Attr("hookColor")) * data.Float("hookOpacity", 1f);
            return hook_params;
        }

        public static void SetHook(LaniHookParams hook_params)
        {
            MeliHelperModule.Instance.Session.LaniHook_Params = hook_params;
            if (!is_loaded)
            {
                is_loaded = true;
                On.Celeste.Player.StartDash += onStartDash;
                On.Celeste.Player.BoostEnd += onBoostEnd;
                On.Celeste.Player.DashCoroutine += onDashCoroutine;
            }
        }

        public static void ClearHook()
        {
            MeliHelperModule.Instance.Session.LaniHook_Params = null;
            if (is_loaded)
            {
                is_loaded = false;
                On.Celeste.Player.StartDash -= onStartDash;
                On.Celeste.Player.BoostEnd -= onBoostEnd;
                On.Celeste.Player.DashCoroutine -= onDashCoroutine;
            }
        }


        static int onStartDash(On.Celeste.Player.orig_StartDash orig, Player self)
        {
            dashCountBeforeDash = self.Dashes;
            dashDirectionBeforeDash = (Vector2)playerLastAim.GetValue(self);
            return orig(self);
        }

        static void onBoostEnd(On.Celeste.Player.orig_BoostEnd orig, Player self)
        {
            if (self.StateMachine.State == Player.StDash || self.StateMachine.State == Player.StRedDash)
            {
                dashCountBeforeDash = self.Dashes;
                dashDirectionBeforeDash = (Vector2)playerLastAim.GetValue(self);
            }

            orig(self);
        }

        private static IEnumerator onDashCoroutine(On.Celeste.Player.orig_DashCoroutine orig, Player self)
        {
            // if inside of booster, just do vanilla coroutine and nothing else
            LaniHookParams hook_params = MeliHelperModule.Instance.Session.LaniHook_Params;
            if (self.CurrentBooster != null || hook_params == null)
            {
                yield return new SwapImmediately(orig(self));
                yield break;
            }
                        
            // Create a hook!
            // make a step forward
            if (orig(self).MoveNext())
                yield return orig(self).Current;

            Level level = self.SceneAs<Level>();
            if (hook_params.Cooldown == 0 || !level.Entities.FindAll<LaniHook>().Exists(t => t.GetCooldown > 0))
            {
                Vector2 playerAim = (Vector2)playerLastAim.GetValue(self);
                if (self.Ducking && self.CanUnDuck)
                    self.Ducking = false;

                Vector2 direction =
                    (self.OverrideDashDirection.HasValue) ? self.OverrideDashDirection.Value :
                    (hook_params.Direction == "Horizontal") ? new Vector2(self.Facing == Facings.Left ? -1 : 1, 0) :
                    playerAim;
                level.Add(new LaniHook(self, direction, hook_params: hook_params));
            }

            self.Dashes = 0;
            CancelPlayerDash(self, 1);
        }

        static void CancelPlayerDash(Player player, int count_dashes = -1)
        {
            // prevent DashEnd from triggering dash events (no dash sound)
            playerCalledDashEvents.SetValue(player, true);
            
            // restore pre-dash speed
            player.Speed = (Vector2)playerBeforeDashSpeed.GetValue(player);
            
            // restore pre-dash dash count
            if (count_dashes > 0)
                player.Dashes = count_dashes;
            
            // prevent the hair from flashing
            playerLastDashes.SetValue(player, player.Dashes);

            // kick the player back to the normal state
            player.StateMachine.State = 0;
        }
    }
}
