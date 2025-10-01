using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._Lani
{
    [CustomEntity("MeliHelper/LaniSnowballGenerator")]
    class LaniSnowballGenerator : Entity
    {
        Level level;
        Vector2 pos, snowball_speed;
		CustomTimer timer;
        int count_bounces;

        public LaniSnowballGenerator(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
			timer = new CustomTimer(data.Float("period", 2f), data.Float("delay", 2f));
            snowball_speed = new Vector2(data.Float("speedX"), data.Float("speedY"));
            pos = this.Position + new Vector2(data.Float("offsetX"), data.Float("offsetY"));
            count_bounces = data.Int("bounces");
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = scene as Level;
        }

        public override void Update()
        {
            base.Update();
            if (timer.Tick())
                level.Add(new LaniSnowball(pos, snowball_speed, count_bounces));
        }
    }
}
