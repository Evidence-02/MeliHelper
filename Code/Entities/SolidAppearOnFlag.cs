using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._Lani
{
    [CustomEntity("MeliHelper/SolidAppearOnFlag")]
    class SolidAppearOnFlag : Entity
    {
        Level level;
        Solid solid;
        TileGrid tileGrid;
        string flag, sound_appear, sound_disappear;
        float time_appear, time_disappear;
        char tiletype;
        bool blendin, is_visible, was_visible;
        float alpha;

        public SolidAppearOnFlag(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            solid = new Solid(data.Position + offset, data.Width, data.Height, true);
            flag = data.Attr("flag");
            time_appear = data.Float("appearTime", 1f);
            time_disappear = data.Float("disappearTime", 0.3f);
            sound_appear = data.Attr("appearSound");
            sound_disappear = data.Attr("disappearSound");
            tiletype = data.Char("tiletype", '3');
            blendin = data.Bool("blendin", false);
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            level = scene as Level;
            //if (flag == "") RemoveSelf();

            is_visible = level.Session.GetFlag(flag);
            tileGrid = Methods.CreateTiles(level, solid, tiletype, blendin);
            SetVisible(is_visible, is_play_sound: false, is_forced: true);
            SetAlpha(is_visible ? 1 : 0);
            was_visible = is_visible;
        }

        public override void Update()
        {
            base.Update();
            bool should_be_visible = level.Session.GetFlag(flag);
            if (should_be_visible != was_visible)
                SetVisible(should_be_visible);
            was_visible = should_be_visible;


            if (is_visible && alpha < 1)
                SetAlpha(alpha + Engine.DeltaTime / time_appear);
            else if (!is_visible && alpha > 0)
                SetAlpha(alpha - Engine.DeltaTime / time_disappear);
        }

        void SetVisible(bool value, bool is_play_sound = true, bool is_forced = false)
        {
            if (!is_forced && is_visible == value)
                return;

            this.is_visible = value;
            if (value)
            {
                level.Add(solid);
                if (is_play_sound && sound_appear != "") Audio.Play(sound_appear);
                //Audio.Play(SFX.appea);
            }
            else
            {
                level.Remove(solid);
                if (is_play_sound && sound_disappear != "") Audio.Play(sound_disappear);
                //Audio.Play(SFX.disappea);
            }
        }

        void SetAlpha(float value)
        {
            this.alpha = Math.Max(0, Math.Min(1, value));
            tileGrid.Color = Color.White * alpha;
        }

        //public override void Render()
        //{
        //    base.Render();
        //    ActiveFont.Draw(alpha.ToString("0.00"), this.Center, Color.White);
        //}
    }
}
