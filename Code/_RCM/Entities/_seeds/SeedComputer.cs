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
    [CustomEntity("MeliHelper/SeedComputer")]
    class SeedComputer : Entity
    {
        MTexture texture;
        Color color;
        TalkComponent talk_component;
        SeedComputerUI ui_component;
        Vector2 pos_texture, pos_text;
        string seed;
        bool is_hidden;

        public SeedComputer(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            texture = GFX.Game["Evidence02/objects_cubes/seed_computer/idle00"];
            color = Calc.HexToColor(data.Attr("seedColor", "FFBF7F"));
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            DashController.Load();
            SeedController.Initialize();
            is_hidden = MeliHelperModule.Instance.Session.HideSeed;
            seed = (is_hidden) ? "----" : SeedController.GetCurrentSeed();

            pos_texture = Position;
            pos_text    = Position + new Vector2(-CustomTextController.GetTextLength(seed, size: 2) / 2, -5);
            scene.Add(ui_component = new SeedComputerUI());

            if (!is_hidden)
                Add(talk_component = new TalkComponent(new Rectangle(-48, -32, 96, 64), new Vector2(0, -12), player => {
                    ui_component.Activate(player);
                })
                { PlayerMustBeFacing = true });
        }

        public void UpdateSeed()
        {
            is_hidden = MeliHelperModule.Instance.Session.HideSeed;
            seed = (is_hidden) ? "----" : SeedController.GetCurrentSeed();
            pos_text = Position + new Vector2(-CustomTextController.GetTextLength(seed, size: 2) / 2, -5);
            talk_component.UI.Active  = !is_hidden;
            talk_component.UI.Visible = !is_hidden;
            talk_component.Active     = !is_hidden;
            talk_component.Visible    = !is_hidden;
            //ui_component.Active     = !is_hidden;
            //ui_component.Visible    = !is_hidden;
            ui_component.UpdateSeed();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Render()
        {
            base.Render();
            texture.DrawCentered(pos_texture);
            CustomTextController.DrawText(seed, pos_text, color, size: 2);
        }




    }
}
