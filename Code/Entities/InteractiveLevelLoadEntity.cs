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
    [CustomEntity("MeliHelper/InteractiveLevelLoadEntity")]
    class InteractiveLevelLoadEntity : Solid
    {
        Level level;
        Rectangle rect, rect_inside;
        Color color_border, color_fill, color_back;
        float val, val_add_per_shoot, val_sub, delay, delay_after_shoot;
        bool is_loaded, is_dash_affective;

        Vector2 room_spawnpoint;
        string room_name, dialogue_before;
        Player.IntroTypes intro_type;

        // ???
        Color clr_text;
        string text;

        public InteractiveLevelLoadEntity(EntityData data, Vector2 offset)
            : base(data.Position + offset, data.Width, data.Height, true)
        {
            rect = new Rectangle((int)Position.X, (int)Position.Y, data.Width, data.Height);
			rect_inside = new Rectangle((int)Position.X + 1, (int)Position.Y + 1, data.Width - 2, data.Height - 2);
            color_border = Methods.GetColorFromString(data.Attr("colorBorder"));
            color_back = Methods.GetColorFromString(data.Attr("colorBack"));
            color_fill = Methods.GetColorFromString(data.Attr("colorFill"));
            val_add_per_shoot = data.Float("valueAdd", 0.1f); 
            val_sub = data.Float("valueSubPerSecond", 0.07f);   
            delay_after_shoot = data.Float("delayBeforeSub", 0.7f);
            is_dash_affective = data.Bool("dashAffective", true);

            room_name = data.Attr("levelLoad");
            room_spawnpoint = new Vector2(data.Int("spawnpointX"), data.Int("spawnpointY"));
            intro_type = (Player.IntroTypes)Enum.Parse(typeof(Player.IntroTypes), data.Attr("introTypes", "WakeUp"));
            dialogue_before = data.Attr("dialogueBefore");

            // ???
            text = data.Attr("text");
            clr_text = Methods.GetColorFromString(data.Attr("colorText"));
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = scene as Level;
            if (is_dash_affective)
                OnDashCollide += onDashCollide;
        }

        public override void Update()
        {
            base.Update();
            if (is_loaded)
                return;

            if (delay > 0)
                delay -= Engine.DeltaTime;
            else if (val > 0)
            {
                val -= val_sub * Engine.DeltaTime;
                if (val < 0) val = 0;
            }
        }

        public override void Render()
        {
            base.Render();
            Draw.Rect(rect, color_back);
            Draw.Rect(new Rectangle(rect_inside.X, rect_inside.Y, (int)(rect_inside.Width * val), rect_inside.Height), color_fill);
            Draw.HollowRect(rect, color_border);

            if (text != "")
                ActiveFont.Draw(text, 
                    position: new Vector2(rect.Center.X, rect.Center.Y), 
                    justify: new Vector2(0.5f), 
                    scale: new Vector2(0.3f), 
                    color: clr_text);
        }

        protected virtual DashCollisionResults onDashCollide(Player player, Vector2 dir)
        {
            IncValue();
            return DashCollisionResults.Bounce;
        }

        public void IncValue()
        {
            if (is_loaded)
                return;

            delay = delay_after_shoot;
            val += val_add_per_shoot;
            if (val >= 1)
            {
                val = 1;
                is_loaded = true;
                level.Add(new CutsceneRoomTeleport(room_name, room_spawnpoint, intro_type, dialogue_before));
            }
        }

    }
}
