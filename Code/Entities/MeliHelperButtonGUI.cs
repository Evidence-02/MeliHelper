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
    [CustomEntity("MeliHelper/MeliHelperButtonGUI")]
    class MeliHelperButtonGUI : Entity
    {
        Level level;
        Player player;
        string button;
        float size, koef_floating, alpha, dist_appear, dist_disappear;
        bool is_appear_just_like_core_message;

        public MeliHelperButtonGUI(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            button = data.Attr("button");
            size = data.Float("size", 1f);
            koef_floating = data.Float("koefFloating", 0.2f);

            dist_appear = data.Float("distanceAppear", 66);
            dist_disappear = data.Float("distanceDisappear", 114);
            is_appear_just_like_core_message = data.Bool("appearJustLikeCoreMessage");
            alpha = 1f;

            Tag = Tags.HUD;
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            level = scene as Level;
            player = level.Tracker.GetEntity<Player>();
        }

        public override void Update()
        {
            base.Update();

            if (is_appear_just_like_core_message && Methods.PlayerIsAlive(player))
            {
                float dist = Math.Abs(this.Center.X - player.Center.X);
                alpha = (dist >= dist_disappear) ? 0 : (dist < dist_appear) ? 1f 
                    : (1f - (dist - dist_appear) / (dist_disappear - dist_appear));
            }
        }

        public override void Render()
        {
            base.Render();

            VirtualButton vb = Methods.GetButtonBinding(button).Button;
            if (vb != null)
            {
                // Copied code from an old project again. But this time I modified it!
                Vector2 pos = Methods.CoordsToHUDwithFloating(level, Position, koef_floating);
                Input.GuiButton(vb, Input.PrefixMode.Attached).DrawCentered(pos, Color.White * alpha, scale: size);
                // pos = Methods.CoordsToHUD(level, Position)
            }
        }


    }
}
