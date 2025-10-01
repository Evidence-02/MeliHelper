using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    static class BonusesController
    {
        static Dictionary<BCEnum_BonusType, float> dict_chances;

        public static void Initialize()
        {
            dict_chances = new Dictionary<BCEnum_BonusType, float>();
            SetDefault();
        }

        public static void SetDefault()
        {
            foreach (BCEnum_BonusType type in Enum.GetValues(typeof(BCEnum_BonusType)))
                dict_chances[type] = 1;
        }

        public static void SetCustom(Dictionary<BCEnum_BonusType, float> dict_new)
        {
            dict_chances.Clear();
            foreach (var item in dict_new)
                dict_chances[item.Key] = item.Value;
        }



        public static BCEnum_BonusType GetRandomBonus()
        {
            if (MeliHelperModule.Settings.Debug_BonusType > 0)
                switch (MeliHelperModule.Settings.Debug_BonusType)
                {
                    case 1: return BCEnum_BonusType.DirtBall;
                    case 2: return BCEnum_BonusType.Mine;
                    case 3: return BCEnum_BonusType.Star;
                }



            BCEnum_BonusType res = BCEnum_BonusType.Star;
            int counter = 10;
            do
            {
                double chance = dict_chances.Values.Sum() * Calc.Random.NextDouble();
                double ch = 0;
                foreach (BCEnum_BonusType type in dict_chances.Keys)
                {
                    ch += dict_chances[type];
                    if (chance < ch)
                    {
                        res = type;
                        break;
                    }
                }
            }
            while (NeedReroll(res) && --counter >= 0);

            if (counter < 0) return BCEnum_BonusType.ExtraLife; // if rerolled 10 times in a row into bad type, get extra life
            return res;
        }

        public static bool NeedReroll(BCEnum_BonusType type)
        {
            return type == BCEnum_BonusType.Star && ProgressController.GetCurrentPlayerInfo().Stars >= 5 
                || type == BCEnum_BonusType.MoveThroughWater && ProgressController.GetCurrentPlayerInfo().MoveThroughWater;
        }


        /*
        public static BCEnum_BonusType GetDebugBonusType()
        {
            //switch (MeliHelperModule.Settings.Debug_BonusType)
            switch (0)
            {
                case 1: return BCEnum_BonusType.Star;       //  hello, this is krasty krabs?
                case 2:
                    switch (Calc.Random.Next(0, 6))
                    {
                        case 0: return BCEnum_BonusType.Duality;
                        case 1: return BCEnum_BonusType.EMI;
                        case 2: return BCEnum_BonusType.HomingBullets;
                        case 3: return BCEnum_BonusType.Shield;
                        case 4: return BCEnum_BonusType.TimeStop;
                        case 5: return BCEnum_BonusType.UnlimitedShooting;
                    }
                    break;

                case 3: return BCEnum_BonusType.DirtBall;
                case 4:
                    switch (Calc.Random.Next(0, 2))
                    {
                        case 0: return BCEnum_BonusType.DirtBall;
                        case 1: return BCEnum_BonusType.Mine;
                    }
                    break;

                case 5:
                    double chance = Calc.Random.NextDouble();
                    if (chance < 0.09f) return BCEnum_BonusType.ExtraLife;
                    if (chance < 0.32f) return BCEnum_BonusType.Star;
                    if (chance < 0.46f) return BCEnum_BonusType.TimeStop;
                    if (chance < 0.64f) return BCEnum_BonusType.Shield;
                    if (chance < 0.82f) return BCEnum_BonusType.Grenade;
                    return BCEnum_BonusType.Shovel;

                case 6:
                    return BCEnum_BonusType.Shovel;
            }
            
            return BCEnum_BonusType.ExtraLife;
        }
        */


        public static void SetVanilla()
        {
            foreach (BCEnum_BonusType type in Enum.GetValues(typeof(BCEnum_BonusType)))
                dict_chances[type] = (type == BCEnum_BonusType.ExtraLife
                                   || type == BCEnum_BonusType.Grenade
                                   || type == BCEnum_BonusType.Shield
                                   || type == BCEnum_BonusType.Shovel
                                   || type == BCEnum_BonusType.Star
                                   || type == BCEnum_BonusType.TimeStop) ? 1 : 0;
        }
    }
}
