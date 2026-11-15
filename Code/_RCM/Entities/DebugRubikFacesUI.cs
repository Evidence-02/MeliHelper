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
    class DebugRubikFacesUI : Entity
    {
        RubikCube cube;

        public DebugRubikFacesUI(RubikCube cube) : base()
        {
            this.cube = cube;
            Tag = Tags.HUD | Tags.PauseUpdate;
        }

        public override void Update()
        {
            base.Update();
            Visible = MeliHelperModule.Settings.DebugTools.RubiksShowDebugUI != RCMEnum_ShowUI.None;
        }

        public override void Render()
        {
            base.Render();

            int cnt_solved = 0;
            float dx = 0;
            float dy = 10;
            foreach (Face face in cube.GetListFaces)
            {
                face.RenderUI(ref dx, ref dy);
                if (face.isSolved())
                    cnt_solved++;
            }
            
            ActiveFont.DrawOutline(text: $"SOLVED: {cnt_solved}/{cube.GetListFaces.Count}", 
                position: new Vector2(dx + 10, dy + 5), justify: Vector2.Zero, scale: new Vector2(0.6f), color: Color.White, stroke: 2, strokeColor: Color.Black);
        }
    }
}
