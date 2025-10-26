using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._Lani
{
    [CustomEntity("MeliHelper/LaniHookSetEntity")]
    class LaniHookSetEntity : Entity
    {
        Level level;
        LaniHookParams hook_params;
        EntityID id;
        Sprite sprite;
        Vector2 base_center;
        string dialogue, flag;
        float soaring_sin, soaring_dist, burst_timer, burst_period;
        bool is_collected, is_load_once;

        public LaniHookSetEntity(EntityData data, Vector2 offset, EntityID id) : base(data.Position + offset)
        {
            this.id = id;
            this.hook_params = LaniController.GetHookParamsFromData(data);
            Add(sprite = GFX.SpriteBank.Create(data.Attr("sprite")));
            dialogue = data.Attr("dialogueOnCollect", "");
            flag     = data.Attr("flagOnCollect", "");
            burst_period = data.Float("periodBurst", 1.2f);
            soaring_dist = data.Float("distFloating", 8);
            is_load_once = data.Bool("loadOnce", true);

            base_center = Center;
            Visible = true;
            Depth = -9999999;
			
			float size_hitbox = data.Float("hitboxSize", 6);
            Add(new PlayerCollider(OnPlayer, new Hitbox(2 * size_hitbox, 2 * size_hitbox, -size_hitbox, -size_hitbox)));
			if (data.Bool("addLight", true)) 
			{
				Add(new VertexLight(Color.White, 1f, 32, 64));
				Add(new BloomPoint(0.8f, 64));
			}
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = scene as Level;
        }

        public override void Update()
        {
            base.Update();
            if (!is_collected)
            {
                soaring_sin += 3f * Engine.DeltaTime;
                Center = base_center + new Vector2(0, soaring_dist * (float)Math.Sin(soaring_sin));


                burst_timer += Engine.DeltaTime;
                if (burst_timer >= burst_period)
                {
                    burst_timer = 0;
                    level.Displacement.AddBurst(Center, .4f, 8, 64, .5f, Ease.QuadOut, Ease.QuadOut);
                }
            }
        }

        void OnPlayer(Player player)
        {
            if (is_collected) return;

            is_collected = true;
            player.Add(new Coroutine(ActionCoroutine(player)));
        }

        IEnumerator ActionCoroutine(Player player)
        {
            // Visuals on collect
            Visible = false;
            
            level.Shake(0.3f);
            for (int i = 0; i < 8; i++)
            {
                float angle = i * (float)Math.PI / 4;
                Vector2 direction = Calc.AngleToVector(angle, 1f);
                SlashFx.Burst(Center + 10 * direction, angle);
            }

            // Wait while player will be on ground
            while (!player.OnGround())
                yield return null;

            // Lock player inputs just like in cutscenes
            Methods.PlayerLock(player);
            if (is_load_once)
                level.Session.DoNotLoad.Add(id);

            // Visuals on weapon activate
            Audio.Play(SFX.game_07_gem_get);
            Celeste.Freeze(0.1f);
            for (int i = 0; i < 6; i++)
                level.Add(new AbsorbOrb(this.Center, player, null));
            level.Flash(Color.White, true);
            Celeste.Freeze(0.2f);


            sprite.Visible = true;
            Center = player.Center - new Vector2(0, 20);
            for (int i = 0; i < 8; i++)
            {
                float angle = i * (float)Math.PI / 4;
                Vector2 direction = Calc.AngleToVector(angle, 1f);
                SlashFx.Burst(Center + 10 * direction, angle);
            }
            Celeste.Freeze(0.12f);


            // Action - create hook and show dialogue (if needed)
            LaniController.SetHook(hook_params);
            if (flag != null)
                level.Session.SetFlag(flag);
            Celeste.Freeze(0.1f);
            if (dialogue != "")
                yield return Textbox.Say(dialogue);

            // Unlock player inputs, just like in cutscenes
            Methods.PlayerLock(player, false);
            RemoveSelf();
        }

    }
}
