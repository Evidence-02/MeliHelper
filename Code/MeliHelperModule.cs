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
            BCController.Load();
            LevelTemplateController.Load();
            BonusesController.Initialize();
            EnemyTypesController.Initialize();
            LaniController.Load();
            BaddyController.Load();
        }

        public override void Unload()
        {
            Everest.Events.Level.OnLoadBackdrop -= OnLoadBackdrop;
            BCController.Unload();
            LevelTemplateController.Unload();
            LaniController.Unload();
            BaddyController.Unload();
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
