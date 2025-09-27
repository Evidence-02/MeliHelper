using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class TemporalSpriteEntity : Entity
    {
        Sprite sprite;
        string anim_stop;

        public TemporalSpriteEntity(Vector2 position, string sprite_id, string anim_stop) : base(position)
        {
            Add(sprite = GFX.SpriteBank.Create(sprite_id));
            this.anim_stop = anim_stop;
        }

        public override void Update()
        {
            base.Update();
            if (sprite != null && sprite.CurrentAnimationID == anim_stop)
                RemoveSelf();
        }

    }
}
