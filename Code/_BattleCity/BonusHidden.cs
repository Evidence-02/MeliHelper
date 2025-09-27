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
    [CustomEntity("MeliHelper/BattleCityBonusHidden")]
    class BonusHidden : Bonus
    {
        public BonusHidden(EntityData data, Vector2 offset) 
            : base(data.Position + offset, (BCEnum_BonusType)Enum.Parse(typeof(BCEnum_BonusType), data.Attr("bonusType", "Star")))
        {
            Depth = data.Int("depth", 0);
        }

        protected override void onPlayer(Player player)
        {
            base.onPlayer(player);
            ProgressController.AddPoints(500);
            RemoveSelf();
        }
    }
}
