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
    [CustomEntity("MeliHelper/WarpZone")]
    class WarpZone : Entity
    {
        Player player;
        RoomTeleportInfo teleport_info;
        Sprite[] mass_sprites;
        Color color;
        int radius;
        float ttl, alpha;
        string sound, flag_not_appear;
        int texture_type;
        bool is_activated, is_show_cutscene;

        public WarpZone(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            texture_type = data.Int("textureType", 2);
            color = Methods.GetColorFromString(data.Attr("color", "000000")) * data.Float("opacity", 0.6f);
            radius = data.Int("radius", 16);
            flag_not_appear = data.Attr("flagNotAppear");
            is_show_cutscene = data.Bool("showCutscene");
            mass_sprites = new Sprite[5];
            for (int i = 0; i < mass_sprites.Length; i++)
            {
                Sprite sprite = GFX.SpriteBank.Create(data.Attr("sprite", "MeliHelper_WarpZone"));
                Dictionary<string, Sprite.Animation> sprite_animations_new = new Dictionary<string, Sprite.Animation>();
                foreach (var item in sprite.Animations)
                {
                    Sprite.Animation anim_old = item.Value;
                    Sprite.Animation anim_new = new Sprite.Animation();
                    anim_new.Delay = anim_old.Delay * (1f - 0.06f * i);
                    anim_new.Frames = anim_old.Frames;
                    anim_new.Goto = anim_old.Goto;
                    sprite_animations_new.Add(item.Key, anim_new);
                }
                sprite.ClearAnimations();
                foreach (var item in sprite_animations_new)
                    sprite.Animations.Add(item.Key, item.Value);

                sprite.Color = color;
                sprite.Scale = new Vector2(1f - 0.25f * i) * (radius / sprite.Width);
                mass_sprites[i] = sprite;
                Add(sprite);
            }

            teleport_info = new RoomTeleportInfo(data, room_param_name: "roomTeleport");
            sound = data.Attr("sound");
            ttl = data.Float("ttl", 20);
            alpha = 1f;
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            player = Methods.GetPlayerOnScene(scene);
            if (flag_not_appear != "" && (scene as Level).Session.GetFlag(flag_not_appear))
            {
                RemoveSelf();
                return;
            }
        }

        public override void Update()
        {
            base.Update();
            if (!is_activated)
            {
                ttl -= Engine.DeltaTime;
                if (ttl <= 0)
                {
                    RemoveSelf();
                    return;
                }

                if (ttl <= 3.78f)
                {
                    alpha = Math.Abs(1f - (ttl % 1.28f) / 0.64f);
                    foreach (var item in mass_sprites)
                        item.Color = color * alpha;
                }
            }


            if (!is_activated && Methods.PlayerIsAlive(player) && Vector2.Distance(this.Center, player.Center) <= radius)
            {
                is_activated = true;
                Depth = player.Depth + 1;

                if (_BattleCity.Field.Instance != null)
                {
                    _BattleCity.Field.Instance.SetState(BCEnum_GameState.Pause);
                    _BattleCity.Field.Instance.GetEventUI.Visible = false;
                }
                SceneAs<Level>().Add(new CutsceneWarpZone(player, this, teleport_info, color, sound, texture_type, is_show_cutscene));
            }
        }
    }
}
