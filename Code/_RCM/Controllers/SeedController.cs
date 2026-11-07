using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class SeedController
    {
        //const string SEED_LETTERS = "0123456789ABCDEFG";
        //const int CHAR_COUNT = 16;
        public const string SEED_LETTERS = "0123456789ABCDEFGHIJKLMNPQRSTUVWXYZ";
        const int CHAR_COUNT = 35;
        const int SEED_LENGTH = 4;
        static bool is_pushed;

        public static void Initialize()
        {
            if (MeliHelperModule.Instance.Session.Seed == null || MeliHelperModule.Instance.Session.Seed == "")
            {
                // 26 big letters + 26 small letters + 10 numbers + 26 symbols = 88 symbols.... umm, no. defused to 36 main symbols
                string NAME_LETTERS = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

                string name = SaveData.Instance.Name.ToUpper();
                int seed_for_the_first_seed = 0;
                for (int i = 0; i < name.Length; i++)
                {
                    int index = NAME_LETTERS.IndexOf(name[i]);
                    seed_for_the_first_seed += index * (int)Math.Pow(NAME_LETTERS.Length, i);
                }

                Calc.PushRandom(seed_for_the_first_seed);
                GenerateNewSeed();
                Calc.PopRandom();
            }     
        }

        public static void GenerateNewSeed()
        {
            if (is_pushed)
                StopRandomizer();

            MeliHelperModule.Instance.Session.SeedID = Calc.Random.Next(0, (int)Math.Pow(CHAR_COUNT, SEED_LENGTH));
            MeliHelperModule.Instance.Session.Seed   = ConvertIDtoSeed(MeliHelperModule.Instance.Session.SeedID);
        }

        public static string ConvertIDtoSeed(int id)
        {
            string seed = "";
            for (int i = 0; i < SEED_LENGTH; i++)
            {
                seed += SEED_LETTERS[id % CHAR_COUNT];
                id /= CHAR_COUNT;
            }
            return seed;
        }

        public static int ConvertSeedToID(string seed)
        {
            int id = 0;
            for (int i = 0; i < seed.Length; i++)
            {
                id += SEED_LETTERS.IndexOf(seed[i]);
                id *= CHAR_COUNT;
            }
            return id;
        }

        public static string GetCurrentSeed()
        {
            return MeliHelperModule.Instance.Session.Seed;
        }

        public static int GetCurrentSeedID()
        {
            return MeliHelperModule.Instance.Session.SeedID;
        }

        public static void SetCurrentSeed(string seed_new)
        {
            MeliHelperModule.Instance.Session.Seed = seed_new;
            MeliHelperModule.Instance.Session.SeedID = ConvertSeedToID(seed_new);
        }
        
        public static void UpdateSeedEntities(Level level)
        {
            foreach (SeedLabel item in level.Entities.FindAll<SeedLabel>())
                item.UpdateSeed();
            foreach (SeedComputer item in level.Entities.FindAll<SeedComputer>())
                item.UpdateSeed();
        }

        public static void RestartRandomizer()
        {
            if (is_pushed) Calc.PopRandom();
            Calc.PushRandom(GetCurrentSeedID());
            is_pushed = true;
        }

        public static void StartRandomizer()
        {
            if (is_pushed) return;
            Calc.PushRandom(GetCurrentSeedID());
            is_pushed = true;
        }

        public static void StopRandomizer()
        {
            if (!is_pushed) return;
            Calc.PopRandom();
            is_pushed = false;
        }



        //static PlayerDeadBody onPlayerDie(On.Celeste.Player.orig_Die orig, Player self, Vector2 direction, bool evenIfInvincible, bool registerDeathInStats)
        //{
        //    PlayerDeadBody res = orig(self, direction, evenIfInvincible, registerDeathInStats);
        //    if (MeliHelperModule.Instance.Session.RerollSeedOnDeath)
        //        GenerateNewSeed();
        //    return res;
        //}
    }
}
