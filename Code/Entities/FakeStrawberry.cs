using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    [CustomEntity("MeliHelper/FakeStrawberry")]
    class FakeStrawberry : Entity
    {
        Sprite sprite;
        VertexLight light;
        BloomPoint bloom;
        Vector2 pos_start;
        string flag_set, flag_not_appear;
        float sin;
        bool is_collected, is_sprite_change, is_sprite_stop, is_flag_inverted;

        public FakeStrawberry(EntityData data, Vector2 offset) : base(data.Position + offset) 
        {
            pos_start = Position;
            Add(sprite = GFX.SpriteBank.Create("strawberry"));
            Add(light = new VertexLight(Color.White, 1.0f, 8, 32));
            Add(bloom = new BloomPoint(1.0f, 24));
            Add(new PlayerCollider(OnPlayer, new Hitbox(14, 14, -7, -7)));

            is_sprite_change = data.Bool("spriteChange");
            is_sprite_stop = data.Bool("spriteStop");
            is_flag_inverted = data.Bool("flagInverted", false);
            flag_not_appear = data.Attr("notAppearWhenFlag");
            flag_set = data.Attr("setFlagOnCollect");
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            if (flag_not_appear != "" && (scene as Level).Session.GetFlag(flag_not_appear))
                RemoveSelf();
            else if (flag_set != "")
                (scene as Level).Session.SetFlag(flag_set, is_flag_inverted);
        }

        public override void Update()
        {
            base.Update();
            if (!is_collected)
            {
                sin += 3f * Engine.DeltaTime;
                Position = pos_start + new Vector2(0, 2 * (float)Math.Sin(sin));
            }
        }

        public void OnPlayer(Player player)
        {
            if (!is_collected)
            {
                is_collected = true;
                //Remove(light);
                Remove(bloom);
                if (is_sprite_change)
                {
                    Remove(sprite);
                    Add(sprite = GFX.SpriteBank.Create("ghostberry"));
                }
                if (is_sprite_stop)
                    sprite.Stop();
                if (flag_set != "")
                    SceneAs<Level>().Session.SetFlag(flag_set, !is_flag_inverted);
            }
        }
    }
}
