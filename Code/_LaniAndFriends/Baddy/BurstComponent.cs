using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class BurstComponent : Component
    {
        Level level;
        float power, duration, alpha;
        float timer, period, period_min, period_del;

        public BurstComponent(float power, 
            float period_min = 0.01f, 
            float period_max = 0.03f, 
            float duration = 0.4f, 
            float alpha = 0.5f) : base(true, true)
        {
            this.power = power;
            this.duration = duration;
            this.alpha = alpha;
            this.period_min = period_min;
            this.period_del = (period_max - period_min);
        }

        public override void EntityAdded(Scene scene)
        {
            base.EntityAdded(scene);
            level = scene as Level;
        }

        public override void Update()
        {
            base.Update();
            timer += Engine.DeltaTime;
            if (timer >= period)
            {
                timer = 0;
                period = period_min + Calc.Random.NextFloat(period_del);
                level.Displacement.AddBurst(Entity.Center, duration, power / 8, power, alpha, Ease.QuadOut, Ease.QuadOut);
            }
        }
    }
}
