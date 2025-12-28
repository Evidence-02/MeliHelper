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
    [CustomEntity("MeliHelper/FakeCrystalHeart")]
    class FakeCrystalHeart : Entity
    {
        Sprite sprite;
        CustomTimer timer;
        VertexLight light;
        BloomPoint bloom;
        string flag_set, flag_not_appear, sprite_after_collect;
        bool is_collected, is_sprite_change, is_sprite_stop, is_flag_inverted;

        public FakeCrystalHeart(EntityData data, Vector2 offset) : base(data.Position + offset) 
        {
            Add(sprite = GFX.SpriteBank.Create(data.Attr("", "heartgem0")));
            Add(light = new VertexLight(Color.White, 1.0f, 8, 32));
            Add(bloom = new BloomPoint(1.0f, 24));
            timer = new CustomTimer(data.Float("burstPeriod", 2.4f));

            float hitbox = data.Float("hitboxRadius", 12);
            Add(new PlayerCollider(OnPlayer, new Hitbox(2 * hitbox, 2 * hitbox, -hitbox, -hitbox)));

            sprite_after_collect = data.Attr("spriteAfterCollect", "heartGemGhost");
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
            if (timer.Tick())
                SceneAs<Level>().Displacement.AddBurst(this.Center, 0.4f, 8, 64, 0.5f, Ease.QuadOut, Ease.QuadOut);
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
                    Add(sprite = GFX.SpriteBank.Create("sprite_after_collect"));
                }
                if (is_sprite_stop)
                    sprite.Stop();
                if (flag_set != "")
                    SceneAs<Level>().Session.SetFlag(flag_set, !is_flag_inverted);
            }
        }
    }
}
