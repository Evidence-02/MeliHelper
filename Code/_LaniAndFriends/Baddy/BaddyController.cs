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

namespace Celeste.Mod.MeliHelper._Baddy
{
    class BaddyController
    {
        private static FieldInfo playerLastAim = typeof(Player).GetField("lastAim", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo playerCalledDashEvents = typeof(Player).GetField("calledDashEvents", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo playerBeforeDashSpeed = typeof(Player).GetField("beforeDashSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo playerLastDashes = typeof(Player).GetField("lastDashes", BindingFlags.NonPublic | BindingFlags.Instance);

        // TODO:
        // trigger or entity "add max power"
        static BaddyPowerUI hud;
        static bool is_loaded, is_switch_button_pressed_old, is_temporary_stop_restoring_power;
        static float previous_shot_time, saved_power_after_boost;
        static int old_player_state;
        
        public static void Load()
        {
            On.Celeste.Level.LoadLevel += onLoadLevel_LoadFromSession;
        }

        public static void Unload()
        {
            On.Celeste.Level.LoadLevel -= onLoadLevel_LoadFromSession;
        }

        public static void onLoadLevel_LoadFromSession(On.Celeste.Level.orig_LoadLevel orig, Level self, Player.IntroTypes playerIntro, bool isFromLoader)
        {
            orig(self, playerIntro, isFromLoader);
            if (MeliHelperModule.Instance.Session.BadelinePower_Params != null && !is_loaded)
                SetPower(self, MeliHelperModule.Instance.Session.BadelinePower_Params);
        }

        public static BadelinePowerParams GetHookParamsFromData(EntityData data)
        {
            BadelinePowerParams baddy_params = new BadelinePowerParams();
            baddy_params.uiTexture = data.Attr("uiTexture", "");
            baddy_params.uiLocation = data.Attr("uiLocation", "BottomLeft");
            baddy_params.FullPower = data.Float("fullPower", 2f);
            baddy_params.ShootPower = data.Float("shootPower", 1f);
            baddy_params.LaserPower = data.Float("laserPower", 2f);
            baddy_params.BoostPower = data.Float("boostPower", 1.5f);
            baddy_params.AddMaxPowerOnStrawberryCollect = data.Float("addMaxPowerOnStrawberryCollect", 0.1f);

            
            baddy_params.isShootEnabled = data.Bool("shootEnabled", true);
            baddy_params.isLaserEnabled = data.Bool("laserEnabled", true);
            baddy_params.isBoostEnabled = data.Bool("boostEnabled", true);
            baddy_params.isCurrentWeaponShoot = (baddy_params.isShootEnabled || baddy_params.isBoostEnabled);
            baddy_params.isAffectPlayerSkin = data.Bool("affectPlayerSkin", true);
            baddy_params.isShowUI = data.Bool("showUI", true);
            return baddy_params;
        }

        public static void SetPower(Level level, BadelinePowerParams _params)
        {
            MeliHelperModule.Instance.Session.BadelinePower_Params = _params;
            _params.RestorePower();

            if (!is_loaded)
            {
                is_loaded = true;
                if (_params.isAffectPlayerSkin)
                    SetSkin(true);
                if (_params.isShowUI && (hud is null || !level.Contains(hud)))
                    level.Add(hud = new BaddyPowerUI());

                On.Celeste.Level.LoadLevel += onLoadLevel;
                On.Celeste.Player.Update += onPlayerUpdate;
                On.Celeste.Player.DashCoroutine += onDashCoroutine;
                On.Celeste.Player.StartDash += onStartDash;
                On.Celeste.Player.RefillDash += onRefillDash;
                On.Celeste.Player.BadelineBoostLaunch += onBadelineBoostLaunch;
                On.Celeste.Refill.OnPlayer += onPlayerRefill;
                On.Celeste.Strawberry.OnCollect += onCollectBerry;
                On.Celeste.SummitGem.SmashRoutine += onSummitGemSmashRoutine;
                On.Celeste.Lookout.Interact += onWatchtowerStart;
                On.Celeste.Lookout.StopInteracting += onWatchtowerStop;
            }
        }

        public static void ClearPower()
        {
            bool is_affect_player_skin = MeliHelperModule.Instance.Session.BadelinePower_Params.isAffectPlayerSkin;
            MeliHelperModule.Instance.Session.BadelinePower_Params = null;
            if (is_loaded)
            {
                is_loaded = false;
                if (is_affect_player_skin)
                    SetSkin(false);
                On.Celeste.Level.LoadLevel -= onLoadLevel;
                On.Celeste.Player.Update -= onPlayerUpdate;
                On.Celeste.Player.DashCoroutine -= onDashCoroutine;
                On.Celeste.Player.StartDash -= onStartDash;
                On.Celeste.Player.RefillDash -= onRefillDash;
                On.Celeste.Player.BadelineBoostLaunch -= onBadelineBoostLaunch;
                On.Celeste.Refill.OnPlayer -= onPlayerRefill;
                On.Celeste.Strawberry.OnCollect -= onCollectBerry;
                On.Celeste.SummitGem.SmashRoutine -= onSummitGemSmashRoutine;
                On.Celeste.Lookout.Interact -= onWatchtowerStart;
                On.Celeste.Lookout.StopInteracting -= onWatchtowerStop;
            }
        }

        static bool isActuallyLoaded()
        {
            return is_loaded && MeliHelperModule.Instance.Session.BadelinePower_Params != null;
        }

        public static BadelinePowerParams GetParams()
        {
            return MeliHelperModule.Instance.Session.BadelinePower_Params;
        }

        public static BaddyPowerUI GetHUD()
        {
            return hud;
        }

        public static void SetSkin(bool is_badeline)
        {
            SaveData.Instance.Assists.PlayAsBadeline = is_badeline;
            Player entity = Engine.Scene.Tracker.GetEntity<Player>();
            if (entity != null)
            {
                PlayerSpriteMode mode = is_badeline ? PlayerSpriteMode.MadelineAsBadeline : entity.DefaultSpriteMode;
                if (entity.Active) entity.ResetSpriteNextFrame(mode);
                else entity.ResetSprite(mode);
            }
        }







        private static void onLoadLevel(On.Celeste.Level.orig_LoadLevel orig, Level self, Player.IntroTypes playerIntro, bool isFromLoader)
        {
            orig(self, playerIntro, isFromLoader);
            if (isActuallyLoaded())
            {
                if (GetParams().isShowUI)
                    self.Add(hud = new BaddyPowerUI());
                GetParams().RestorePower();
            }
        }

        private static void onPlayerUpdate(On.Celeste.Player.orig_Update orig, Player self)
        {
            orig(self);
            previous_shot_time += Engine.DeltaTime;
            if (isActuallyLoaded() && GetParams().isShootEnabled && GetParams().isLaserEnabled)
            {
                if (!is_switch_button_pressed_old && MeliHelperModule.Settings.BadelinePower_Switch.Pressed)
                    GetParams().isCurrentWeaponShoot = !GetParams().isCurrentWeaponShoot;
                is_switch_button_pressed_old = MeliHelperModule.Settings.BadelinePower_Switch.Pressed;
            }
        }

        static void onPlayerRefill(On.Celeste.Refill.orig_OnPlayer orig, Refill self, Player player)
        {
            if (self.Collidable && isActuallyLoaded() && !is_temporary_stop_restoring_power)
            {
                BadelinePowerParams _params = MeliHelperModule.Instance.Session.BadelinePower_Params;
                DynData<Refill> data = new DynData<Refill>(self);
                bool twoDashes = data.Get<bool>("twoDashes");
                float max = _params.FullPower + (twoDashes ? 1 : 0);
                float power = _params.CurrentPower;
                float power_new = (self.SceneAs<Level>().CoreMode != Session.CoreModes.None) ? max :
                    Math.Max(player.MaxDashes, Math.Min(max, power + (twoDashes ? 2 : 1)));
                if (power < power_new)
                {
                    _params.CurrentPower = power_new;

                    // Set count dashes to 0 - restore everything in the vanilla orig(self, player);
                    player.Dashes = 0;
                }
            }

            orig(self, player);
        }

        static int onStartDash(On.Celeste.Player.orig_StartDash orig, Player self)
        {
            old_player_state = self.StateMachine.State;
            return orig(self);
        }

        private static bool onRefillDash(On.Celeste.Player.orig_RefillDash orig, Player self)
        {
            if (!is_temporary_stop_restoring_power)
                GetParams().RestorePower();
            return orig(self);
        }



        static void onBadelineBoostLaunch(On.Celeste.Player.orig_BadelineBoostLaunch orig, Player self, float atX)
        {
            // Boost restores dash (and Badeline power), so I need to downvote it again
            orig(self, atX);
            if (saved_power_after_boost >= 0)
            {
                GetParams().CurrentPower = saved_power_after_boost;
                saved_power_after_boost = -1;
            }
        }

        private static void onCollectBerry(On.Celeste.Strawberry.orig_OnCollect orig, Strawberry self)
        {
            orig(self);
            if (isActuallyLoaded() && GetParams().AddMaxPowerOnStrawberryCollect > 0)
            {
                GetParams().AddMaxPower(GetParams().AddMaxPowerOnStrawberryCollect);
                if (hud != null) hud.SetColorTemp(Color.Red);
            }
        }

        static IEnumerator onSummitGemSmashRoutine(On.Celeste.SummitGem.orig_SmashRoutine orig, SummitGem self, Player player, Level level)
        {
            if (isActuallyLoaded())
            {
                GetParams().AddMaxPower(0.2f);
                if (hud != null) hud.SetColorTemp(Color.RoyalBlue);
            }
            yield return new SwapImmediately(orig(self, player, level));
        }

        static void onWatchtowerStart(On.Celeste.Lookout.orig_Interact orig, Lookout self, Player player)
        {
            orig(self, player);
            if (hud != null) hud.Visible = false;
        }

        static void onWatchtowerStop(On.Celeste.Lookout.orig_StopInteracting orig, Lookout self)
        {
            orig(self);
            if (hud != null) hud.Visible = true;
        }

        private static IEnumerator onDashCoroutine(On.Celeste.Player.orig_DashCoroutine orig, Player self)
        {
            // if inside of booster, just do vanilla coroutine and nothing else
            BadelinePowerParams _params = GetParams();
            if (self.CurrentBooster != null || _params == null)
            {
                yield return new SwapImmediately(orig(self));
                yield break;
            }
            


            // make a step forward
            if (orig(self).MoveNext())
                yield return orig(self).Current;


            // get the dash general direction
            Level level = self.SceneAs<Level>();
            Vector2 direction = (self.OverrideDashDirection.HasValue) ?
                                 self.OverrideDashDirection.Value :
                                 (Vector2)playerLastAim.GetValue(self);

            // Cancel
            CancelPlayerDash(self);
            
            if (_params.isCurrentWeaponShoot)
            {
                // Shoot or Boost
                if (direction.Y == -1 && direction.X == 0 && _params.isBoostEnabled)
                {
                    if (!_params.TryUseWeapon("Boost"))
                        yield break;

                    is_temporary_stop_restoring_power = true;
                    saved_power_after_boost = _params.CurrentPower;
                    Vector2[] nodes = {
                            self.Center,
                            self.Center + new Vector2(0, 10000)
                        };
                    float timeInvincibility = 0.85f;
                    self.Add(new Coroutine(Methods.SpinnerShatterTempCoroutine(self, new Vector2(0, -16), 20, timeInvincibility)));
                    self.Add(new Coroutine(Methods.TrailGradientCoroutine(self, Color.DarkViolet, Color.Violet, timeInvincibility)));
                    level.Add(new BadelineBoost(nodes, false));
                    Methods.RegisterDash(level);
                    is_temporary_stop_restoring_power = false;
                }
                else
                {
                    if (!_params.TryUseWeapon("Shoot"))
                        yield break;

                    // Visuals
                    Audio.Play(SoundController.BADELINE_CLASSIC_SHOT);
                    Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
                    SlashFx.Burst(self.Center, direction.Angle());
                    self.Add(new Coroutine(Methods.TrailGradientCoroutine(self, Color.Red, Color.DarkViolet, 0.12f)));
                    level.Displacement.AddBurst(self.Center, .4f, 8, 64, .5f, Ease.QuadOut, Ease.QuadOut);

                    // Special functon for winged and golden winged strawberries
                    Methods.RegisterDash(level);

                    // Restore dash, but not restore power
                    if (_params.CurrentPower >= self.Dashes + 1)
                    {
                        is_temporary_stop_restoring_power = true;
                        self.RefillDash();
                        is_temporary_stop_restoring_power = false;
                    }


                    // (overpowered) Player speed after shot
                    bool playerAutoJump = true;
                    float speedX = -Math.Sign(direction.X) * (direction.Y == 0 ? 252 : 225);
                    float speedY = (Math.Sign(direction.Y) == 1) ? (direction.X == 0 ? -270 : -243) :
                        (direction.Y == 0) ? -150 :
                        30;

                    // Correct speed
                    Vector2 shot_center = self.Center + 10 * direction;
                    if (level.CollideCheck<Solid>(shot_center))
                    {
                        // Don't jump, if you shooting down and it's a special block that can throw you up
                        if (direction.Y > 0
                            && (   level.Entities.FindAll<ZipMover>().FirstOrDefault(t => t.CollidePoint(shot_center)) != null
                                || level.Entities.FindAll<SwapBlock>().FirstOrDefault(t => t.CollidePoint(shot_center)) != null
                                || level.Entities.FindAll<CrushBlock>().FirstOrDefault(t => t.CollidePoint(shot_center)) != null
                                ))
                        {
                            playerAutoJump = false;
                            speedX = 0;
                            speedY = 0;
                        }

                        // if player stands close to solid, change bullet spot point
                        shot_center -= 8 * direction;
                    }
                    if (self.Ducking)
                        shot_center.Y -= 8;

                    // Set speed down...
                    if (old_player_state == Player.StRedDash || old_player_state == Player.StStarFly)
                    {
                        // ...but still do Nyoooooom with boosters or feathers!
                        self.Speed.X *= 0.55f;
                        self.Speed.Y *= 0.55f;
                    }
                    else
                    {
                        bool itsFuckingWindyX = (level.Wind.X != 0);
                        bool itsFuckingWindyY = (level.Wind.Y != 0 && Math.Sign(level.Wind.Y) != Math.Sign(speedY));
                        float baseKoefX = (itsFuckingWindyX) ? 0.6f : 0.3f;
                        float baseKoefY = (itsFuckingWindyY) ? 0.6f : 0.3f;
                        float koef =
                            (previous_shot_time < 0.2f) ? 1f :
                            (previous_shot_time > 0.8f) ? 0f :
                            (1 - (previous_shot_time - 0.2f) / 0.6f);
                        if (koef > 0)
                        {
                            baseKoefX += koef * (0.66f - baseKoefX);
                            baseKoefY += koef * (0.66f - baseKoefY);
                        }

                        self.Speed.X *= baseKoefX;
                        self.Speed.Y *= baseKoefY;
                        speedX *= 0.9f;
                        if (!itsFuckingWindyY) speedY *= 0.9f;
                    }

                    if (Math.Sign(self.Speed.X) != Math.Sign(speedX)) self.Speed.X = 0;
                    if (Math.Sign(self.Speed.Y) != Math.Sign(speedY)) self.Speed.Y = 0;

                    self.Speed.X += speedX;
                    self.Speed.Y += speedY;
                    self.AutoJump = playerAutoJump;
                    self.RefillStamina();

                    // Create shot
                    previous_shot_time = 0;
                    level.Add(new ClassicBadelineShotInteractive(shot_center, 180 * direction, ClassicBadelineShotColorEnum.Red, 
                        is_can_damage_player: false, 
                        radius_explosion: 25));
                }
            }
            else
            {
                // Laser
                if (level.Entities.FindAll<ClassicBadelineBeam>().FindAll(t => !t.isEnded).Count > 0)
                {
                    // Cancel beam activating and restore power
                    if (_params.CurrentPower >= 2f)
                    {
                        is_temporary_stop_restoring_power = true;
                        self.RefillDash();
                        is_temporary_stop_restoring_power = false;
                    }
                    yield break;
                }

                if (!_params.TryUseWeapon("Laser"))
                    yield break;

                Methods.RegisterDash(level);
                self.RefillStamina();
                if (self.Ducking && self.CanUnDuck)
                    self.Ducking = false;

                level.Add(new ClassicBadelineBeam(self, direction));
                if (_params.CurrentPower >= 1f)
                {
                    is_temporary_stop_restoring_power = true;
                    self.RefillDash();
                    is_temporary_stop_restoring_power = false;
                }
            }
            
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
