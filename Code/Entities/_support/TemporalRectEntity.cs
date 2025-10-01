using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class TemporalRectEntity : Entity
    {
        Rectangle rect;
        Color color;
        float alpha;

        public TemporalRectEntity(Rectangle rect, Color color) : base(Vector2.Zero)
        {
            this.rect = rect;
            this.color = color;
            this.alpha = 1f;
        }

        public override void Update()
        {
            base.Update();
            alpha -= Engine.DeltaTime / 5f;
            if (alpha < 0)
                RemoveSelf();
        }

        public override void Render()
        {
            base.Render();
            Draw.Rect(rect, color * alpha);
        }

    }
}
