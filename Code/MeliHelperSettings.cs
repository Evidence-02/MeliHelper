using Microsoft.Xna.Framework.Input;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    public class MeliHelperSettings : EverestModuleSettings
    {
        [DefaultButtonBinding(Buttons.LeftShoulder, Keys.X)]
        public ButtonBinding BattleCity_Shoot { get; set; }


        [DefaultButtonBinding(Buttons.LeftShoulder, Keys.X)]
        public ButtonBinding Minesweeper_ChangeDashMode { get; set; }


        [DefaultButtonBinding(Buttons.LeftShoulder, Keys.Tab)]
        public ButtonBinding BadelinePower_Switch { get; set; }



        [SettingIgnore()]
        public bool Debug_ShowEnemyAhhhhMovebox { get; set; } = false;

        [SettingIgnore()]
        public bool Debug_EnemiesPoisoned { get; set; } = false;

        [SettingIgnore()]
        public bool Debug_EnemiesShootingEndlessly { get; set; } = false;

        [SettingIgnore()]
        public bool Debug_FasterSpawn { get; set; } = false;

        [SettingIgnore()]
        public bool Debug_IntroEverytime { get; set; } = false;

        [SettingIgnore()]
        public bool Debug_BonusesEverytime { get; set; } = false;

        [SettingIgnore()]
        public bool Debug_UnlimitedShooting { get; set; } = false;

        [SettingIgnore()]
        public bool Debug_MoveThroughtWaterAlways { get; set; } = false;

        [SettingIgnore()]
        public bool Testing_MoveThroughWaterisDreamblock { get; set; } = false;


        [SettingIgnore()]
        [SettingRange(0, 999)]
        public int Debug_BonusType { get; set; } = 0;

        [SettingIgnore()]
        [SettingRange(0, 5)]
        public int Debug_StarPower { get; set; } = 0;



        [SettingIgnore()]
        public bool Debug_MinesweeperSolvedFromStart { get; set; } = false;

        [SettingIgnore()]
        public bool Debug_MinesweeperAutoWin { get; set; } = false;


        [SettingIgnore()]
        public bool Debug_LaniHookShowInfo { get; set; } = false;



        ////[SettingIgnore()]
        //[SettingRange(0, 999)]
        //public int Debug_LaniHookLength10 { get; set; } = 0;

        ////[SettingIgnore()]
        //[SettingRange(0, 999)]
        //public int Debug_LaniHookSpeed10 { get; set; } = 0;

        ////[SettingIgnore()]
        //[SettingRange(0, 999)]
        //public int Debug_LaniHookSpeedReturn10 { get; set; } = 0;

    }
}
