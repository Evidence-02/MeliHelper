using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class SeedComputerUI : Entity
    {
        protected const int BORDER_TOP = 410 - 88;
        protected const int BORDER_BTM = 410 + 88;

        Level level;
        Player player;
        MTexture texture_top, texture_center, texture_bottom;
        SeedComputerUIElement[] mass_elements;
        CustomVirtualButtonChecker btn_dash, btn_talk;
        int id_element;
        float alpha;
        bool is_visible;

        public SeedComputerUI()
        {
            Tag = Tags.HUD | Tags.PauseUpdate;
            Active = false;
            Visible = false;
            btn_dash = new CustomVirtualButtonChecker(Input.Dash);
            btn_talk = new CustomVirtualButtonChecker(Input.Talk);

            MTexture texture = GFX.Gui["Evidence02/rcm/window"];
            texture_top    = texture.GetSubtexture(0, 0,          texture.Width, BORDER_TOP);
            texture_center = texture.GetSubtexture(0, BORDER_TOP, texture.Width, BORDER_BTM - BORDER_TOP);
            texture_bottom = texture.GetSubtexture(0, BORDER_BTM, texture.Width, 1080 - BORDER_BTM);
        }
        
        public void Activate(Player player)
        {
            Active = true;
            Visible = true;
            is_visible = true;
            id_element = 0;
            UpdateSeed();
            //UpdateElements();
            btn_dash.Clear();
            btn_talk.Clear();
            this.player = player;
            Methods.PlayerLock(player, true);
        }

        public void Deactivate()
        {
            Active = false;
            Visible = false;
            is_visible = false;
            if (player != null && !player.Dead)
                Methods.PlayerLock(player, false);
            player = null;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = scene as Level;

            int seed_length = SeedController.GetCurrentSeed().Length;
            mass_elements = new SeedComputerUIElement[seed_length + 3];
            for (int i = 0; i < seed_length; i++)
                mass_elements[i] = new SeedComputerUILetter(new Vector2(1920 / 2 + 250 * (i - 0.5f * seed_length + 0.5f), 1080 / 2 - 130));
            mass_elements[seed_length + 0] = new SeedComputerUICheckbox(new Vector2(1920 / 2 - 220, 1080 / 2 +  65), Dialog.Get("EVIDENCE02_RCM_SEEDCOMPUTER_BUTTON_REROLL"), MeliHelperModule.Instance.Session.RerollSeedOnDeath, scale: 0.8f);
            mass_elements[seed_length + 1] = new SeedComputerUIButton  (new Vector2(1920 / 2,       1080 / 2 + 165), UIButtonActionEnum.Save, Dialog.Get("EVIDENCE02_RCM_SEEDCOMPUTER_BUTTON_SAVE")  , scale: 1.2f);
            mass_elements[seed_length + 2] = new SeedComputerUIButton  (new Vector2(1920 / 2,       1080 / 2 + 245), UIButtonActionEnum.Exit, Dialog.Get("EVIDENCE02_RCM_SEEDCOMPUTER_BUTTON_CANCEL"), scale: 1.05f);
            for (int i = 0; i < mass_elements.Length; i++)
                mass_elements[i].SetID(i);
            UpdateSeed();
        }
        
        public override void Update()
        {
            base.Update();
            if (level.Paused)
            {
                Deactivate();
                return;
            }

            if (is_visible)
            {
                if (alpha < 0.3f) alpha += Engine.DeltaTime / 0.6f;
            }
            else
            {
                if (alpha > 0) alpha -= Engine.DeltaTime / 0.4f;
            }
            
            if (Input.MenuUp.Pressed   ) PressAction(UIButtonEnum.Up);
            if (Input.MenuDown.Pressed ) PressAction(UIButtonEnum.Down);
            if (Input.MenuLeft.Pressed ) PressAction(UIButtonEnum.Left);
            if (Input.MenuRight.Pressed) PressAction(UIButtonEnum.Right);
            if (btn_dash.OhItsReallyFuckingPressedIsntIt()) PressAction(UIButtonEnum.Dash);
            if (btn_talk.OhItsReallyFuckingPressedIsntIt()) PressAction(UIButtonEnum.Talk);
            
            foreach (SeedComputerUIElement letter in mass_elements)
                letter.Update();
        }

        void PressAction(UIButtonEnum btn)
        {
            SeedComputerUIElement e = mass_elements[id_element];
            UIButtonActionEnum action = UIButtonActionEnum.None;
            switch (btn)
            {
                case UIButtonEnum.Up:    action = e.PressUp();    break;
                case UIButtonEnum.Down:  action = e.PressDown();  break;
                case UIButtonEnum.Left:  action = e.PressLeft();  break;
                case UIButtonEnum.Right: action = e.PressRight(); break;
                case UIButtonEnum.Dash:  action = e.PressDash();  break;
                case UIButtonEnum.Talk:  action = e.PressTalk();  break;
            }

            switch (action)
            {
                case UIButtonActionEnum.GoPrev:
                    if (--id_element < 0) id_element += mass_elements.Length;
                    UpdateElements();
                    break;
                    
                case UIButtonActionEnum.GoNext:
                    if (++id_element >= mass_elements.Length) id_element -= mass_elements.Length;
                    UpdateElements();
                    break;

                case UIButtonActionEnum.GoTo0:
                    id_element = 0;
                    UpdateElements();
                    break;

                case UIButtonActionEnum.Save:
                    Deactivate();

                    string seed_new = "";
                    foreach (SeedComputerUIElement elem in mass_elements)
                        if (elem is SeedComputerUILetter)
                            seed_new += (elem as SeedComputerUILetter).GetChar;
                    SeedController.SetCurrentSeed(seed_new);
                    SeedController.UpdateSeedEntities(level);

                    SeedComputerUICheckbox cb = (SeedComputerUICheckbox)mass_elements.FirstOrDefault(t => t is SeedComputerUICheckbox);
                    if (cb != null) MeliHelperModule.Instance.Session.RerollSeedOnDeath = cb.GetValue;
                    break;

                case UIButtonActionEnum.Exit:
                    Deactivate();
                    break;
            }
        }

        public void UpdateSeed()
        {
            string seed = SeedController.GetCurrentSeed();
            for (int i = 0; i < seed.Length; i++)
                (mass_elements[i] as SeedComputerUILetter).SetChar(seed[i]);

            SeedComputerUICheckbox cb = (SeedComputerUICheckbox)mass_elements.FirstOrDefault(t => t is SeedComputerUICheckbox);
            if (cb != null) cb.GetValue = MeliHelperModule.Instance.Session.RerollSeedOnDeath;

            UpdateElements();
        }

        void UpdateElements()
        {
            for (int i = 0; i < mass_elements.Length; i++)
                mass_elements[i].isUnderCursor = (i == id_element);
        }



        public override void Render()
        {
            base.Render();
            Draw.Rect(-1, -1, 1922, 1082, Color.Black * alpha);
            texture_center.Draw(new Vector2(0, BORDER_TOP));


            //texture.DrawCentered(new Vector2(1920 / 2, 1080 / 2), Color.White, MeliHelperModule.Settings.SeedTextureSize);
            foreach (SeedComputerUIElement letter in mass_elements)
                letter.Render();

            texture_top   .Draw(Vector2.Zero);
            texture_bottom.Draw(new Vector2(0, BORDER_BTM));


            foreach (SeedComputerUIElement letter in mass_elements)
                letter.RenderUI();
        }
    }
}
