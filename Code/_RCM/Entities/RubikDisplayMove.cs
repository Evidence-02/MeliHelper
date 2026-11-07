using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;

namespace Celeste.Mod.MeliHelper
{
    [CustomEntity("MeliHelper/RubikDisplayMove")]
    class RubikDisplayMove : Entity
    {
        RubikCube cube;
        MTexture texture;
        Color color_text_moves, color_text_unreg, color_text_title;
        Dictionary<RCMEnum_CubeState, string> dict_cube_texts;
        int w, h;
		
        Vector2 pos_title;
        int row_max, columns, colX, colY, colDX, colDY;
        
        public RubikDisplayMove(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            w = data.Width;
            h = data.Height;
            texture = GFX.Game[data.Attr("texture")];
            color_text_title = Calc.HexToColor(data.Attr("colorTextTitle",      "00FF11"));
            color_text_moves = Calc.HexToColor(data.Attr("colorTextMoves",      "00FF11"));
            color_text_unreg = Calc.HexToColor(data.Attr("colorTextMovesUnreg", "E5000B"));

            //                 title: (width / 2, data.titleY)
            // data.titleY
			columns  = data.Int("columns");
			colX = data.Int("colX");
			colY = data.Int("colY");
			colDX    = data.Int("colDX");
			colDY    = data.Int("colDY");
            pos_title = this.Position + new Vector2(w / 2, data.Int("titleY"));

            row_max = (int)(h - colY - data.Int("bottomMargin", 12)) / colDY;
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            cube = (scene as Level).Entities.FindFirst<RubikCube>();
            Methods.StayBehindPlayer(scene as Level, this);

            dict_cube_texts = new Dictionary<RCMEnum_CubeState, string>();
            foreach (RCMEnum_CubeState state in Enum.GetValues(typeof(RCMEnum_CubeState)))
            {
                string dialog_name = "EVIDENCE02_RCM_DISPLAYMOVES_" + state.ToString().ToUpper();
                if (w < 88 && Dialog.Has(dialog_name + "_SHORT"))
                    dialog_name += "_SHORT";
                dict_cube_texts[state] = Dialog.Get(dialog_name);
            }
        }

        public override void Render()
        {
            base.Render();
            texture.Draw(Position);

            // 1. Title
            /*
            string title_text = dict_cube_texts[cube.GetState];
            if (title_text.Contains("#PERC"))
                title_text.Replace("#PERC", cube.GetStatePercentage.ToString());
            if (title_text.Contains("#MOVES"))
                title_text.Replace("#MOVES", cube.GetJournalManual.GetCountMoves.ToString());
                */
            string title_text = dict_cube_texts[cube.GetState]
                .Replace("::PERC::",  cube.GetStatePercentage.ToString())
                .Replace("::MOVES::", cube.GetJournalManual.GetCountMoves.ToString());
            if (title_text != "")
            {
                int len = CustomTextController.GetTextLength(title_text);
                CustomTextController.DrawText(title_text, pos_title - new Vector2(len / 2, 0), color_text_title);
            }



            /*
            // 1. Title
            string title_text = "";
            switch (cube.GetState)
            {
                case CubeStateEnum.Scramble:   title_text = $"SCRAMBLE {cube.GetStatePercentage}%"; break;
                case CubeStateEnum.Descramble: title_text = "RETURN TO MONKE"; break;
                case CubeStateEnum.Normal:     title_text = "MOVES: " + cube.GetJournalManual.GetCountMoves; break;
                case CubeStateEnum.Solved:     title_text = ((w >= 88) ? $"SOLVED IN " : "") + cube.GetJournalManual.GetCountMoves + " MOVES!"; break;
            }
            if (title_text != "")
            {
                int len = CustomTextController.GetTextLength(title_text);
                CustomTextController.DrawText(title_text, pos_title - new Vector2(len / 2, 0), color_text_title);
            }
            */


            // 2. Moves
            if (cube.GetState == RCMEnum_CubeState.Normal || cube.GetState == RCMEnum_CubeState.Solved)
            {
                bool has_prev = false;
                int col = 0, row = 0;
                int row_count = (cube.GetJournalManual.GetListMoves.Count - 1) / columns;
                if (row_count > row_max)
                {
                    row -= row_count - row_max;
                    has_prev = true;
                }

                foreach (JournalMove move in cube.GetJournalManual.GetListMoves)
				{
					if (row >= 0)
					{
						string text = has_prev ? "..." : move.Description;
						Vector2 position = this.Position + new Vector2(colX + col * colDX, colY + row * colDY);
						Color clr = (move.isRegisteredInJournal ? color_text_moves : color_text_unreg);
                        CustomTextController.DrawText(text, position, clr);
                        has_prev = false;
                    }
                    
                    if (++col >= columns)
					{
						col = 0;
						row++;
					}
				}
				
                //CustomTextController.DrawJournal(pos_moves, color_text_moves, color_text_unreg, length_moves, cube.GetJournalManual);
			}

        }
    }
}
