using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    static class ProgressController
    {
        public static void SaveProgress()
        {
            BattleCityPlayerInfo from = MeliHelperModule.Instance.Session.BattleCity_PlayerInfo;
            BattleCityPlayerInfo to = MeliHelperModule.Instance.Session.BattleCity_PlayerInfoSaved;
            to.Points = from.Points;
            to.Stars = from.Stars;
            to.Lifes = from.Lifes;
            to.MoveThroughWater = from.MoveThroughWater;

            string campaign = CampaignName();
            if (campaign != null && campaign != "")
            {
                CheckAndCreateCampaignHighScore(campaign);
                if (MeliHelperModule.Instance.SaveData.BattleCity_CampaignHiScores[campaign] < from.Points)
                    MeliHelperModule.Instance.SaveData.BattleCity_CampaignHiScores[campaign] = from.Points;
            }
        }
        
        public static void LoadProgress()
        {
            BattleCityPlayerInfo from = MeliHelperModule.Instance.Session.BattleCity_PlayerInfoSaved;
            BattleCityPlayerInfo to = MeliHelperModule.Instance.Session.BattleCity_PlayerInfo;
            to.Points = from.Points;
            to.Stars = from.Stars;
            to.Lifes = from.Lifes;
            to.MoveThroughWater = from.MoveThroughWater;

            string campaign = CampaignName();
            if (campaign != null && campaign != "")
                CheckAndCreateCampaignHighScore(campaign);

            //if (campaign != ""
            //    && MeliHelperModule.Instance.SaveData.BattleCity_HiScores != null
            //    && !MeliHelperModule.Instance.SaveData.BattleCity_HiScores.ContainsKey(campaign))
            //    MeliHelperModule.Instance.SaveData.BattleCity_HiScores[campaign] = 20000;
        }

        static void CheckAndCreateCampaignHighScore(string campaign, int default_value = 0) // 20000
        {
            if (MeliHelperModule.Instance.SaveData.BattleCity_CampaignHiScores is null)
                MeliHelperModule.Instance.SaveData.BattleCity_CampaignHiScores = new Dictionary<string, int>();
            if (!MeliHelperModule.Instance.SaveData.BattleCity_CampaignHiScores.Keys.Contains(campaign))
                MeliHelperModule.Instance.SaveData.BattleCity_CampaignHiScores[campaign] = default_value;
        }

        public static BattleCityPlayerInfo GetCurrentPlayerInfo()
        {
            return MeliHelperModule.Instance.Session.BattleCity_PlayerInfo;
        }

        public static int GetPlayerPower()
        {
            return (MeliHelperModule.Settings.DebugToolsBC.StarPower > 0) ? MeliHelperModule.Settings.DebugToolsBC.StarPower :
                   GetCurrentPlayerInfo().Stars;
        }

        public static bool isPlayerCanDestroySteel()
        {
            return (MeliHelperModule.Instance.Session.BattleCity_CustomRules != null)
                ? MeliHelperModule.Instance.Session.BattleCity_CustomRules.PlayerCanDestroySteel
                : GetPlayerPower() >= 3;
        }

        public static bool PlayerCanMoveThroughWater()
        {
            return MeliHelperModule.Settings.DebugToolsBC.MoveThroughtWaterAlways ||
                   GetCurrentPlayerInfo().MoveThroughWater;
        }

        public static void AddPoints(int value)
        {
            BattleCityPlayerInfo info = GetCurrentPlayerInfo();
            info.Points += value;
        }

        public static string CampaignName()
        {
            return MeliHelperModule.Instance.Session.BattleCity_CampaignName;
        }


    }
}
