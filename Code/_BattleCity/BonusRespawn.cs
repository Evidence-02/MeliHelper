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
    [CustomEntity("MeliHelper/BattleCityBonusRespawn")]
    class BonusRespawn : Bonus
    {
        CustomTimer timer;
        bool is_active;
        float opacity_inactive;

        public BonusRespawn(EntityData data, Vector2 offset) 
            : base(data.Position + offset, (BCEnum_BonusType)Enum.Parse(typeof(BCEnum_BonusType), data.Attr("bonusType", "Star")))
        {
            timer = new CustomTimer(data.Float("period", 2.4f));
            opacity_inactive = data.Float("opacityInactive", 0.3f);
            Depth = data.Int("depth", 0);
        }

        public override void Update()
        {
            base.Update();
            if (!is_active && timer.Tick())
            {
                is_active = true;
                image.Color = Color.White;
                Audio.Play(SFX.music_reflection_main);
            }
        }

        protected override void onPlayer(Player player)
        {
            if (is_active)
            {
                base.onPlayer(player);
                // refill collect sound?
                //Audio.Play(Refill.P_Glow);
                image.Color = Color.White * opacity_inactive;
                is_active = false;
            }
        }
    }
}
