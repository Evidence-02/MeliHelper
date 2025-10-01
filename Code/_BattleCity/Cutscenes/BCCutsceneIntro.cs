using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    class BCCutsceneIntro : CutsceneEntity
    {
        Field field;
        Player player;
        float height_gray;
        string name;
        bool is_show_name;

        public BCCutsceneIntro(Field field, Player player, string name) : base()
        {
            this.field = field;
            this.player = player;
            this.name = name;
            this.height_gray = 0;
            //Depth = DepthController.BC_FIELD_BACKGROUND_INTRO;
            Tag = Tags.HUD;
        }

        public override void OnBegin(Level level)
        {
            Methods.PlayerLock(player);
            field.SetState(BCEnum_GameState.Pause);
            Add(new Coroutine(Cutscene(level)));
        }

        private IEnumerator Cutscene(Level level)
        {
            yield return 0.6f;

            // 1. Move line
            while (height_gray < 540)
            {
                height_gray += 1440 * Engine.DeltaTime;
                yield return null;
            }
            height_gray = 540;

            // 2. Show name
            Audio.Play(SoundController.BC_STARTUP);
            is_show_name = true;
            yield return 2.4f;
            is_show_name = false;

            // 3. Move back
            while (height_gray > 0)
            {
                height_gray -= 1440 * Engine.DeltaTime;
                yield return null;
            }

            
            // cutscene end
            EndCutscene(level);
        }


        public override void OnEnd(Level level)
        {
            Methods.PlayerLock(player, false);
            field.SetState(BCEnum_GameState.Normal);
        }


        public override void Render()
        {
            base.Render();
            if (height_gray > 0)
            {
                Draw.Rect(-2, -2, 1924, height_gray + 2, Color.Gray);
                Draw.Rect(-2, 1080 - height_gray, 1924, height_gray + 2, Color.Gray);
            }


            if (is_show_name)
            {
                FontControllerNES.ShowTextNES(name, new Vector2(920 - FontControllerNES.GetTextWidth(name) / 2, 500), Color.Black, TextAlignment.Left);
            }
        }

    }
}
