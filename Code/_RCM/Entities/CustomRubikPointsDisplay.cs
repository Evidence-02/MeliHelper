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
    [CustomEntity("MeliHelper/CustomRubikPointsDisplay")]
    class CustomRubikPointsDisplay : Entity
    {
        Color color_text;
        MTexture[] mass_textures;
        int w, h, length, spacing;
        
        public CustomRubikPointsDisplay(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            w = data.Width;
            h = data.Height;
            length = data.Int("digits", 7);
            spacing = data.Int("spacing", 6);
            color_text = Calc.HexToColor(data.Attr("colorText", "FFFFFF"));

            mass_textures = new MTexture[length];
            int current_points = MeliHelperModule.Instance.Session.Points;
            for (int i = 0; i < length; i++)
            {
                mass_textures[length - i - 1] = GFX.Game["Evidence02/objects_cubes/display/num" + (current_points % 10).ToString("00")];
                current_points /= 10;
                if (current_points == 0)
                    break;
            }
            this.Depth = data.Int("depth");
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            Methods.StayBehindPlayer(scene as Level, this);
        }

        public override void Render()
        {
            base.Render();
            Vector2 pos = Position;
            for (int i = 0; i < length; i++)
            {
                if (mass_textures[i] != null)
                    mass_textures[i].Draw(pos, origin: Vector2.Zero, color: color_text);
                pos.X += spacing;
            }
        }
    }
}
