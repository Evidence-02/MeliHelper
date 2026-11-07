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
    [CustomEntity("MeliHelper/SeedLabel")]
    class SeedLabel : Entity
    {
        Level level;
        Vector2 pos;
        Color color_text, color_stroke;
        float size;
        string seed_cube, seed_current;
        bool is_seeds_different;

        public SeedLabel(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            Tag = Tags.HUD;
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
            
            Level level = scene as Level;
            RubikCube cube = level.Entities.FindAll<RubikCube>().FirstOrDefault();
            seed_cube = (cube != null && MeliHelperModule.Instance.Session.CubeInfo.ContainsKey(cube.GetCubeID)) 
                 ? MeliHelperModule.Instance.Session.CubeInfo[cube.GetCubeID].GetSeed
                 : "";
            UpdateSeed();
        }

        public void UpdateSeed()
        {
            seed_current = (MeliHelperModule.Instance.Session.HideSeed) ? Dialog.Get("EVIDENCE02_RCM_SEED_HIDDEN") : SeedController.GetCurrentSeed();
            is_seeds_different = (seed_cube != "" && seed_current != Dialog.Get("EVIDENCE02_RCM_SEED_HIDDEN") && seed_cube != seed_current);
        }

        public override void Render()
        {
            base.Render();
            Vector2 pos = Methods.CoordsToHUD(level, Position);
            if (is_seeds_different)
            {
                ActiveFont.DrawOutline(Dialog.Get("EVIDENCE02_RCM_SEED_CUBE") + ": " + seed_cube, pos, justify: new Vector2(0.5f), scale: new Vector2(size),
                    color: color_text, stroke: 2, strokeColor: color_stroke);
                ActiveFont.DrawOutline(Dialog.Get("EVIDENCE02_RCM_SEED_MAP" ) + ": " + seed_current, pos + new Vector2(0, 40), justify: new Vector2(0.5f), scale: new Vector2(0.7f * size),
                    color: color_text, stroke: 2, strokeColor: color_stroke);
            }
            else
            {
                ActiveFont.DrawOutline(Dialog.Get("EVIDENCE02_RCM_SEED"     ) + ": " + seed_current, pos, justify: new Vector2(0.5f), scale: new Vector2(size),
                    color: color_text, stroke: 2, strokeColor: color_stroke);
            }
            
            // TEST
            //ActiveFont.DrawOutline($"ACTUAL SEED: " + SeedController.GetCurrentSeed(), pos + new Vector2(0, -150), justify: new Vector2(0.5f), scale: new Vector2(0.8f * size),
            //    color: color_text, stroke: 2, strokeColor: color_stroke);
            //ActiveFont.DrawOutline($"ACTUAL ID: " + SeedController.GetCurrentSeedID(), pos + new Vector2(0, -110), justify: new Vector2(0.5f), scale: new Vector2(0.8f * size),
            //    color: color_text, stroke: 2, strokeColor: color_stroke);
        }
    }
}
