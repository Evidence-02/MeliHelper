using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class VirtualButtonRebindingUI : Entity
    {
        Dictionary<Keys, bool> dict_keys_state;
        Binding binding;
        Player player;
        float alpha;
        string state, title;

        public VirtualButtonRebindingUI(string title, Binding binding) : base()
        {
            this.title = title;
            this.binding = binding;
            dict_keys_state = new Dictionary<Keys, bool>();
            Tag = Tags.HUD | Tags.PauseUpdate | Tags.FrozenUpdate;
            state = "00:appear";
        }

        #region start and end

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            player = (scene as Level).Tracker.GetEntity<Player>();
            if (player != null) Methods.PlayerLock(player);
        }

        public override void Removed(Scene scene)
        {
            base.Removed(scene);
            if (player != null) Methods.PlayerLock(player, false);
        }

        public override void SceneEnd(Scene scene)
        {
            base.SceneEnd(scene);
            if (player != null) Methods.PlayerLock(player, false);
        }

        #endregion
        
        public override void Update()
        {
            base.Update();
            switch (state)
            {
                case "00:appear":    alpha += Engine.DeltaTime / 0.4f; if (alpha >= 1) { alpha = 1; state = "01:press"; } break;
                case "09:disappear": alpha -= Engine.DeltaTime / 0.6f; if (alpha <= 0) { alpha = 0; RemoveSelf();       } break;

                case "01:press":
                    if (Methods.isKeyboardConnected())
                    {
                        // Keyboard
                        if (Keyboard.GetState().IsKeyDown(Keys.Back))
                            binding.ClearKeyboard();
                        else if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                            state = "09:disappear";
                        else
                        {
                            Keys[] pressed_keys = Keyboard.GetState().GetPressedKeys();
                            foreach (Keys key in pressed_keys)
                                if (!dict_keys_state.ContainsKey(key) || !dict_keys_state[key])
                                {
                                    dict_keys_state[key] = true;
                                    // Invert state of the key
                                    if (binding.Keyboard.Contains(key)) binding.Keyboard.Remove(key);
                                    else binding.Keyboard.Add(key);

                                }
                            
                            foreach (Keys keys in dict_keys_state.Keys)
                                dict_keys_state[keys] = pressed_keys.Contains(keys);
                        }
                    }
                    break;
            }
        }

        static int WIDPOQWEUIPJDF = 90;
        void RenderTextureOnCenter(MTexture texture, Vector2 center, Color color)
        {
            texture.Draw(center, origin: new Vector2(texture.Width / 2, texture.Height / 2), color: color);
        }

        public override void Render()
        {
            base.Render();
            if (alpha > 0)
                Draw.Rect(-1, -1, 1922, 1082, Color.Black * alpha);
            if (alpha >= 1)
            {
                ActiveFont.Draw(title, position: new Vector2(1920 / 2, 100), justify: new Vector2(0.5f), scale: new Vector2(1f), color: Color.White);
                ActiveFont.Draw(state, position: new Vector2(1920 / 2, 200), justify: new Vector2(0.5f), scale: new Vector2(1f), color: Color.White);

                int x = 500;
                int y = 480;
                Color color_buttons = Color.White * (Methods.isKeyboardConnected() ? 1f : 0.6f);
                ActiveFont.Draw("KEYBOARD", position: new Vector2(x, y - 120), justify: new Vector2(0.5f), scale: new Vector2(1f), color: color_buttons);
                if (binding.Keyboard.Count == 0)
                    RenderTextureOnCenter(Input.GuiKey(Keys.None), new Vector2(x, y), color_buttons);
                else
                    for (int i = 0; i < binding.Keyboard.Count; i++)
                        RenderTextureOnCenter(Input.GuiKey(binding.Keyboard[i]), 
                            new Vector2(x + WIDPOQWEUIPJDF * i - WIDPOQWEUIPJDF * (binding.Keyboard.Count - 1) / 2, y), color_buttons);

                x = 1920 - 500;
                color_buttons = Color.White * (Methods.isKeyboardConnected() ? 0.6f : 1f);
                ActiveFont.Draw("CONTROLLER", position: new Vector2(x, y - 120), justify: new Vector2(0.5f), scale: new Vector2(1f), color: color_buttons);
                if (binding.Controller.Count == 0)
                    RenderTextureOnCenter(Input.GuiKey(Keys.None), new Vector2(x, y), color_buttons);
                else
                    for (int i = 0; i < binding.Controller.Count; i++)
                        RenderTextureOnCenter(Input.GuiSingleButton(binding.Controller[i], Input.PrefixMode.Attached), 
                            new Vector2(x + WIDPOQWEUIPJDF * i - WIDPOQWEUIPJDF * (binding.Controller.Count - 1) / 2, y), color_buttons);
            }
        }



    }
}
