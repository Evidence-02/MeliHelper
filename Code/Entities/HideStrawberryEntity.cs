using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._BattleCity._EntitiesMeli
{
    [CustomEntity("MeliHelper/HideStrawberryEntity")]
    class HideStrawberryEntity : Entity
    {
        string affect;
        bool hide_sprite, hide_light, hide_bloom, set_depth;
        int depth;

        public HideStrawberryEntity(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            affect = data.Attr("affect");
            hide_sprite = data.Bool("hideSprite");
            hide_light = data.Bool("hideLight");
            hide_bloom = data.Bool("hideBloom");
            set_depth = data.Bool("setDepth");
            depth = data.Int("depth");
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);

            // Hide all strawberries
            Level level = scene as Level;
            switch (affect)
            {
                case "Nearest":
                    List<Strawberry> list_strawberries = level.Entities.FindAll<Strawberry>();
                    if (list_strawberries.Count > 0)
                    {
                        Strawberry berry = list_strawberries.OrderBy(t => Vector2.Distance(t.Center, this.Center)).ToList()[0];
                        AffectStorby(berry);
                    }
                    break;

                // All Strawberries
                default:
                    foreach (var item in level.Entities.FindAll<Strawberry>())
                        AffectStorby(item);
                    break;
            }

            RemoveSelf();
        }

        void AffectStorby(Strawberry berry)
        {
            if (hide_sprite) berry.Visible = false;
            if (hide_light) berry.Components.RemoveAll<VertexLight>();
            if (hide_bloom) berry.Components.RemoveAll<BloomPoint>();
            if (set_depth) berry.Depth = depth;
        }
    }
}
