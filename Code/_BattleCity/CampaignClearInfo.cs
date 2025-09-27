using Celeste;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;
using Celeste.Mod.Entities;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    [CustomEntity("MeliHelper/BattleCityCampaignClearInfo")]
    class CampaignClearInfo : Entity
    {
        public CampaignClearInfo(EntityData data, Vector2 offset) : base(data.Position + offset) 
        {
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            MeliHelperModule.Instance.Session.BattleCity_StartedLevelsID.Clear();
            MeliHelperModule.Instance.Session.BattleCity_CampaignName = "";
            ProgressController.GetCurrentPlayerInfo().BruhGameover();
            RemoveSelf();
        }
    }
}
