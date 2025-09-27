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
    [CustomEntity("MeliHelper/MinesweeperFace")]
    class MinesweeperFace : Entity
    {
        MTexture texture, texture_sad, texture_win;
        string state;

        public MinesweeperFace(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            texture     = GFX.Game[data.Attr("textureNormal")];
            texture_sad = GFX.Game[data.Attr("textureGameover")];
            texture_win = GFX.Game[data.Attr("textureWin")];
        }

        public override void Render()
        {
            base.Render();
            switch (state)
            {
                case "Sad": texture_sad.DrawCentered(Center); break;
                case "Win": texture_win.DrawCentered(Center); break;
                default: texture.DrawCentered(Center); break;
            }
        }

        public void SetState(string state)
        {
            this.state = state;
        }

    }
}
