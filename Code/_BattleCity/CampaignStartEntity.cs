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
    [CustomEntity("MeliHelper/BattleCityCampaignStartEntity")]
    class CampaignStartEntity : Entity
    {
        string name;
        int count_lifes;

        public CampaignStartEntity(EntityData data, Vector2 offset) : base(data.Position + offset) 
        {
            name = data.Attr("name", "Vanilla");
            count_lifes = data.Int("lifes", 3);
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);

            if (MeliHelperModule.Instance.Session.BattleCity_CampaignName != name)
            {
                MeliHelperModule.Instance.Session.BattleCity_CampaignName = name;
                MeliHelperModule.Instance.Session.BattleCity_PlayerInfoSaved.StartCampaign(count_lifes);
                ProgressController.GetCurrentPlayerInfo().StartCampaign(count_lifes);
            }
            RemoveSelf();
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);

        }
    }
}
