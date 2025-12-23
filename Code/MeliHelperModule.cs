using Celeste.Mod;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Celeste.Mod.MeliHelper._BattleCity;
using Celeste.Mod.MeliHelper._Lani;
using Celeste.Mod.MeliHelper._Baddy;

namespace Celeste.Mod.MeliHelper
{
    class MeliHelperModule : EverestModule
    {
        public static MeliHelperModule Instance;

        public override Type SettingsType => typeof(MeliHelperSettings);
        public static MeliHelperSettings Settings => (MeliHelperSettings)Instance._Settings;


        public override Type SessionType => typeof(MeliHelperSession);
        public MeliHelperSession Session => _Session as MeliHelperSession;


        public override Type SaveDataType => typeof(MeliHelperSaveData);
        public MeliHelperSaveData SaveData => _SaveData as MeliHelperSaveData;


        public MeliHelperModule()
        {
            Instance = this;
        }

        public override void Load()
        {
            Everest.Events.Level.OnLoadBackdrop += OnLoadBackdrop;
            On.Celeste.Level.LoadLevel += onLoadLevel;
            On.Celeste.Session.Restart += onSessionRestart;
            //On.Celeste.Level.End += onEndLevel;
            //On.Celeste.LevelExit.Begin += onLevelExitBegin;

            LevelTemplateController.Load();
            BCController.Load();
            BonusesController.Initialize();
            EnemyTypesController.Initialize();
        }

        public override void Unload()
        {
            Everest.Events.Level.OnLoadBackdrop -= OnLoadBackdrop;
            On.Celeste.Level.LoadLevel -= onLoadLevel;
            On.Celeste.Session.Restart -= onSessionRestart;
            //On.Celeste.Level.End -= onEndLevel;
            //On.Celeste.LevelExit.Begin -= onLevelExitBegin;

            LevelTemplateController.Unload();
            BCController.Unload();
            KillPlayerDashBlock.Unload();
            PuzzleBlockBreaking.Unload();
        }

        public static void onLoadLevel(On.Celeste.Level.orig_LoadLevel orig, Level self, Player.IntroTypes playerIntro, bool isFromLoader)
        {
            orig(self, playerIntro, isFromLoader);
            CustomLogger.Log("MeliHelperModule.onLoadLevel", self.Session.Level);
            if (Instance.Session.LaniHook_Params      != null && !LaniController.isLoaded())  LaniController.SetHook(Instance.Session.LaniHook_Params);
            if (Instance.Session.BadelinePower_Params != null && !BaddyController.isLoaded()) BaddyController.SetPower(self, Instance.Session.BadelinePower_Params);
        }

        public static Session onSessionRestart(On.Celeste.Session.orig_Restart orig, Session self, string intoLevel)
        {
            Session session = orig(self, intoLevel);
            CustomLogger.Log("MeliHelperModule.onSessionRestart", intoLevel);
            if (LaniController.isLoaded()) LaniController.ClearHook();
            if (BaddyController.isLoaded()) BaddyController.ClearPower();
            return session;
        }

        public static void onEndLevel(On.Celeste.Level.orig_End orig, Level self)
        {
            orig(self);
            CustomLogger.Log("MeliHelperModule.LevelEnd", self.Session.Level);
        }

        public static void onLevelExitBegin(On.Celeste.LevelExit.orig_Begin orig, LevelExit self)
        {
            orig(self);
            CustomLogger.Log("MeliHelperModule.LevelExitBegin", "");
        }




        private Backdrop OnLoadBackdrop(MapData map, BinaryPacker.Element child, BinaryPacker.Element super)
        {
            if (child.Name.Equals("MeliHelper/CirclineDisasterBackdrop", StringComparison.OrdinalIgnoreCase))
                return new CirclineDisasterBackdrop(map, child, super);

            if (child.Name.Equals("MeliHelper/StarCitizenBackdrop", StringComparison.OrdinalIgnoreCase))
                return new StarCitizenBackdrop(map, child, super);

            return null;
        }
    }
}
