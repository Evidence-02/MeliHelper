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
    [CustomEntity("MeliHelper/CustomEntityPortal")] 
    class CustomEntityPortal : Entity
    {
        Level level;
        CustomEntityPortal other_portal;
        string id, type;
        int radius;
        bool is_only_once, is_ignore_player;
        //string debug_text;

        public CustomEntityPortal(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            id = data.Attr("portalID");
            type = data.Attr("portalType", "in");
            radius = data.Int("radius", 16);
            is_only_once = data.Bool("onlyOnce");
            is_ignore_player = data.Bool("ignorePlayer");

            Sprite sprite = GFX.SpriteBank.Create(data.Attr("sprite", "MeliHelper_WarpZone"));
            sprite.Color = Methods.GetColorFromString(data.Attr("color", "000000")) * data.Float("opacity", 0.6f);
            Add(sprite);
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            level = scene as Level;
            other_portal = level.Entities.FindAll<CustomEntityPortal>().FirstOrDefault(t => t.id == id && t != this);

        }

        public override void Update()
        {
            base.Update();
            if (type == "In" && other_portal != null)
            {
                Entity entity = level.Entities.FirstOrDefault(t => 
                    !(t is CustomEntityPortal) &&
                    //(t is Player || t is TheoCrystal || t is _Lani.LaniThrowableObject) &&
                    Vector2.Distance(t.Center, this.Center) <= radius && !(is_ignore_player && t is Player));
                if (entity != null)
                {
                    entity.Center = other_portal.Center;
                    if (other_portal.is_only_once)
                        other_portal.RemoveSelf();

                    //type = "nothing";
                    //debug_text = entity.GetType().ToString();
                    if (is_only_once)
                        RemoveSelf();
                }
            }
        }

        //public override void Render()
        //{
        //    base.Render();
        //    if (debug_text != "")
        //        ActiveFont.Draw(debug_text, this.Center, new Vector2(0.5f), new Vector2(0.2f), Color.White);
        //}
    }
}
