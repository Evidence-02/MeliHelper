using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    class EventShield : EventBC
    {
        Player player;
        Shield shield;

        public EventShield(Player player, int ttl) 
            : base(BCEnum_BonusEvent.Shield, ttl)
        {
            this.player = player;
        }

        public override void Added(Entity entity)
        {
            base.Added(entity);
            player.SceneAs<Level>().Add(shield = new Shield(player));
        }

        public override void Clear()
        {
            base.Clear();
            shield.RemoveSelf();
        }
    }

}
