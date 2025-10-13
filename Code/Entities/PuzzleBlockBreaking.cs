using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Celeste.SummitCheckpoint;

namespace Celeste.Mod.MeliHelper
{
    [CustomEntity("MeliHelper/PuzzleBlockBreaking")]
    class PuzzleBlockBreaking : Entity
    {
        Strawberry strawberry;
        Vector2 strawberry_pos;
        int count_blocks, count_blocks_max;
        string flag_on_solve, action;
        bool is_solved;

        MTexture texture;
        Vector2 texture_center, text_center;
        Color text_color;
        float text_scale, shake_power, shake_del, shake_sin;

        public PuzzleBlockBreaking(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            Tag = Tags.TransitionUpdate | Tags.PauseUpdate | TagsExt.SubHUD;

            this.count_blocks_max = data.Int("blocksNeedToBreak", 40);
            this.count_blocks = 0;
            this.flag_on_solve = data.Attr("flagOnSolve");
            this.action = data.Attr("action", "UnlockStrawberry");

            string textureGUI = data.Attr("textureGUI", @"Evidence02/puzzle_sugoma");
            if (textureGUI != "")
                this.texture = GFX.Gui[textureGUI];
            this.texture_center = new Vector2(data.Float("textureX", 920), data.Float("textureY", 60));
            this.text_center = texture_center + new Vector2(data.Float("textDX"), data.Float("textDY"));
            this.text_color = Methods.GetColorFromString(data.Attr("color", "FFFFFF"));
            this.text_scale = data.Float("textScale", 0.9f);
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            Level level = scene as Level;
            is_solved = (flag_on_solve != "" && level.Session.GetFlag(flag_on_solve));
            
            strawberry = level.Entities.FindFirst<Strawberry>();
            if (strawberry != null)
            {
                strawberry_pos = strawberry.Position;
                if (!is_solved)
                    strawberry.Position += new Vector2(0, -10000);
            }

            Load();
        }
        
        public override void Removed(Scene scene)
        {
            base.Removed(scene);
            Unload();
        }

        public override void Update()
        {
            base.Update();
            if (shake_del > 0)
            {
                shake_sin += 48f * Engine.DeltaTime;
                shake_del -= shake_power * Engine.DeltaTime;
                if (shake_del <= 0) shake_del = 0;
            }
        }

        public override void Render()
        {
            base.Render();
            if (texture != null)
                texture.DrawCentered(texture_center);
            ActiveFont.Draw(
                text: count_blocks + " / " + count_blocks_max,
                position: text_center + new Vector2(0, shake_del * (float)Math.Sin(shake_sin)),
                justify: new Vector2(0.5f),
                scale: new Vector2(text_scale),
                color: Color.White);
        }

        public void SetSolved()
        {
            is_solved = true;
            if (flag_on_solve != "")
                SceneAs<Level>().Session.SetFlag(flag_on_solve, true);
            strawberry.Position = strawberry_pos;
        }

        public void SetShake(float power, float time)
        {
            shake_power = power / time;
            shake_del = power;
        }



        #region Load
        
        static bool is_loaded;

        static void Load()
        {
            if (!is_loaded)
            {
                is_loaded = true;
                On.Celeste.DashBlock.Break_Vector2_Vector2_bool_bool += onDashBlockBreak;
            }
        }

        static void Unload()
        {
            if (is_loaded)
            {
                is_loaded = false;
                On.Celeste.DashBlock.Break_Vector2_Vector2_bool_bool -= onDashBlockBreak;
            }
        }

        static void onDashBlockBreak(On.Celeste.DashBlock.orig_Break_Vector2_Vector2_bool_bool orig, DashBlock self, 
            Vector2 from, Vector2 direction, bool playSound, bool playDebrisSound)
        {
            orig(self, from, direction, playSound, playDebrisSound);
            
            Level level = self.Scene as Level;
            SugomaPuzzleEntity puzzle = level.Entities.FindFirst<SugomaPuzzleEntity>();
            if (puzzle != null && !puzzle.is_solved)
            {
                puzzle.SetShake(32f, 0.4f);
                puzzle.count_blocks++;
                if (puzzle.count_blocks >= puzzle.count_blocks_max)
                {
                    puzzle.SetSolved();
                    puzzle.Add(new Coroutine(CoroutineWeaponSetVisible(level, puzzle)));
                }
            }
        }

        static IEnumerator CoroutineWeaponSetVisible(Level level, SugomaPuzzleEntity puzzle)
        {
            Audio.Play(SFX.game_01_birdbros_thrust);
            Player player = level.Tracker.GetEntity<Player>();
            if (Methods.PlayerIsAlive(player))
                Methods.PlayerLock(player, true);
            level.Shake(0.3f);

            // Unlock storby
            Celeste.Freeze(0.1f);
            if (puzzle.strawberry != null)
            {
                for (int i = 0; i < 12; i++)
                {
                    float angle = i * (float)Math.PI / 6;
                    Vector2 direction = Calc.AngleToVector(angle, 1f);
                    SlashFx.Burst(puzzle.strawberry_pos + 10 * direction, angle);
                }
            }

            Celeste.Freeze(0.05f);
            level.Add(new ConfettiRenderer(puzzle.strawberry_pos));
            puzzle.strawberry.Position = puzzle.strawberry_pos;

            // Unlock place for strawberry
            DashBlock block = level.Entities.FindAll<DashBlock>().FirstOrDefault(t => t.CollidePoint(puzzle.strawberry.Center));
            if (block != null)
                InteractionController.ActivateBlock(block, puzzle.strawberry);
            
            
            if (Methods.PlayerIsAlive(player))
                Methods.PlayerLock(player, false);
            yield break;
        }


        #endregion

    }
}
