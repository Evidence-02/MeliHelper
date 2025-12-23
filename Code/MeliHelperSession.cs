using Celeste.Mod.MeliHelper._BattleCity;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class MeliHelperSession : EverestModuleSession
    {
        public HashSet<int> BattleCity_StartedLevelsID { get; set; } = new HashSet<int>();
        public BattleCityCustomRules BattleCity_CustomRules { get; set; }
        public BattleCityPlayerInfo BattleCity_PlayerInfo { get; set; } = new BattleCityPlayerInfo();
        public BattleCityPlayerInfo BattleCity_PlayerInfoSaved { get; set; } = new BattleCityPlayerInfo();
        public string BattleCity_CampaignName { get; set; } = "";


        public Minesweeper_CellMark Minesweeper_CellMarker { get; set; } = Minesweeper_CellMark.None;

        public LaniHookParams LaniHook_Params { get; set; }
        public int LaniActiveFlagID { get; set; }



        public BadelinePowerParams BadelinePower_Params { get; set; }
    }

    class BattleCityPlayerInfo
    {
        public int Lifes { get; set; }
        public int Points { get; set; }
        public int Stars { get; set; }
        public bool MoveThroughWater { get; set; }

        public void StartCampaign(int lifes)
        {
            Lifes = lifes;
            Points = 0;
            Stars = 0;
            MoveThroughWater = false;
        }

        public void PlayerFakeDeath()
        {
            Lifes--;
            Stars = 0;
            if (MoveThroughWater)
            {
                MoveThroughWater = false;
                if (Engine.Scene != null && Engine.Scene is Level && !ProgressController.PlayerCanMoveThroughWater())
                    foreach (FieldCellWater cell_water in (Engine.Scene as Level).Entities.FindAll<FieldCellWater>())
                        cell_water.SetMoveThrough(false);
            }
        }

        public void BruhGameover()
        {
            Lifes = 0;
            Points = 0;
            Stars = 0;
            MoveThroughWater = false;
        }
    }

    class BattleCityCustomRules
    {
        public int PlayerShotsAtOnce { get; set; }
        public float PlayerShotSpeed { get; set; }
        public bool PlayerCanDestroySteel { get; set; }
        public bool PlayerCustomShooting { get; set; }
        public bool isVanillaDeaths { get; set; }
        public bool isShootOnlyCenter { get; set; }
    }

    class LaniHookParams
    {
        public string Direction { get; set; }
        public float Length { get; set; }
        public float Speed { get; set; }
        public float SpeedMovePlayer { get; set; }
        public float Cooldown { get; set; }
        public Color Color { get; set; }
        public bool isAllowHypers { get; set; }
    }


    class BadelinePowerParams
    {
        // Power
        public float FullPower { get; set; }
        public float CurrentPower { get; set; }
        public float ShootPower { private get; set; }
        public float LaserPower { private get; set; }
        public float BoostPower { private get; set; }
        public float AddMaxPowerOnStrawberryCollect { get; set; }
        public float AddMaxPowerOnGem { get; set; }


        // Possible weapons
        public bool isShootEnabled { get; set; }
        public bool isLaserEnabled { get; set; }
        public bool isBoostEnabled { get; set; }
        public bool isCurrentWeaponShoot { get; set; }


        // Visuals
        public string uiTexture { get; set; }
        public string uiLocation { get; set; }
        public bool isShowUI { get; set; }
        public bool isAffectPlayerSkin { get; set; }




        public bool TryUseWeapon(string weapon)
        {
            float power_use = 0;
            switch (weapon)
            {
                case "Shoot": power_use = ShootPower; break;
                case "Laser": power_use = LaserPower; break;
                case "Boost": power_use = BoostPower; break;
                default: return false;
            }

            if (CurrentPower >= power_use)
            {
                CurrentPower -= power_use;
                return true;
            }
            return false;
        }

        public void AddMaxPower(float add)
        {
            FullPower += add;
            RestorePower();
        }

        public void RestorePower(float val_additional = 0)
        {
            CurrentPower = Math.Max(CurrentPower, FullPower) + val_additional;
        }
    }
}
