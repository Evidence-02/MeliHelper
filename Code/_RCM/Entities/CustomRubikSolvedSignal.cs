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
    [CustomEntity("MeliHelper/CustomRubikSolvedSignal")]
    class CustomRubikSolvedSignal : Entity
    {
        MTexture texture;
        Vector2 texture_center;
        Rectangle rect;
        Color color;
        string cube_id;
        
        public CustomRubikSolvedSignal(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            rect = new Rectangle((int)Position.X, (int)Position.Y, (int)data.Width, (int)data.Height);
            color = Calc.HexToColor(data.Attr("color", "000000")) * data.Float("opacity", 0.6f);

            string texture = data.Attr("texture");
            if (texture != "")
            {
                this.texture = GFX.Game[texture];
                this.texture_center = Position + new Vector2(data.Width / 2, data.Height / 2);
            }

            this.cube_id = data.Attr("cubeID");
            this.Depth = data.Int("depth");
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            if (!MeliHelperModule.Instance.Session.CubeInfo.ContainsKey(cube_id) || !MeliHelperModule.Instance.Session.CubeInfo[cube_id].isCubeSolved)
                RemoveSelf();
        }

        public override void Render()
        {
            base.Render();
            Draw.Rect(rect, color);
            if (texture != null)
                texture.DrawCentered(texture_center);
        }
    }
}
