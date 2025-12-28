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
    [CustomEntity("MeliHelper/FakeKey")]
    class FakeKey : Entity
    {
        Sprite sprite;
        string flag_set, flag_not_appear;
        bool is_collected, is_sprite_stop, is_flag_inverted;

        public FakeKey(EntityData data, Vector2 offset) : base(data.Position + offset) 
        {
            Add(sprite = GFX.SpriteBank.Create("key"));
            Add(new PlayerCollider(OnPlayer, new Hitbox(14, 14, -7, -7)));
            
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

        public void OnPlayer(Player player)
        {
            if (!is_collected)
            {
                is_collected = true;
                if (is_sprite_stop)
                    sprite.Stop();
                if (flag_set != "")
                    SceneAs<Level>().Session.SetFlag(flag_set, !is_flag_inverted);
            }
        }
    }
}
