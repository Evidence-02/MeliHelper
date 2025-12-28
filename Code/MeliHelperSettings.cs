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




        ////[SettingIgnore()]
        //[SettingRange(0, 999)]
        //public int Debug_LaniHookLength10 { get; set; } = 0;

        ////[SettingIgnore()]
        //[SettingRange(0, 999)]
        //public int Debug_LaniHookSpeed10 { get; set; } = 0;

        ////[SettingIgnore()]
        //[SettingRange(0, 999)]
        //public int Debug_LaniHookSpeedReturn10 { get; set; } = 0;




        [SettingSubMenu]
        public class DebugBC_Tools
        {
            [SettingRange(0, 5)]
            public int StarPower { get; set; } = 0;

            [SettingRange(0, 999)]
            public int BonusType { get; set; } = 0;

            public bool ShowEnemyHitboxes { get; set; } = false;
            public bool EnemiesPoisoned { get; set; } = false;
            public bool EnemiesShootingEndlessly { get; set; } = false;
            public bool EnemiesFasterSpawn { get; set; } = false;
            public bool IntroEverytime { get; set; } = false;
            public bool BonusesEverytime { get; set; } = false;
            public bool UnlimitedShooting { get; set; } = false;
            public bool MoveThroughtWaterAlways { get; set; } = false;
            public bool MoveThroughWaterAsDreamblock { get; set; } = false;
        }
        [SettingName("EVIDENCE02_MELIHELPER_DEBUGTOOLSBC")]
        public DebugBC_Tools DebugToolsBC { get; set; } = new DebugBC_Tools();



        [SettingSubMenu]
        public class Debug_Tools
        {
            public bool LaniHookShowHitboxes { get; set; } = false;
            public bool MinesweeperSolvedFromStart { get; set; } = false;
            public bool MinesweeperCantLose { get; set; } = false;
        }
        [SettingName("EVIDENCE02_MELIHELPER_DEBUGTOOLS")]
        public Debug_Tools DebugTools { get; set; } = new Debug_Tools();
    }
}
