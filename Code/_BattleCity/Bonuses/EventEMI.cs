using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    class EventEMI : EventBC
    {
        Player player;
        CustomTimer timer;

        public EventEMI(Player player, int ttl) 
            : base(BCEnum_BonusEvent.EMI, ttl)
        {
            this.player = player;
            timer = new CustomTimer(0.5f);
        }

        public override void Added(Entity entity)
        {
            base.Added(entity);
        }

        public override void Update()
        {
            base.Update();
            if (timer.Tick() && Methods.PlayerIsAlive(player))
            {

            }
        }

        public override void Removed(Entity entity)
        {
            base.Removed(entity);
        }
    }

}
