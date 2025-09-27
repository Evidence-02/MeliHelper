using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    class EventTimeStop : EventBC
    {
        public EventTimeStop(int ttl) 
            : base(BCEnum_BonusEvent.TimeStop, ttl)
        {
        }

        public override void Added(Entity entity)
        {
            base.Added(entity);
            //foreach (Enemy enemy in Entity.SceneAs<Level>().Entities.FindAll<Enemy>().FindAll(t => !t.isDead))
            //    enemy.GetSprite.Stop();
        }

        public override void Removed(Entity entity)
        {
            base.Removed(entity);
            //foreach (Enemy item in Entity.SceneAs<Level>().Entities.FindAll<Enemy>().FindAll(t => !t.isDead))
            //    Methods.RestoreSpriteAfterStop(item.GetSprite);
        }
    }

}
