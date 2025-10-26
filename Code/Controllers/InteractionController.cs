using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class InteractionController
    {
        private static MethodInfo playerDreamDashCheck = typeof(Player).GetMethod("DreamDashCheck", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo playerDashAttackTimer = typeof(Player).GetField("dashAttackTimer", BindingFlags.NonPublic | BindingFlags.Instance);
        private static MethodInfo playerOnPickup = typeof(Player).GetMethod("Pickup", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo playerHoldTimer = typeof(Player).GetField("minHoldTimer", BindingFlags.NonPublic | BindingFlags.Instance);

        private static FieldInfo dreamblockPlayerCanDash = typeof(DreamBlock).GetField("playerHasDreamDash", BindingFlags.NonPublic | BindingFlags.Instance);
        private static MethodInfo clutchSwitchOnDashed = typeof(ClutterSwitch).GetMethod("OnDashed", BindingFlags.NonPublic | BindingFlags.Instance);
        private static MethodInfo oshiroOnPlayerBounce = typeof(AngryOshiro).GetMethod("OnPlayerBounce", BindingFlags.NonPublic | BindingFlags.Instance);
        private static MethodInfo swapblockOnDash = typeof(SwapBlock).GetMethod("OnDash", BindingFlags.NonPublic | BindingFlags.Instance);
        private static MethodInfo seekerGotBounced = typeof(Seeker).GetMethod("GotBouncedOn", BindingFlags.NonPublic | BindingFlags.Instance);
        private static MethodInfo eyeballOnHoldable = typeof(TempleBigEyeball).GetMethod("OnHoldable", BindingFlags.NonPublic | BindingFlags.Instance);
        private static MethodInfo kevinblockOnDashed = typeof(CrushBlock).GetMethod("OnDashed", BindingFlags.NonPublic | BindingFlags.Instance);
        private static MethodInfo featherOnPlayer = typeof(FlyFeather).GetMethod("OnPlayer", BindingFlags.NonPublic | BindingFlags.Instance);
        //private static MethodInfo bumperOnChangeMode = typeof(Bumper).GetMethod("OnChangeMode", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo finalBossShotSpeed = typeof(FinalBossShot).GetField("speed", BindingFlags.NonPublic | BindingFlags.Instance);
        private static MethodInfo summitGemOnPlayer = typeof(SummitGem).GetMethod("OnPlayer", BindingFlags.NonPublic | BindingFlags.Instance);
        private static MethodInfo heartGemCollect = typeof(HeartGem).GetMethod("Collect", BindingFlags.NonPublic | BindingFlags.Instance);
        private static MethodInfo coreChangeModeBounceBlock = typeof(BounceBlock).GetMethod("OnChangeMode", BindingFlags.NonPublic | BindingFlags.Instance);
        private static MethodInfo coreChangeModeFireBarrier = typeof(FireBarrier).GetMethod("OnChangeMode", BindingFlags.NonPublic | BindingFlags.Instance);
        private static MethodInfo coreChangeModeIceBlock = typeof(IceBlock).GetMethod("OnChangeMode", BindingFlags.NonPublic | BindingFlags.Instance);
        private static MethodInfo coreChangeModeFireball = typeof(FireBall).GetMethod("OnChangeMode", BindingFlags.NonPublic | BindingFlags.Instance);
        private static MethodInfo coreChangeModeWallBooster = typeof(WallBooster).GetMethod("OnChangeMode", BindingFlags.NonPublic | BindingFlags.Instance);
        private static MethodInfo coreModeToggleOnPlayer = typeof(CoreModeToggle).GetMethod("OnPlayer", BindingFlags.NonPublic | BindingFlags.Instance);
        private static MethodInfo pufferAlert = typeof(Puffer).GetMethod("Alert", BindingFlags.NonPublic | BindingFlags.Instance);
        private static MethodInfo pufferExplode = typeof(Puffer).GetMethod("Explode", BindingFlags.NonPublic | BindingFlags.Instance);
        private static MethodInfo pufferGotoGone = typeof(Puffer).GetMethod("GotoGone", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo pufferStartPosition = typeof(Puffer).GetField("startPosition", BindingFlags.NonPublic | BindingFlags.Instance);

        
        #region Blocks

        public static void ActivateBlock(DashBlock block, Entity collide_entity)
        {
            if (block != null && collide_entity != null)
                block.Break(collide_entity.Center, collide_entity.Center - block.Center, true, true);
        }

        public static void ActivateBlock(FallingBlock block, Entity entity)
        {
            if (!block.Triggered)
            {
                Audio.Play(SFX.game_gen_fallblock_shake);
                block.Triggered = true;
            }
        }

        public static void ActivateCrumblePlatform(CrumblePlatform block, Entity entity)
        {
            Level level = block.SceneAs<Level>();
            Player player = level.Tracker.GetEntity<Player>();
            if (player != null)
            {
                Vector2 player_center = player.Center;
                player.Center = block.Center;
                block.Update();
                player.Center = player_center;
            }
        }

        public static void ActivateTouchSwitch(TouchSwitch switch_touch)
        {
            // In the first version there was a bug with multiply sounds, when check on ease didn't exists yet
            // Normally check should be "ease == 0", but I like that sound bug
            // A different deep sound on touch just wow
            DynData<TouchSwitch> dyn = new DynData<TouchSwitch>(switch_touch);
            Single ease = dyn.Get<Single>("ease");
            if (ease < 0.16f)
            {
                Audio.Play(SFX.game_gen_touchswitch_any, "", 0.4f);
                switch_touch.TurnOn();
            }
        }

        public static void ActivateDashSwitch(DashSwitch dash_switch)
        {
            DynData<DashSwitch> dyn = new DynData<DashSwitch>(dash_switch);
            DashSwitch.Sides side = dyn.Get<DashSwitch.Sides>("side");
            Vector2 direction = Vector2.Zero;
            switch (side)
            {
                case DashSwitch.Sides.Up: direction = new Vector2(0, -1); break;
                case DashSwitch.Sides.Down: direction = new Vector2(0, +1); break;
                case DashSwitch.Sides.Left: direction = new Vector2(-1, 0); break;
                case DashSwitch.Sides.Right: direction = new Vector2(+1, 0); break;
            }
            dash_switch.OnDashed(null, direction);
        }

        public static void ImitateHeartGemBounce(HeartGem heart, Player player, Vector2 speed)
        {
            Audio.Play(SFX.game_gen_crystalheart_bounce, heart.Position);
            Vector2 player_center_old = player.Center;
            Vector2 player_speed_old = player.Speed;
            player.Center = heart.Center - 2 * speed;
            player.Speed = speed;
            heart.OnPlayer(player);
            player.Center = player_center_old;
            player.Speed = player_speed_old;
        }

        public static void ImitateHeartGemCollect(HeartGem heart, Player player)
        {
            //Audio.Play(SFX.game_gen_crystalheart_blue_get, heart.Position);

            //On.Celeste.HeartGem.Collect;
            //ImitateHeartGemBounce(heart, player, 240 * Vector2.One);
            heartGemCollect.Invoke(heart, new object[] { player });
        }

        public static void ImitateSummitGemBounce(SummitGem gem, Player player, Vector2 speed)
        {
            // no CollectRoutine? just imitate bounce from player, lol
            Vector2 player_center_old = player.Center;
            Vector2 player_speed_old = player.Speed;
            player.Speed = speed;
            player.Center = gem.Center - speed / 2;
            summitGemOnPlayer.Invoke(gem, new object[] { player });
            player.Center = player_center_old;
            player.Speed = player_speed_old;
        }

        #endregion

        #region Player

        public static bool PlayerPickupHoldable(Player player, Holdable holdable)
        {
            playerHoldTimer.SetValue(player, 0);
            new DynData<Holdable>(holdable)["cannotHoldTimer"] = 0;
            if ((bool)playerOnPickup.Invoke(player, new object[] { holdable }))
            {
                player.StateMachine.State = Player.StPickup;
                playerHoldTimer.SetValue(player, 0.05f);
                return true;
            }
            return false;
        }

        public static void PlayerSetPickupTimer(Player player, float value)
        {
            playerHoldTimer.SetValue(player, value);
        }

        #endregion

        #region 1. Forsaken City

        public static void ActivateBlock(ZipMover block, Entity entity)
        {
            Level level = block.SceneAs<Level>();
            Player player = level.Tracker.GetEntity<Player>();
            if (player != null)
            {
                Vector2 player_center_old = player.Center;
                player.Center = block.Center;
                block.Update();
                player.Center = player_center_old;
            }
        }

        #endregion

        #region 2. Old Site

        public static bool DashIntoDreamBlock(DreamBlock block, Player player)
        {
            if (player == null) return false;

            Level level = block.SceneAs<Level>();
            level.Session.Inventory.DreamDash = true;
            player.DashDir = Vector2.UnitX * Math.Sign(player.Speed.X) + Vector2.UnitY * Math.Sign(player.Speed.Y);
            player.RefillDash();
            playerDashAttackTimer.SetValue(player, 2f);
            if ((bool)playerDreamDashCheck.Invoke(player, new object[] { player.DashDir }))
            {
                // Imitate classic start of dream dashing inside of player class
                player.StateMachine.State = Player.StDreamDash;
                playerDashAttackTimer.SetValue(player, 0);
                return true;
            }
            return false;
        }

        public static bool CanDashIntoDreamBlock(DreamBlock block)
        {
            return (bool)dreamblockPlayerCanDash.GetValue(block);
        }


        #endregion

        #region 3. Celestial Resort

        public static void ActivateClutterSwitch(ClutterSwitch clutter_switch, Vector2 entity_speed)
        {
            Level level = clutter_switch.SceneAs<Level>();
            Player player = level.Tracker.GetEntity<Player>();
            if (player != null)
                clutchSwitchOnDashed.Invoke(clutter_switch, new object[] { player, new Vector2(0, 1) });
        }

        public static void HitOshiro(AngryOshiro boss_oshiro, Player player)
        {
            // doesn't work when Oshiro in pre-attack state (attackIndex <> 3)
            Vector2 player_center_old = player.Center;
            Vector2 player_speed_old = player.Speed;
            DynData<AngryOshiro> dyn = new DynData<AngryOshiro>(boss_oshiro);
            player.Center = boss_oshiro.Center - new Vector2(0, 20);
            dyn.Set("attackIndex", 3);
            oshiroOnPlayerBounce.Invoke(boss_oshiro, new object[] { player });
            player.Center = player_center_old;
            player.Speed = player_speed_old;
        }

        #endregion

        #region 4. Golden Ridge

        public static void ActivateBlock(MoveBlock block, Entity entity)
        {
            block.OnStaticMoverTrigger(null);
        }

        public static void HitSnowball(Snowball snowball)
        {
            Audio.Play(SFX.game_04_snowball_impact);
            DynData<Snowball> dyn = new DynData<Snowball>(snowball);
            Sprite sprite = dyn.Get<Sprite>("sprite");
            snowball.Collidable = false;
            sprite.Play("break");
        }

        #endregion

        #region 5. Mirror Temple

        public static void ActivateBlock(SwapBlock block, Entity entity)
        {
            swapblockOnDash.Invoke(block, new object[] { entity.Center });
        }

        public static void ActivateBlock(TempleCrackedBlock block, Entity entity)
        {
            block.Break(entity.Center);
        }

        public static bool ExplodeBooster(Booster booster, float maxRespawnTimer = 0.01f)
        {
            DynData<Booster> dyn = new DynData<Booster>(booster);
            float respawnTimer = dyn.Get<float>("respawnTimer");
            if (respawnTimer < maxRespawnTimer)
            {
                // Actually doesn't interact with player, so it's safe 
                Player player = booster.SceneAs<Level>().Tracker.GetEntity<Player>();
                if (player != null)
                    booster.PlayerBoosted(player, Vector2.Zero);
                booster.PlayerReleased();
                return true;
            }
            return false;
        }

        public static void HitSeeker(Seeker seeker, Entity entity)
        {
            seekerGotBounced.Invoke(seeker, new object[] { entity });
        }

        public static void ActivateSeekerStatue(SeekerStatue seekerStatue, Player player)
        {
            Vector2 player_center_old = player.Center;
            player.Center = seekerStatue.Center + new Vector2(80, 0);
            seekerStatue.Update();
            player.Center = player_center_old;
        }

        public static void HitTempleGiantEyeball(TempleBigEyeball eyeball_ch5, Player player, TheoCrystal crystal)
        {
            Vector2 crystal_center_old = crystal.Speed;
            Vector2 crystal_speed_old = crystal.Speed;
            crystal.Center = eyeball_ch5.Center - new Vector2(50, 0);
            crystal.Speed = new Vector2(100, 0);
            eyeballOnHoldable.Invoke(eyeball_ch5, new object[] { crystal.Hold });
            crystal.Center = crystal_center_old;
            crystal.Speed = crystal_speed_old;
        }



        #endregion

        #region 6. Reflection

        public static void ActivateBlockKevin(CrushBlock block, Entity entity, float power_move)
        {
            //IL.Celeste.CrushBlock.OnDashed;
            Level level = block.SceneAs<Level>();
            Player player = level.Tracker.GetEntity<Player>();
            if (player != null)
            {
                Vector2 speed_kevin = Vector2.Zero;
                if (entity.Top < block.Top) speed_kevin.Y = +power_move;
                else if (entity.Bottom > block.Bottom) speed_kevin.Y = -power_move;
                else if (entity.Left < block.Left) speed_kevin.X = +power_move;
                else if (entity.Right > block.Right) speed_kevin.X = -power_move;
                else
                    speed_kevin = -power_move * Methods.RectangleDirection(new Rectangle((int)block.Left, (int)block.Top, (int)block.Width, (int)block.Height), entity.Center);

                kevinblockOnDashed.Invoke(block, new object[] { player, speed_kevin });
            }
        }

        public static void FeatherCollect(FlyFeather feather, Player player)
        {
            featherOnPlayer.Invoke(feather, new object[] { player });
        }

        public static void HitBadelineBoss(FinalBoss boss, Player player = null)
        {
            boss.OnPlayer(player);
        }

        public static void SetBadelineBossShotSpeed(FinalBossShot shot, Vector2 speed_new)
        {
            finalBossShotSpeed.SetValue(shot, speed_new);
        }



        #endregion

        #region 8. Core

        public static void SetCoreMode(BounceBlock bounceblock, Session.CoreModes mode)
        {
            coreChangeModeBounceBlock.Invoke(bounceblock, new object[] { mode });
        }

        public static void SetCoreMode(FireBarrier barrier, Session.CoreModes mode)
        {
            coreChangeModeFireBarrier.Invoke(barrier, new object[] { mode });
        }

        public static void SetCoreMode(IceBlock iceblock, Session.CoreModes mode)
        {
            coreChangeModeIceBlock.Invoke(iceblock, new object[] { mode });
        }

        public static void SetCoreMode(FireBall fireball, Session.CoreModes mode)
        {
            coreChangeModeFireball.Invoke(fireball, new object[] { mode });
        }

        public static void SetCoreMode(WallBooster wall_booster, Session.CoreModes mode)
        {
            coreChangeModeWallBooster.Invoke(wall_booster, new object[] { mode });
        }

        public static void SwitchCoreModeToggle(CoreModeToggle core_toggle, Player player)
        {
            coreModeToggleOnPlayer.Invoke(core_toggle, new object[] { player });
        }


        #endregion

        #region 9. Farewell

        public static void ActivateBlock(LightningBreakerBox block, Entity entity, Vector2 speed)
        {
            Level level = block.SceneAs<Level>();
            Player player = level.Tracker.GetEntity<Player>();
            if (player != null)
                block.Dashed(player, speed);
        }

        public static void PufferAlert(Puffer puffer, float alert_timer_max = 2f)
        {
            DynData<Puffer> dyn = new DynData<Puffer>(puffer);
            Single alertTimer = dyn.Get<Single>("alertTimer");
            if (alertTimer <= alert_timer_max)
                pufferAlert.Invoke(puffer, new object[] { true, true });

            // animations: idle, alert, alerted, unalert, explode, hidden, recover
            //Sprite sprite = dyn.Get<Sprite>("sprite");
            //if (sprite.CurrentAnimationID == "idle")
            //    pufferAlert.Invoke(puffer_alert, new object[] { true, true });
        }

        public static void PufferExplode(Puffer puffer)
        {
            // Explode and restore fish!
            pufferExplode.Invoke(puffer, new object[] { });
            puffer.Collidable = false;
            pufferGotoGone.Invoke(puffer, new object[] { });
        }

        public static Vector2 PufferGetPosition(Puffer puffer)
        {
            return (Vector2)pufferStartPosition.GetValue(puffer);
        }

        public static void PufferSetPosition(Puffer puffer, Vector2 value)
        {
            pufferStartPosition.SetValue(puffer, value);
        }

        #endregion










    }
}
