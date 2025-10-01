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

            VirtualButton vb = null;
            switch (button)
            {
                case "BattleCity_Shoot": vb = MeliHelperModule.Settings.BattleCity_Shoot.Button; break;
                case "Minesweeper_ChangeDashMode": vb = MeliHelperModule.Settings.Minesweeper_ChangeDashMode.Button; break;
            }
            if (vb != null)
            {
                // Copied code from an old project again. But this time I modified it!
                Vector2 cam = level.Camera.Position;
                float zoom_koef = Methods.GetZoomKoefHUD(level);
                Vector2 posTmp = cam + new Vector2(960, 540) / zoom_koef;
                Vector2 pos = (Position - cam + (Position - posTmp) * koef_floating) * zoom_koef;
                if (SaveData.Instance != null && SaveData.Instance.Assists.MirrorMode)
                    pos.X = 1920f - pos.X;
                Input.GuiButton(vb, Input.PrefixMode.Attached).DrawCentered(pos, Color.White * alpha, scale: size);
                // pos = Methods.CoordsToHUD(level, Position)
            }
        }


    }
}
