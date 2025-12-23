using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._Minesweeper
{
    [CustomEntity("MeliHelper/MinesweeperFlagGUI")]
    class MinesweeperFlagGUI : Entity
    {
        Level level;
        Player player;
        string message_on_flag_mode, message_on_normal_mode;
        float size, koef_floating, alpha, dist_appear, dist_disappear;
        bool is_appear_just_like_core_message;

        public MinesweeperFlagGUI(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            size = data.Float("size", 1f);
            koef_floating = data.Float("koefFloating", 0.2f);

            dist_appear = data.Float("distanceAppear", 66);
            dist_disappear = data.Float("distanceDisappear", 114);
            is_appear_just_like_core_message = data.Bool("appearJustLikeCoreMessage");
            alpha = 1f;

            message_on_flag_mode   = Dialog.Get(data.Attr("messageOnFlagMode"));
            message_on_normal_mode = Dialog.Get(data.Attr("messageOnNormalMode"));
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

            // Copied code from MeliHelperButtonGUI. Made a new function in Methods?
            Vector2 pos = Methods.CoordsToHUDwithFloating(level, Position, koef_floating);
            string text = (MeliHelperModule.Instance.Session.Minesweeper_CellMarker == Minesweeper_CellMark.Flag
                ? message_on_flag_mode : message_on_normal_mode);
            ActiveFont.DrawOutline(text: text, position: pos, justify: new Vector2(0.5f), scale: new Vector2(size), color: Color.White * alpha,
                stroke: 2, strokeColor: Color.Black * alpha);
        }


    }
}
