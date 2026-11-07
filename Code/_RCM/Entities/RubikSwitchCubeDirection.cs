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
    [CustomEntity("MeliHelper/RubikSwitchCubeDirection")]
    class RubikSwitchCubeDirection : Entity
    {
        Level level;
        VirtualButton button;
        RubikCube cube;
        Color color_text, color_stroke;
        float size;
        bool saved_check;

        public RubikSwitchCubeDirection(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            Tag = Tags.HUD;
            button = Input.Talk;
            color_text = Color.White;
            color_stroke = Color.Black;
            size = data.Float("size", 0.7f);
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = scene as Level;
        }
        
        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            cube = (scene as Level).Entities.FindFirst<RubikCube>();
        }

        public override void Update()
        {
            base.Update();
            saved_check = button.Check;
            cube.isRotateClockwise = !saved_check;
        }

        public override void Render()
        {
            base.Render();

            string str = Dialog.Get("EVIDENCE02_RCM_CUBEDIRECTION_" + ((cube == null) ? "NONE" : (cube.isRotateClockwise) ? "CLOCKWISE" : "OTHERWISE"));
            float alpha = saved_check ? 1f : 0.3f;
            float size_koef = saved_check ? 1f : 0.8f;
            Vector2 pos = Methods.CoordsToHUD(level, Position);
            Input.GuiButton(button, Input.PrefixMode.Attached).DrawCentered(pos, color: Color.White * alpha, scale: size * size_koef);
            ActiveFont.DrawOutline(str, pos + new Vector2(40, 0), justify: new Vector2(0, 0.5f), scale: new Vector2(size),
                color: color_text * alpha, stroke: 2, strokeColor: color_stroke * alpha);
        }
    }
}
