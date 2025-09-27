using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    [CustomEntity("MeliHelper/BattleCityCustomRulesEntity")]
    class CustomRulesEntity : Entity
    {
        BattleCityCustomRules rules;

        public CustomRulesEntity(EntityData data, Vector2 offset)
        {
            rules = new BattleCityCustomRules();
            rules.PlayerShotsAtOnce = data.Int("playerShotsAtOnce");
            rules.PlayerShotSpeed = data.Float("playerShotSpeed");
            rules.PlayerCanDestroySteel = data.Bool("playerCanDestroySteel");
            rules.PlayerCustomShooting = data.Bool("playerCustomShooting", true);
            rules.isVanillaDeaths = data.Bool("vanillaDeaths");
            rules.isShootOnlyCenter = data.Bool("shootOnlyCenter");
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            MeliHelperModule.Instance.Session.BattleCity_CustomRules = rules;
            RemoveSelf();
        }
    }
}
