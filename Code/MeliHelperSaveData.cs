using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class MeliHelperSaveData : EverestModuleSaveData
    {
        public HashSet<string> CustomFlags { get; set; } = new HashSet<string>();

        public Dictionary<string, int> BattleCity_CampaignHiScores { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> BattleCity_HiScores { get; set; } = new Dictionary<string, int>();

        public int MinesweeperFieldSeed { get; set; } = 0;

    }
}
