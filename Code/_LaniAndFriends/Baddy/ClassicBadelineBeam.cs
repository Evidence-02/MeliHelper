using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class ClassicBadelineBeam : Entity
    {
        Level level;
        Player player;
        Sprite spriteBeam, spriteStart;
        Vector2 center, player_center, direction;
        CustomTimer timer_particles;
        float alpha, angle, sprite_start_scale, sprite_start_scale_sin, sprite_start_color;

        string state;
        float timer_charge, timer_attack, timer_stop;
        
        public ClassicBadelineBeam(Player player, Vector2 direction, bool is_red = true) : base()
        {
            this.player = player;
            this.player_center = player.Center;
            this.direction = direction;
            this.angle = direction.Angle();
            this.alpha = 0;
            this.center = player.Center + 15 * direction - new Vector2(0, 3);
            this.sprite_start_scale = 0.16f;
            this.timer_particles = new CustomTimer(0.06f);
            Depth = Math.Min(-9999996, player.Depth - 1);
            
            this.state = "charge";
            this.timer_charge = 1.4f;
            this.timer_attack = 0.2f;
            this.timer_stop = 0.7f;

            string clr = (is_red ? "Red" : "Violet");
            Add(spriteBeam   = GFX.SpriteBank.Create("MeliHelper_ClassicBeam" + clr));
            Add(spriteStart  = GFX.SpriteBank.Create("MeliHelper_ClassicBeam" + clr + "Start"));
            spriteStart.Scale = new Vector2(sprite_start_scale, sprite_start_scale);
            spriteBeam.Color = Color.White * alpha;
            spriteStart.Rotation = angle;
            spriteBeam.Rotation = angle;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = scene as Level;
            if (MeliHelperModule.Instance.Session.BadelinePower_Params.isAffectPlayerSkin)
                level.Add(new BaddyVisualShadow(player, center - 15 * direction + new Vector2(0, -6)));
            Audio.Play(SFX.char_bad_boss_laser_charge);
            Methods.PlayerLock(player, true);
        }

        public override void Removed(Scene scene)
        {
            base.Removed(scene);
            Methods.PlayerLock(player, false);
        }

        public override void Update()
        {
            base.Update();
            if (!Methods.PlayerIsAlive(player)) 
            {
                RemoveSelf();
                return;
            }

            switch (state)
            {
                // Stay player at the same place
                case "charge":
                    timer_charge -= Engine.DeltaTime;
                    player.Center = player_center;

                    // 1. Sprites appears
                    if (alpha < 1)
                    {
                        alpha += Engine.DeltaTime / 0.3f;
                        spriteBeam.Color = Color.White * alpha;
                    }
                    
                    if (timer_charge > 0.4f)
                        sprite_start_scale += 0.5f * Engine.DeltaTime;  // -> 0.66f
                    else
                    {
                        sprite_start_scale_sin += 12 * (float)Math.PI * Engine.DeltaTime;
                        sprite_start_scale = 0.58f + 0.08f * (float)Math.Cos(sprite_start_scale_sin);   // 0.66 --> 0.5 --> 0.66
                        sprite_start_color += Engine.DeltaTime / 0.4f;
                        spriteStart.Color = Methods.GetColorBetween(Color.White, Color.Red, sprite_start_color);
                    }
                    spriteStart.Scale = new Vector2(sprite_start_scale, sprite_start_scale);


                    // Create particles 
                    if (timer_charge < 1 && timer_particles.Tick())
                        CreateParticles(12 + 60 * timer_charge, 24 * timer_charge);

                    // Go to attack phase
                    if (timer_charge <= 0)
                    {
                        state = "attack";
                        spriteStart.Play("charge");
                        CreateParticles();
                        Audio.Play(SFX.char_bad_boss_laser_fire);

                        // Player: unlick, make trail and go nyooom!
                        Methods.PlayerLock(player, false);
                        player.Add(new Coroutine(Methods.TrailGradientCoroutine(player, Color.Red, Color.Transparent, 0.33f)));

                        // Nyooooom?
                        //player.Speed.X = -2f * Math.Sign(direction.X) * (direction.Y == 0 ? 252 : 225);
                        //player.Speed.Y = ((direction.Y != 0) ? (-Math.Sign(direction.Y) * (direction.X == 0 ? 540 : 486)) : -40);
                        player.Speed.X = -2f * Math.Sign(direction.X) * (direction.Y == 0 ? 196 : 175);
                        player.Speed.Y = ((direction.Y != 0) ? (-Math.Sign(direction.Y) * (direction.X == 0 ? 420 : 378)) : -40);
                        player.AutoJump = true;
                    }
                    break;

                // 
                case "attack":
                    timer_attack -= Engine.DeltaTime;
                    ObjCollideCheck();
                    if (timer_attack <= 0)
                        state = "stop";
                    break;

                case "stop":
                    timer_stop -= Engine.DeltaTime;
                    if (timer_stop <= 0)
                        RemoveSelf();
                    break;
            }
        }



        private void CreateParticles(float distance = 12, float left = 0)
        {
            Level level = SceneAs<Level>();
            Vector2 closestTo = level.Camera.Position + new Vector2(160f, 90f);
            Vector2 lineA = center + Calc.AngleToVector(angle, 12);
            Vector2 lineB = center + Calc.AngleToVector(angle, 2000f);
            Vector2 vector = (lineB - lineA).Perpendicular().SafeNormalize();
            Vector2 vector2_1 = (lineB - lineA).SafeNormalize();
            Vector2 vector2_2 = Calc.ClosestPointOnLine(lineA, lineB, closestTo);
            Vector2 min = -vector * 1f;
            Vector2 max = vector * 1f;
            float direction1 = vector.Angle();
            float direction2 = (-vector).Angle();
            float num = Vector2.Distance(closestTo, lineA) - 12f;
            for (float index1 = left; index1 < 200; index1 += distance)
            {
                for (int sign = -1; sign <= 1; sign += 2)
                {
                    level.ParticlesFG.Emit(FinalBossBeam.P_Dissipate,
                        vector2_2 + vector2_1 * index1 + vector * 2f * sign + Calc.Random.Range(min, max),
                        direction1);
                    level.ParticlesFG.Emit(FinalBossBeam.P_Dissipate,
                        vector2_2 + vector2_1 * index1 - vector * 2f * sign + Calc.Random.Range(min, max),
                        direction2);
                }
            }
        }

        private void ObjCollideCheck()
        {
            Level level = SceneAs<Level>();
            Vector2 from = center - Calc.AngleToVector(angle, 12f);
            Vector2 to = center + Calc.AngleToVector(angle, 280f);
            Vector2 perp = (to - from).Perpendicular().SafeNormalize(4f);

            List<CrystalStaticSpinner> list_spinners = level.Entities.FindAll<CrystalStaticSpinner>()
                .FindAll(t => t.CollideLine(from + 1.5f * perp, to + 1.5f * perp)
                           || t.CollideLine(from + 0.5f * perp, to + 0.5f * perp)
                           || t.CollideLine(from - 0.5f * perp, to - 0.5f * perp)
                           || t.CollideLine(from - 1.5f * perp, to - 1.5f * perp)
                           || t.CollideLine(from, to));
            if (list_spinners.Count > 0)
            {
                Audio.Play("event:/game/06_reflection/boss_spikes_burst");
                foreach (CrystalStaticSpinner spinner in list_spinners)
                    spinner.Destroy();
            }


            // Dash blocks
            List<DashBlock> list_blocks = level.Entities.FindAll<DashBlock>().FindAll(t => t.CollideLine(from, to));
            foreach (DashBlock block_dash in list_blocks)
                InteractionController.ActivateBlock(block_dash, this);
                
            // Temple blocks
            List<TempleCrackedBlock> list_blocks_temple = level.Entities.FindAll<TempleCrackedBlock>().FindAll(t => t.CollideLine(from, to));
            foreach (TempleCrackedBlock block_temple in list_blocks_temple)
                InteractionController.ActivateBlock(block_temple, this);

            // Falling block
            List<FallingBlock> list_blocks_fall = level.Entities.FindAll<FallingBlock>().FindAll(t => !t.Triggered && t.CollideLine(from, to));
            foreach (FallingBlock block_fall in list_blocks_fall)
                InteractionController.ActivateBlock(block_fall, this);

            // Dust bunnies
            List<DustStaticSpinner> list_dusts = level.Entities.FindAll<DustStaticSpinner>().FindAll(t => 
							  t.CollideLine(from + 2.5f * perp, to + 2.5f * perp)
                           || t.CollideLine(from + 1.5f * perp, to + 1.5f * perp)
                           || t.CollideLine(from + 0.5f * perp, to + 0.5f * perp)
                           || t.CollideLine(from - 0.5f * perp, to - 0.5f * perp)
                           || t.CollideLine(from - 1.5f * perp, to - 1.5f * perp)
                           || t.CollideLine(from - 2.5f * perp, to - 2.5f * perp)
                           || t.CollideLine(from, to));
            foreach (DustStaticSpinner dust in list_dusts)
            {
                dust.RemoveSelf();
                for (int i = 0; i < 4; i++)
                    level.Particles.Emit(new ParticleType(FinalBoss.P_Burst) { Color = Color.Black, Color2 = Color.Gray }, dust.Center);
            }

            List<DustTrackSpinner> list_dusts2 = level.Entities.FindAll<DustTrackSpinner>().FindAll(t => 
							  t.CollideLine(from + 2.5f * perp, to + 2.5f * perp)
                           || t.CollideLine(from + 1.5f * perp, to + 1.5f * perp)
                           || t.CollideLine(from + 0.5f * perp, to + 0.5f * perp)
                           || t.CollideLine(from - 0.5f * perp, to - 0.5f * perp)
                           || t.CollideLine(from - 1.5f * perp, to - 1.5f * perp)
                           || t.CollideLine(from - 2.5f * perp, to - 2.5f * perp)
                           || t.CollideLine(from, to));
            foreach (DustTrackSpinner dust in list_dusts2)
            {
                dust.RemoveSelf();
                for (int i = 0; i < 4; i++)
                    level.Particles.Emit(new ParticleType(FinalBoss.P_Burst) { Color = Color.Black, Color2 = Color.Gray }, dust.Center);
            }

            List<DustRotateSpinner> list_dusts3 = level.Entities.FindAll<DustRotateSpinner>().FindAll(t => 
							  t.CollideLine(from + 2.5f * perp, to + 2.5f * perp)
                           || t.CollideLine(from + 1.5f * perp, to + 1.5f * perp)
                           || t.CollideLine(from + 0.5f * perp, to + 0.5f * perp)
                           || t.CollideLine(from - 0.5f * perp, to - 0.5f * perp)
                           || t.CollideLine(from - 1.5f * perp, to - 1.5f * perp)
                           || t.CollideLine(from - 2.5f * perp, to - 2.5f * perp)
                           || t.CollideLine(from, to));
            foreach (DustRotateSpinner dust in list_dusts3)
            {
                dust.RemoveSelf();
                for (int i = 0; i < 4; i++)
                    level.Particles.Emit(new ParticleType(FinalBoss.P_Burst) { Color = Color.Black, Color2 = Color.Gray }, dust.Center);
            }

            // Theo crystal (i'm not sorry!)
            List<TheoCrystal> list_theocrystals = level.Entities.FindAll<TheoCrystal>().FindAll(t => t.CollideLine(from, to));
            foreach (TheoCrystal crystal in list_theocrystals)
                crystal.Die();
            


            //--------------------------------------------------------------------------
            // Evil things (oshiro, snowballs, seekers and baddy) (baddy isn't evil, she is cute!)
            //             (there a list  and not one entity because in mods can be more than 1 oshiro or badeline bosses)
            List<AngryOshiro> list_oshiros = level.Entities.FindAll<AngryOshiro>().FindAll(t => t.CollideLine(from, to));
            foreach (AngryOshiro boss_oshiro in list_oshiros)
                boss_oshiro.RemoveSelf();   // oshiro have no death animation, just delete (sad)

            List<Snowball> list_snowballs = level.Entities.FindAll<Snowball>().FindAll(t => t.CollideLine(from, to));
            foreach (Snowball snowball in list_snowballs)
            {
                Audio.Play(SFX.game_04_snowball_impact);
                DynData<Snowball> dyn = new DynData<Snowball>(snowball);
                Sprite sprite = dyn.Get<Sprite>("sprite");
                snowball.RemoveSelf();
            }

            // Seeker's hitbox is really small...
            List<Seeker> list_seekers = level.Entities.FindAll<Seeker>().FindAll(t => t.CollideLine(from + 2.0f * perp, to + 2.0f * perp)
                                                                                   || t.CollideLine(from + 1.5f * perp, to + 1.5f * perp)
                                                                                   || t.CollideLine(from + 1.0f * perp, to + 1.0f * perp)
                                                                                   || t.CollideLine(from + 0.5f * perp, to + 0.5f * perp)
                                                                                   || t.CollideLine(from, to)
                                                                                   || t.CollideLine(from - 0.5f * perp, to - 0.5f * perp)
                                                                                   || t.CollideLine(from - 1.0f * perp, to - 1.0f * perp)
                                                                                   || t.CollideLine(from - 1.5f * perp, to - 1.5f * perp)
                                                                                   || t.CollideLine(from - 2.0f * perp, to - 2.0f * perp));
            foreach (Seeker seeker in list_seekers)
            {
                // okay, how to squish? idk. just bounce it!
                InteractionController.HitSeeker(seeker, this);
                //seeker.SquishCallback(new CollisionData() { });
                //seeker.RemoveSelf();
            }

            List<FinalBoss> list_baddies = level.Entities.FindAll<FinalBoss>().FindAll(t => t.CollideLine(from, to) && !t.Sitting);
            foreach (FinalBoss boss_baddy in list_baddies)
            {
                boss_baddy.RemoveSelf();
                foreach (FinalBossBeam beam in level.Tracker.GetEntities<FinalBossBeam>())
                    beam.RemoveSelf();
            }

            // Temple eyeball
            TempleBigEyeball eyeball_ch5 = level.Entities.FindAll<TempleBigEyeball>().FirstOrDefault(t => t.CollideLine(from, to));
            if (eyeball_ch5 != null)
            {
                // Set a big Theo speed and get it back after eyeballcrushing because it checks inside of OnHoldable method
                TheoCrystal crystal = level.Tracker.GetEntity<TheoCrystal>();
                if (crystal != null)
                    InteractionController.HitTempleGiantEyeball(eyeball_ch5, player, crystal);
            }


            // Fishes!
            List<Puffer> list_puffers = level.Entities.FindAll<Puffer>()
                .FindAll(t =>  t.CollideLine(from + 2.0f * perp, to + 2.0f * perp)
                            || t.CollideLine(from + 1.5f * perp, to + 1.5f * perp)
                            || t.CollideLine(from + 1.0f * perp, to + 1.0f * perp)
                            || t.CollideLine(from + 0.5f * perp, to + 0.5f * perp)
                            || t.CollideLine(from, to)
                            || t.CollideLine(from - 0.5f * perp, to - 0.5f * perp)
                            || t.CollideLine(from - 1.0f * perp, to - 1.0f * perp)
                            || t.CollideLine(from - 1.5f * perp, to - 1.5f * perp)
                            || t.CollideLine(from - 2.0f * perp, to - 2.0f * perp));
            foreach (Puffer puffer in list_puffers)
                InteractionController.PufferExplode(puffer);

            // Core fireballs
            List<FireBall> list_fireballs = level.Entities.FindAll<FireBall>()
                .FindAll(t =>  t.CollideLine(from + 2.0f * perp, to + 2.0f * perp)
                            || t.CollideLine(from + 1.5f * perp, to + 1.5f * perp)
                            || t.CollideLine(from + 1.0f * perp, to + 1.0f * perp)
                            || t.CollideLine(from + 0.5f * perp, to + 0.5f * perp)
                            || t.CollideLine(from, to)
                            || t.CollideLine(from - 0.5f * perp, to - 0.5f * perp)
                            || t.CollideLine(from - 1.0f * perp, to - 1.0f * perp)
                            || t.CollideLine(from - 1.5f * perp, to - 1.5f * perp)
                            || t.CollideLine(from - 2.0f * perp, to - 2.0f * perp));
            foreach (FireBall fireball in list_fireballs)
                fireball.RemoveSelf();

            // That fire block from core named as "Bounce block", it's hard to predict
            List<BounceBlock> list_bounceblocks = level.Entities.FindAll<BounceBlock>()
                .FindAll(t =>  t.CollideLine(from + 2.0f * perp, to + 2.0f * perp)
                            || t.CollideLine(from, to)
                            || t.CollideLine(from - 2.0f * perp, to - 2.0f * perp));
            foreach (BounceBlock bounceblock in list_bounceblocks)
                InteractionController.SetCoreMode(bounceblock, Session.CoreModes.Hot);

            List<IceBlock> list_iceblocks = level.Entities.FindAll<IceBlock>()
                .FindAll(t =>  t.CollideLine(from + 2.0f * perp, to + 2.0f * perp)
                            || t.CollideLine(from, to)
                            || t.CollideLine(from - 2.0f * perp, to - 2.0f * perp));
            foreach (IceBlock iceblock in list_iceblocks)
                iceblock.RemoveSelf();

            //List<FireBarrier> list_firebarriers = level.Entities.FindAll<FireBarrier>()
            //    .FindAll(t => t.CollideLine(from + 2.0f * perp, to + 2.0f * perp)
            //                || t.CollideLine(from, to)
            //                || t.CollideLine(from - 2.0f * perp, to - 2.0f * perp));
            //foreach (FireBarrier firebarrier in list_firebarriers)
            //    firebarrier.RemoveSelf();



            List<Lightning> list_lightning = level.Entities.FindAll<Lightning>().FindAll(t => t.CollideLine(from, to));
            if (list_lightning.Count > 0)
            {
                Audio.Play(SFX.game_10_lightning_strike);
                foreach (Lightning block_lightning in list_lightning)
                    block_lightning.RemoveSelf();
            }

            



            // Destroying decals also, yay
            // UPD: bruh, doesn't work
            //if (!is_decals_destroyed)
            //{
            //    List<Decal> list_decals = level.Entities.FindAll<Decal>()
            //        .FindAll(t => t.Visible &&
            //                     (t.CollideLine(from + 3f * perp, to + 3f * perp)
            //                   || t.CollideLine(from + 2f * perp, to + 2f * perp)
            //                   || t.CollideLine(from + 1f * perp, to + 1f * perp)
            //                   || t.CollideLine(from, to)
            //                   || t.CollideLine(from - 1f * perp, to - 1f * perp)
            //                   || t.CollideLine(from - 2f * perp, to - 2f * perp)
            //                   || t.CollideLine(from - 3f * perp, to - 3f * perp)
            //                   ));
            //    foreach (Decal decal in list_decals)
            //    {
            //        decal.Visible = false;
            //        for (int i = 0; i < 3; i++)
            //            level.Particles.Emit(new ParticleType(FinalBoss.P_Burst) { Color = Color.DarkRed, Color2 = Color.Red }, decal.Center);
            //    }

            //    is_decals_destroyed = true;
            //}
        }

        public override void Render()
        {
            Vector2 beamPositionDel = Calc.AngleToVector(angle, spriteBeam.Width);
            Vector2 beamPosition = center + beamPositionDel / 2;
            for (int i = 0; i < 15; i++)
            {
                spriteBeam.RenderPosition = beamPosition;
                spriteBeam.Render();
                beamPosition += beamPositionDel;
            }

            spriteStart.RenderPosition = center;
            spriteStart.Render();
        }

        public Entity GetParent
        {
            get
            {
                return player;
            }
        }

        public bool isEnded
        {
            get
            {
                return state == "attack" || state == "stop";
            }
        }

    }
}
