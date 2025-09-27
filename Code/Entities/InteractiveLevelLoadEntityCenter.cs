using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    [CustomEntity("MeliHelper/InteractiveLevelLoadEntityCenter")]
    class InteractiveLevelLoadEntityCenter : Solid
    {
        Level level;
        Rectangle rect, rect_inside;
        Color color_border, color_fill, color_back;
        float val, val_add_per_shoot, val_sub, delay, delay_after_shoot;
        bool is_loaded, is_dash_affective;

        Vector2 spawnpoint;
        string level_load, dialogue_before;
        Player.IntroTypes intro_type;

        public InteractiveLevelLoadEntityCenter(EntityData data, Vector2 offset)
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

            level_load = data.Attr("levelLoad");
            dialogue_before = data.Attr("dialogueBefore");
            intro_type = (Player.IntroTypes)Enum.Parse(typeof(Player.IntroTypes), data.Attr("introTypes", "WakeUp"));
            spawnpoint = new Vector2(data.Int("spawnpointX"), data.Int("spawnpointY"));
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

            float w_full = rect_inside.Width * val / 2;
            float h_full = rect_inside.Height * val / 2;

            int w = (int)w_full;
            int h = (int)h_full;
            
            Draw.Rect(new Rectangle(rect_inside.X, rect_inside.Y, w, rect_inside.Height), color_fill);
            Draw.Rect(new Rectangle(rect_inside.X + rect_inside.Width - w, rect_inside.Y, w, rect_inside.Height), color_fill);
            if (w_full - w > 0)
            {
                Draw.Rect(new Rectangle(rect_inside.X + w, rect_inside.Y, 1, rect_inside.Height), color_fill * (w_full - w));
                Draw.Rect(new Rectangle(rect_inside.X + rect_inside.Width - w - 1, rect_inside.Y, 1, rect_inside.Height), color_fill * (w_full - w));
            }

            Draw.Rect(new Rectangle(rect_inside.X, rect_inside.Y, rect_inside.Width, h), color_fill);
            Draw.Rect(new Rectangle(rect_inside.X, rect_inside.Y + rect_inside.Height - h, rect_inside.Width, h), color_fill);
            if (h_full - h > 0)
            {
                Draw.Rect(new Rectangle(rect_inside.X, rect_inside.Y + h, rect_inside.Width, 1), color_fill * (h_full - h));
                Draw.Rect(new Rectangle(rect_inside.X, rect_inside.Y + rect_inside.Height - h - 1, rect_inside.Width, 1), color_fill * (h_full - h));
            }

            Draw.HollowRect(rect, color_border);
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
                level.Add(new CutsceneRoomTeleport(level_load, spawnpoint, intro_type, dialogue_before));
            }
        }

    }
}
