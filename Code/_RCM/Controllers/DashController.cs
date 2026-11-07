using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class DashController
    {
        private static FieldInfo playerLastAim = typeof(Player).GetField("lastAim", BindingFlags.NonPublic | BindingFlags.Instance);
        static bool is_enabled;
        private static RubikDashSchemeEntity current_scheme;

        public static void Load()
        {
            if (!is_enabled)
            {
                On.Celeste.Player.DashBegin += onPlayerDashBegin;
                On.Celeste.Level.LoadLevel += onLevelLoad;
                On.Celeste.Strawberry.OnPlayer += onStrawberryCollect;
                is_enabled = true;
            }
        }

        public static void Unload()
        {
            if (is_enabled)
            {
                On.Celeste.Player.DashBegin -= onPlayerDashBegin;
                On.Celeste.Level.LoadLevel -= onLevelLoad;
                On.Celeste.Strawberry.OnPlayer -= onStrawberryCollect;
                is_enabled = false;
            }
        }

        static void onLevelLoad(On.Celeste.Level.orig_LoadLevel orig, Level self, Player.IntroTypes intro, bool isFromLoader)
        {
            if (intro == Player.IntroTypes.Respawn && !isFromLoader && MeliHelperModule.Instance.Session.RerollSeedOnDeath)
                SeedController.GenerateNewSeed();

            orig(self, intro, isFromLoader);
            TryLoadScheme(self, RCMEnum_ControlScheme.Main);
        }

        static void onPlayerDashBegin(On.Celeste.Player.orig_DashBegin orig, Player self)
        {
            orig(self);
            if (current_scheme != null)
            {
                Level level = self.SceneAs<Level>();
                Vector2 v = (self.OverrideDashDirection.HasValue) ? self.OverrideDashDirection.Value : (Vector2)playerLastAim.GetValue(self);
                if (   self.StateMachine.State != Player.StRedDash && self.StateMachine.State != Player.StStarFly 
                    && !level.Entities.FindAll<RubikCancelDashTrigger>().Exists(t => t.CollideCheck(self) && t.isCancelDash(v)))
                {
                    foreach (RubikCube cube in level.Entities.FindAll<RubikCube>())
                        current_scheme.Dash(cube, v);
                }
            }
        }

        static void onStrawberryCollect(On.Celeste.Strawberry.orig_OnPlayer orig, Strawberry self, Player player)
        {
            orig(self, player);
            if (is_enabled && self.Golden && !MeliHelperModule.Instance.Session.HideSeed)
            {
                Level level = self.SceneAs<Level>();
                MeliHelperModule.Instance.Session.RerollSeedOnSolve = true;
                MeliHelperModule.Instance.Session.HideSeed = true;
                SeedController.GenerateNewSeed();
                SeedController.UpdateSeedEntities(level);
                UpdateGoldenEntities(level);
                if (player != null && !player.Dead)
                    level.Add(new CustomRubikWaveText(player, Dialog.Get("EVIDENCE02_RCM_MESSAGE_SEEDREROLLED"), ttl: 4.8f));
            }
        }

        public static void UpdateGoldenEntities(Level level)
        {
            foreach (CustomRubikBackground item in level.Entities.FindAll<CustomRubikBackground>())
                item.UpdateGolden(level);
            foreach (CustomRubikPointsDoor item in level.Entities.FindAll<CustomRubikPointsDoor>())
                item.UpdateGolden(level);
        }

        public static void TryLoadScheme(Level level, RCMEnum_ControlScheme scheme_type)
        {
            List<RubikDashSchemeEntity> list_schemes = level.Entities.FindAll<RubikDashSchemeEntity>();
            current_scheme = list_schemes.FirstOrDefault(t => t.GetScheme == scheme_type);
            if (current_scheme == null)                           current_scheme = list_schemes.FirstOrDefault(t => t.GetScheme == RCMEnum_ControlScheme.Main);
            if (current_scheme == null && list_schemes.Count > 0) current_scheme = list_schemes[0];

            //foreach (RubikDisplayScheme display in level.Entities.FindAll<RubikDisplayScheme>())
            //    display.GetScheme = current_scheme;
        }

        public static RubikDashSchemeEntity GetCurrentScheme()
        {
            return current_scheme;
        }


    }
}
