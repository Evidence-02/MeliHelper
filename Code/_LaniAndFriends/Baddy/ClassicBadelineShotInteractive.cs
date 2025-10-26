using FMOD.Studio;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
//using VivHelper.Entities;

namespace Celeste.Mod.MeliHelper._Baddy
{
    class ClassicBadelineShotInteractive : ClassicBadelineShot
    {
        //private static MethodInfo bumperOnPlayer = typeof(Bumper).GetMethod("OnPlayer", BindingFlags.NonPublic | BindingFlags.Instance);
        //private static MethodInfo bumperUpdatePosition = typeof(Bumper).GetMethod("UpdatePosition", BindingFlags.NonPublic | BindingFlags.Instance);
        enum ShotState { Normal, DreamBlocked }


        ShotState state;
        DreamBlock block_dream_current;
        SoundSource dreamSfxLoop;
        float radius_explosion;
        bool is_explode, is_dream_reflected;

        public ClassicBadelineShotInteractive(Vector2 pos, Vector2 speed, ClassicBadelineShotColorEnum color, bool is_can_damage_player, 
            float radius_explosion = 40) 
            : base(pos, speed, color, is_can_damage_player: is_can_damage_player)
        {
            this.radius_explosion = radius_explosion;
            this.state = ShotState.Normal;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            // overpowered only
            //Add(new VertexLight(Color.Red, 1f, 32, 64));
            //Add(new BloomPoint(0.4f, 64));
            if (level.Lighting.Alpha >= 0.075)
                Add(new VertexLight(Color.Red, 0.75f, 32, 64));
        }

        public override void Removed(Scene scene)
        {
            if (block_dream_current != null)
                block_dream_current.RemoveSelf();
            if (dreamSfxLoop != null)
                dreamSfxLoop.Stop();
            base.Removed(scene);
        }

        public override void Update()
        {
            base.Update();
            
            Entity killbox = level.Tracker.GetEntities<Killbox>().FirstOrDefault(t => t.CollideCheck(this));
            if (killbox != null)
            {
                RemoveSelf();
                return;
            }

            switch (state)
            {
                case ShotState.Normal:
                    CheckCollisionsBlocks();
                    CheckCollisionsFriendly();
                    CheckCollisionsEnemies();
                    CheckCollisionsObjects();
                    CheckCollisionsCore();
                    break;

                case ShotState.DreamBlocked:
                    if (block_dream_current.CollideCheck(this))
                    {
                        TrailManager.Add(this, Color.Red * 0.5f, 1);
                        level.Displacement.AddBurst(Center, .4f, 8, 64, .5f, Ease.QuadOut, Ease.QuadOut);
                    }
                    else
                    {
                        // Create new crumble platform from VivHelper (if block is not on ground or block too high)
                        if (block_dream_current.Width > 8)
                        {
                            float platform_y = -1;
                            Rectangle rect_check_solid = new Rectangle((int)block_dream_current.Left + 4, (int)block_dream_current.Bottom + 2,
                                (int)block_dream_current.Width - 8, 2);
                            if (!level.Tracker.GetEntities<Solid>().Exists(t => t.CollideRect(rect_check_solid)))
                                platform_y = block_dream_current.Bottom;
                            else if (block_dream_current.Height > 200)
                                platform_y = block_dream_current.Center.Y;

                            if (platform_y != -1)
                            {
                                EntityData entityData = new EntityData()
                                {
                                    Position = new Vector2(block_dream_current.Left + 4, platform_y),
                                    ID = Calc.Random.Next(),
                                    Name = @"VivHelper/CrumbleJumpThru",
                                    Width = (int)block_dream_current.Width - 8
                                };
                                entityData.Values = new Dictionary<string, object>();
                                entityData.Values.Add("texture", "dream");
                                entityData.Values.Add("delay", 0.2f);
                                //level.Add(new CrumbleJumpThruOnTouch(entityData, Vector2.Zero));
                            }
                        }

                        // Delete all spikes in radius 8 pixels
                        Methods.DeleteSpikesAroundRect(level, new Rectangle((int)block_dream_current.Left, (int)block_dream_current.Top, 
                            (int)block_dream_current.Width, (int)block_dream_current.Height));
                        block_dream_current.RemoveSelf();
                        
                        // Find new block
                        block_dream_current = CollideFirst<DreamBlock>();
                        if (block_dream_current == null)
                        {
                            // Explode with 5 bullets, if new dream block hasn't finded
                            Audio.Play(SFX.char_bad_dreamblock_exit);
                            Player player = level.Tracker.GetEntity<Player>();
                            if (player != null)
                                player.Stop(dreamSfxLoop);
                            RemoveSelf();

                            if (is_dream_reflected)
                                level.Add(new ClassicBadelineShot(Center, Speed, ClassicBadelineShotColorEnum.Violet, 
                                    is_collide_walls: false, burst_power: 48));
                            else
                            {
                                float angle = Speed.Angle();
                                float length = Speed.Length();
                                ClassicBadelineShotColorEnum[] mass_colors = {
                                    ClassicBadelineShotColorEnum.Black,
                                    ClassicBadelineShotColorEnum.White,
                                    ClassicBadelineShotColorEnum.Red,
                                    ClassicBadelineShotColorEnum.Violet,
                                    ClassicBadelineShotColorEnum.Yellow,
                                    ClassicBadelineShotColorEnum.Blue,
                                    ClassicBadelineShotColorEnum.Red,
                                    ClassicBadelineShotColorEnum.Violet,
                                    ClassicBadelineShotColorEnum.Yellow,
                                    ClassicBadelineShotColorEnum.Blue
                                };
                                foreach (ClassicBadelineShotColorEnum color in mass_colors)
                                {
                                    Vector2 speed = Calc.AngleToVector(
                                       angle + Calc.Random.Next(-30, 31) * (float)Math.PI / 180,
                                       length * (1.2f + Calc.Random.NextFloat(0.8f)));
                                    level.Add(new ClassicBadelineShot(Center, speed, color, 
                                        is_collide_walls: false, burst_power: 32));
                                }
                            }
                        }
                    }
                    break;
            }
            

            if (is_explode)
                if (radius_explosion > 0) Explode(); else RemoveSelf();
        }


        void CheckCollisionsBlocks()
        {
            Entity spinner = level.Tracker.GetEntities<CrystalStaticSpinner>()
                .FirstOrDefault(t => t.CollideCheck(this));
            if (spinner != null)
            {
                //Audio.Play(SFX.game_06_boss_spikes_burst);
                if (radius_explosion == 0)
                    (spinner as CrystalStaticSpinner).Destroy();
                is_explode = true;
            }
         
            // All types of blocks
            DashBlock block_dash = CollideFirst<DashBlock>();
            if (block_dash != null)
            {
                InteractionController.ActivateBlock(block_dash, this);
                is_explode = true;
            }

            FallingBlock block_fall = CollideFirst<FallingBlock>();
            if (block_fall != null)
            {
                InteractionController.ActivateBlock(block_fall, this);
                is_explode = true;
            }

            ZipMover block_ch1 = level.Entities.FindAll<ZipMover>()
                .FirstOrDefault(t => t.CollideCheck(this));
            if (block_ch1 != null)
            {
                InteractionController.ActivateBlock(block_ch1, this);
                is_explode = true;
            }

            DreamBlock block_dream = CollideFirst<DreamBlock>();
            if (block_dream != null)
            {
                if (InteractionController.CanDashIntoDreamBlock(block_dream))
                {
                    Audio.Play(SFX.char_bad_dreamblock_enter);
                    Player player = level.Tracker.GetEntity<Player>();
                    if (Methods.PlayerIsAlive(player))
                    {
                        DynData<Player> playerData = new DynData<Player>(player);
                        dreamSfxLoop = playerData.Get<SoundSource>("dreamSfxLoop");
                        if (dreamSfxLoop == null)
                        {
                            dreamSfxLoop = new SoundSource();
                            player.Add(dreamSfxLoop);
                            playerData["dreamSfxLoop"] = dreamSfxLoop;
                        }
                        player.Loop(dreamSfxLoop, SFX.char_bad_dreamblock_travel);
                    }
                    block_dream_current = block_dream;
                    //sprite.Play("black");
                    Speed *= 1.25f;
                    sprite.Color = Color.White * 0.5f;
                    state = ShotState.DreamBlocked;
                }
                else
                {
                    // Just reflects from block
                    Audio.Play(SFX.game_assist_dreamblockbounce);
                    //sprite.Play("black");

                    Vector2 center_old = Center;
                    Vector2 center_new = GetPrevLocation();

                    bool is_reflect_x = false;
                    bool is_reflect_y = false;
                    int counter = 30;
                    Vector2 collide_point = Center;
                    while (!block_dream.CollidePoint(collide_point))
                    {
                        collide_point += Speed * Engine.DeltaTime;
                        if (--counter <= 0) break;
                    }
                    Vector2 center_check_x = collide_point;
                    Vector2 center_check_y = collide_point;
                    while (!is_reflect_x && !is_reflect_y)
                    {
                        center_check_x.X -= Speed.X * Engine.DeltaTime;
                        center_check_y.Y -= Speed.Y * Engine.DeltaTime;
                        is_reflect_x = !block_dream.CollidePoint(center_check_x);
                        is_reflect_y = !block_dream.CollidePoint(center_check_y);
                        if (--counter <= 0) break;
                    }

                    // if counter < 0, we don't know what direction is reflect
                    if (counter <= 0)
                        Speed *= -1;
                    else
                    {
                        Center = center_new;
                        if (is_reflect_x) Speed.X *= -1.2f;
                        else if (is_reflect_y) Speed.Y *= -1.2f;
                    }
                }
            }

            // Dust bunnies from ch3, all types
            DustStaticSpinner spinner_dust_static = level.Entities.FindAll<DustStaticSpinner>()
                .FirstOrDefault(t => t.CollideCheck(this));
            if (spinner_dust_static != null)
            {
                spinner_dust_static.RemoveSelf();
                is_explode = true;
                for (int i = 0; i < 4; i++)
                    level.Particles.Emit(new ParticleType(FinalBoss.P_Burst) { Color = Color.Black, Color2 = Color.Gray }, spinner_dust_static.Center);
            }

            DustTrackSpinner spinner_dust_track = level.Entities.FindAll<DustTrackSpinner>()
                .FirstOrDefault(t => t.CollideCheck(this));
            if (spinner_dust_track != null)
            {
                spinner_dust_track.RemoveSelf();
                is_explode = true;
                for (int i = 0; i < 4; i++)
                    level.Particles.Emit(new ParticleType(FinalBoss.P_Burst) { Color = Color.Black, Color2 = Color.Gray }, spinner_dust_track.Center);
            }

            DustRotateSpinner spinner_dust_rotate = level.Entities.FindAll<DustRotateSpinner>()
                .FirstOrDefault(t => t.CollideCheck(this));
            if (spinner_dust_rotate != null)
            {
                spinner_dust_rotate.RemoveSelf();
                is_explode = true;
                for (int i = 0; i < 4; i++)
                    level.Particles.Emit(new ParticleType(FinalBoss.P_Burst) { Color = Color.Black, Color2 = Color.Gray }, spinner_dust_rotate.Center);
            }

            // Move blocks from ch4
            MoveBlock block_ch4_move = level.Entities.FindAll<MoveBlock>()
                .FirstOrDefault(t => t.CollideCheck(this));
            if (block_ch4_move != null)
            {
                InteractionController.ActivateBlock(block_ch4_move, this);
                is_explode = true;
            }

            // Temple
            TempleCrackedBlock block_ch5_crack = level.Entities.FindAll<TempleCrackedBlock>()
                .FirstOrDefault(t => t.CollideCheck(this));
            if (block_ch5_crack != null)
            {
                InteractionController.ActivateBlock(block_ch5_crack, this);
                is_explode = true;
            }

            SwapBlock block_ch5_swap = level.Entities.FindAll<SwapBlock>()
                .FirstOrDefault(t => t.CollideCheck(this));
            if (block_ch5_swap != null)
            {
                InteractionController.ActivateBlock(block_ch5_swap, this);
                is_explode = true;
            }

            // yes, Kevins is named as "CrushBlock" in the game code. deal with it!
            // normal kevin speed is 1, but bullets make them a little bit faster than vanilla (1.25f)
            // moral: don't shoot in kevins!
            CrushBlock block_kevin = level.Entities.FindAll<CrushBlock>()
                .FirstOrDefault(t => t.CollideCheck(this));
            if (block_kevin != null)
            {
                InteractionController.ActivateBlockKevin(block_kevin, this, 1.25f);
                is_explode = true;
            }

            // Farewell, power source block
            LightningBreakerBox block_light = level.Entities.FindAll<LightningBreakerBox>()
                .FirstOrDefault(t => t.CollideCheck(this));
            if (block_light != null)
            {
                InteractionController.ActivateBlock(block_light, this, this.Speed);
                is_explode = true;
            }

            Lightning block_lightning = level.Entities.FindAll<Lightning>()
                .FirstOrDefault(t => t.CollideCheck(this));
            if (block_lightning != null)
            {
                Audio.Play(SFX.char_mad_water_dash_in, block_lightning.Center);
                block_lightning.RemoveSelf();

                // Create water block instead of lightning (too op)
                level.Add(new Water(block_lightning.Position, false, false, block_lightning.Width, block_lightning.Height));
                is_explode = true;

                //Player player = level.Tracker.GetEntity<Player>();
                //block_lightning.Add(new Coroutine(Lightning.RemoveRoutine(level), true));
            }
        }

        void CheckCollisionsFriendly()
        {
            // Theo crystal (it's not dangerous, just a little move!)
            TheoCrystal theocrystal = level.Entities.FindAll<TheoCrystal>()
                .FirstOrDefault(t => t.CollideCheck(this));
            if (theocrystal != null)
            {
                Audio.Play(SFX.game_05_crystaltheo_impact_ground);
                theocrystal.Speed.X -= 160f * Math.Sign(this.Center.X - theocrystal.Center.X);
                theocrystal.Speed.Y -= 80f;
                RemoveSelf();
            }

            // she angwy
            BadelineBoost baddy_boost = level.Entities.FindAll<BadelineBoost>()
                .FirstOrDefault(t => t.CollideCheck(this));
            if (baddy_boost != null)
            {
                baddy_boost.Wiggle();
                string[] mass_sounds = { 
                    SFX.game_06_badeline_freakout_1, 
                    SFX.game_06_badeline_freakout_2, 
                    SFX.game_06_badeline_freakout_3, 
                    SFX.game_06_badeline_freakout_4, 
                    SFX.game_06_badeline_freakout_5 };
                Audio.Play(mass_sounds[Calc.Random.Next(0, mass_sounds.Length)]);
                level.Lighting.Alpha -= 0.16f;
                Glitch.Value += 0.012f;
                RemoveSelf();
            }
        }

        void CheckCollisionsEnemies()
        {
            //--------------------------------------------------------------------------
            // Evil things (oshiro, snowballs, seekers and baddy) (baddy isn't evil, she is cute!)
            AngryOshiro boss_oshiro = CollideFirst<AngryOshiro>();
            if (boss_oshiro != null)
            {
                Player player = level.Tracker.GetEntity<Player>();
                if (Methods.PlayerIsAlive(player))
                {
                    float power = BaddyController.GetParams().CurrentPower;
                    InteractionController.HitOshiro(boss_oshiro, player);
                    BaddyController.GetParams().CurrentPower = power;
                }
                is_explode = true;
            }

            Snowball snowball = level.Entities.FindAll<Snowball>()
                .FirstOrDefault(t => t.CollideCheck(this) && t.Collidable);
            if (snowball != null)
            {
                InteractionController.HitSnowball(snowball);
                is_explode = true;
            }

            // wtf with a seeker hitbox? it's too small!
            //Seeker seeker = CollideFirst<Seeker>();
            Seeker seeker = level.Entities.FindAll<Seeker>()
                .FirstOrDefault(t => Vector2.Distance(t.Center, this.Center) <= 12);
            if (seeker != null)
            {
                InteractionController.HitSeeker(seeker, this);
                is_explode = true;
            }

            SeekerStatue seekerStatue = level.Entities.FindAll<SeekerStatue>()
                .FirstOrDefault(t => Vector2.Distance(t.Center, this.Center) <= 16);
            if (seekerStatue != null)
            {
                Player player = level.Tracker.GetEntity<Player>();
                if (Methods.PlayerIsAlive(player))
                {
                    InteractionController.ActivateSeekerStatue(seekerStatue, player);
                    is_explode = true;
                }
            }

            FinalBoss boss_baddy = CollideFirst<FinalBoss>();
            if (boss_baddy != null)
            {
                boss_baddy.OnPlayer(null);
                is_explode = true;
            }

            //FinalBossShot boss_shot = CollideFirst<FinalBossShot>();
            //if (boss_shot != null)
            //    is_explode = true;

            TempleBigEyeball eyeball_ch5 = level.Entities.FindAll<TempleBigEyeball>()
                .FirstOrDefault(t => t.CollideCheck(this));
            if (eyeball_ch5 != null)
            {
                // Set a big Theo speed and get it back after eyeballcrushing because it checks inside of OnHoldable method
                Player player = level.Tracker.GetEntity<Player>();
                TheoCrystal crystal = level.Tracker.GetEntity<TheoCrystal>();
                if (Methods.PlayerIsAlive(player) && player.Center.X - eyeball_ch5.Center.X < 200 && crystal != null)
                {
                    InteractionController.HitTempleGiantEyeball(eyeball_ch5, player, crystal);
                    RemoveSelf();
                }
            }
        }

        void CheckCollisionsObjects()
        {
            //--------------------------------------------------------------------
            // Another custom things from the game (switches, feather, bumpers, )
            TouchSwitch switch_touch = level.Entities.FindAll<TouchSwitch>()
                .FirstOrDefault(t => t.CollideCheck(this));
            if (switch_touch != null)
            {
                // In the first version there was a bug with multiply sounds, when check on ease didn't exists yet
                // Normally check should be "ease == 0", but I like that sound bug
                // A different deep sound on touch just wow
                DynData<TouchSwitch> dyn = new DynData<TouchSwitch>(switch_touch);
                Single ease = dyn.Get<Single>("ease");
                if (ease < 0.16f)
                {
                    Audio.Play(SFX.game_gen_touchswitch_any, "", 0.4f);
                    switch_touch.TurnOn();
                }
            }

            DashSwitch switch_dash = level.Entities.FindAll<DashSwitch>()
                .FirstOrDefault(t => t.CollideCheck(this));
            if (switch_dash != null)
                InteractionController.ActivateDashSwitch(switch_dash);
            

            ClutterSwitch switch_clutter = level.Entities.FindAll<ClutterSwitch>()
                .FirstOrDefault(t => t.CollideCheck(this));
            if (switch_clutter != null)
            {
                InteractionController.ActivateClutterSwitch(switch_clutter, Vector2.Zero);
                RemoveSelf();
            }

            // Feathers  - delete the shield from a shielded feathers or collect otherwise
            FlyFeather feather = CollideFirst<FlyFeather>();
            if (feather != null)
            {
                DynData<FlyFeather> dyn = new DynData<FlyFeather>(feather);
                Player player = level.Tracker.GetEntity<Player>();
                if (dyn.Get<bool>("shielded"))
                {
                    level.Particles.Emit(FlyFeather.P_Collect, 12, Center, Vector2.One * 4, (float)Math.PI + Speed.Angle());
                    dyn.Set("shielded", false);
                }
                else if (Methods.PlayerIsAlive(player))
                    InteractionController.FeatherCollect(feather, player);
                RemoveSelf();
            }

            // Bumpers! (they're evil)
            Bumper bumper = level.Entities.FindAll<Bumper>()
                .FirstOrDefault(t => t.CollideCheck(this));
            if (bumper != null)
            {
                DynData<Bumper> dyn = new DynData<Bumper>(bumper);
                bool isFireMode = dyn.Get<bool>("fireMode");
                Sprite sprite_bumper = dyn.Get<Sprite>("sprite");
                sprite_bumper.Play("hit");

                // yep, sprite changes and evil bumper is moving cool but player is dead
                //bumperOnPlayer.Invoke(bumper, new object[] { level.Tracker.GetEntity<Player>() });
                //bumperUpdatePosition.Invoke(bumper, new object[] { });
                
                // Ricochet from bumpers can damage player (really evil!)
                Audio.Play(isFireMode ? SFX.game_09_pinballbumper_hit : SFX.game_06_pinballbumper_hit);
                if (!isFireMode)
                {
                    // Normal behaviour - reflect back evil violet bullet that can damage player
                    Vector2 speed_norm_new = Vector2.Normalize(this.Center - bumper.Center);
                    this.Speed = 1.4f * this.Speed.Length() * speed_norm_new;
                    this.Center += Speed * Engine.DeltaTime;
                    if (!is_damage_player)
                        SetColor(ClassicBadelineShotColorEnum.Violet, true);
                }
                else
                {
                    // Evil core bumper behaviour - create 12 evil violet bullets 
                    // (It's a touhou reference?!)
                    RemoveSelf();
                    if (!is_damage_player)
                    {
                        int count_bullets = 12;
                        for (int i = 0; i < count_bullets; i++)
                        {
                            Vector2 speed_norm = Calc.AngleToVector(2 * (float)Math.PI * i / count_bullets, 1f);
                            Vector2 center = bumper.Center + 12 * speed_norm;
                            Vector2 speed = 180 * speed_norm;
                            level.Add(new ClassicBadelineShotInteractive(center, speed, 
                                color: Calc.Random.Next(0, 3) == 0 ? ClassicBadelineShotColorEnum.Violet : ClassicBadelineShotColorEnum.Red,
                                is_can_damage_player: true,
                                radius_explosion: 20
                                ));
                        }
                    }
                }
            }


            /*
            Torch torch = level.Entities.FindAll<Torch>()
                .FirstOrDefault(t => t.CollideCheck(this));
            if (torch != null)
            {
                DynData<Torch> dyn = new DynData<Torch>(torch);
                Sprite sprite = dyn.Get<Sprite>("sprite");
                //Methods.DebugLogHiddenFields(typeof(Torch), torch);
                //is_explode = true;

                if (sprite.CurrentAnimationID == "off")
                {
                    if (!sprite.Animations.ContainsKey("turnOnRed"))
                    {
                        Sprite.Animation turnOnAnim = sprite.Animations["turnOn"];
                        sprite.Animations.Add("turnOnRed", Methods.CreateAnimation(@"Evidence02/objects/torch/torchRed",
                            0, 4, turnOnAnim.Delay, "onRed"));
                        //sprite.Animations.Add("onRed", Methods.CreateAnimation(@"Evidence02/objects/torch/torchRed",
                        //    5, 8, turnOnAnim.Delay));
                        sprite.AddLoop("onRed", turnOnAnim.Delay,
                            Methods.CreateTextureArray(@"Evidence02/objects/torch/torchRed", 5, 8));
                    }

                    Audio.Play(SFX.game_05_torch_activate, Position);
                    PlayBaddyModule.Instance.Session.listRedTorches.Add(dyn.Get<EntityID>("id"));
                    Color color_light  = Methods.GetColorBetween(Color.White, Color.Red, 0.8f);
                    Color color_sprite = Methods.GetColorBetween(Color.White, Color.Red, 0.55f);
                    dyn.Set("lit", true);
                    sprite.Play("turnOnRed");
                    sprite.Color = color_sprite;
                    torch.Collidable = false;

                    torch.Remove(dyn.Get<VertexLight>("light"));
                    torch.Add(new VertexLight(color_light, 1f, 48, 64));
                    torch.Remove(dyn.Get<BloomPoint>("bloom"));
                    //torch.Add(new BloomPoint(0.5f, 8));
                }
            }
            */

            // explode boosters just for lulz, yay
            Booster booster = level.Entities.FindAll<Booster>()
                .FirstOrDefault(t => t.CollideCheck(this));
            if (booster != null && InteractionController.ExplodeBooster(booster, 0.01f))
                RemoveSelf();

            // Gems!
            HeartGem heart = level.Entities.FindAll<HeartGem>()
                .FirstOrDefault(t => t.CollideCheck(this));
            if (heart != null)
            {
                Vector2 speed_norm_new = Vector2.Normalize(this.Center - heart.Center);
                this.Speed = 1.4f * this.Speed.Length() * speed_norm_new;
                this.Center += Speed * Engine.DeltaTime;

                // Visuals
                SetColor(ClassicBadelineShotColorEnum.Blue, is_damage_player);
                SetParticleColor(HeartGem.P_BlueShine, Color.White);
                for (int i = 0; i < 12; i++)
                    level.ParticlesFG.Emit(HeartGem.P_BlueShine,
                        heart.Center + Calc.AngleToVector(Calc.Random.NextAngle(), Calc.Random.Next(20, 30)));

                // Add 0.1 max power
                BaddyController.GetParams().AddMaxPower(0.1f);
                BaddyController.GetHUD().SetColorTemp(Color.AliceBlue);

                // Imitate bounce from player
                Player player = level.Tracker.GetEntity<Player>();
                if (Methods.PlayerIsAlive(player))
                    InteractionController.ImitateHeartGemBounce(heart, player, 20f * Vector2.Normalize(heart.Center - this.Center));
            }

            SummitGem gem = level.Entities.FindAll<SummitGem>()
                .FirstOrDefault(t => t.CollideCheck(this));
            if (gem != null)
            {
                // Collect gem and add 0.2 to max power
                Player player = level.Tracker.GetEntity<Player>();
                if (Methods.PlayerIsAlive(player))
                {
                    Audio.Play(SFX.game_gen_crystalheart_bounce, gem.Position);
                    //BaddyPowerController.AddMaxPower(0.2f);   // for some reason it doesn't wants to break from the first time...
                    //CharacterSwitchEntity baddy_entity = level.Entities.FindFirst<CharacterSwitchEntity>();
                    //if (baddy_entity != null) baddy_entity.GetBaddyComponent.SetColorTemp(Color.RoyalBlue);

                    // Visuals
                    SetColor(ClassicBadelineShotColorEnum.Blue, is_damage_player);
                    SetParticleColor(HeartGem.P_BlueShine, Color.White);
                    for (int i = 0; i < 4; i++)
                        level.ParticlesFG.Emit(HeartGem.P_BlueShine,
                            gem.Center + Calc.AngleToVector(Calc.Random.NextAngle(), Calc.Random.Next(20, 30)));

                    // no CollectRoutine? just imitate bounce from player, lol
                    InteractionController.ImitateSummitGemBounce(gem, player, 160f * Vector2.Normalize(gem.Center - this.Center));

                    RemoveSelf();
                }
            }

            // Fish! from farewell
            List<Puffer> list_puffers_alert = level.Entities.FindAll<Puffer>()
                .FindAll(t => Vector2.Distance(t.Center, this.Center) <= 40);
            foreach (Puffer puffer_alert in list_puffers_alert)
                InteractionController.PufferAlert(puffer_alert, 1.9f);

            Puffer puffer = level.Entities.FindAll<Puffer>()
                .FirstOrDefault(t => t.CollideCheck(this));
            if (puffer != null)
            {
                // Explode and restore fish!
                InteractionController.PufferExplode(puffer);
                is_explode = true;
            }
        }

        void CheckCollisionsCore()
        {
            CoreModeToggle core_toggle = level.Entities.FindAll<CoreModeToggle>()
                .FirstOrDefault(t => t.CollideCheck(this));
            if (core_toggle != null)
            {
                //On.Celeste.CoreModeToggle.OnPlayer;
                Player player = level.Tracker.GetEntity<Player>();
                if (Methods.PlayerIsAlive(player))
                    InteractionController.SwitchCoreModeToggle(core_toggle, player);

                //coremodetoggleOnChangeMode.Invoke(core_toggle, new object[] {
                //    (level.CoreMode == Session.CoreModes.Hot)
                //    ? Session.CoreModes.Cold
                //    : Session.CoreModes.Hot
                //});
                //RemoveSelf();
            }

            FireBall fireball = level.Entities.FindAll<FireBall>()
                .FirstOrDefault(t => t.CollideCheck(this));
            if (fireball != null)
            {
                DynData<FireBall> dyn = new DynData<FireBall>(fireball);
                InteractionController.SetCoreMode(fireball, dyn.Get<bool>("iceMode") ? Session.CoreModes.Hot : Session.CoreModes.Cold);
                is_explode = true;
            }

            // That fire block from core named just as "Bounce block", it was hard to find
            BounceBlock bounceblock = level.Entities.FindAll<BounceBlock>()
                .FirstOrDefault(t => t.CollideCheck(this) && t.Collidable);
            if (bounceblock != null)
            {
                DynData<BounceBlock> dyn = new DynData<BounceBlock>(bounceblock);
                InteractionController.SetCoreMode(bounceblock, dyn.Get<bool>("iceMode") ? Session.CoreModes.Hot : Session.CoreModes.Cold);
                is_explode = true;
            }

            WallBooster wall_booster = level.Entities.FindAll<WallBooster>()
                .FirstOrDefault(t => t.CollideCheck(this));
            if (wall_booster != null)
            {
                // Screw cold core movers, they're suck! Just hot and speed!
                InteractionController.SetCoreMode(wall_booster, Session.CoreModes.Hot);
                is_explode = true;
            }

            FireBarrier firebarrier = level.Entities.FindAll<FireBarrier>()
                .FirstOrDefault(t => t.CollideCheck(this));
            if (firebarrier != null && firebarrier.Collidable && level.CoreMode == Session.CoreModes.Hot)
            {
                InteractionController.SetCoreMode(firebarrier, Session.CoreModes.Cold);
                is_explode = true;
            }

            IceBlock iceblock = level.Entities.FindAll<IceBlock>()
                .FirstOrDefault(t => t.CollideCheck(this));
            if (iceblock != null && iceblock.Collidable && level.CoreMode == Session.CoreModes.Cold)
            {
                InteractionController.SetCoreMode(iceblock, Session.CoreModes.Hot);
                is_explode = true;
            }

            // Am I really need to switch lava from hot to cold?
            // I have a better idea!
            RisingLava lava = level.Entities.FindAll<RisingLava>()
                .FirstOrDefault(t => t.CollideCheck(this));
            if (lava != null && lava.Collidable)
            {
                //On.Celeste.RisingLava.OnChangeMode;
                //risingLavaOnChangeMode.Invoke(lava, new object[] { Session.CoreModes.Cold });
                for (int i = 0; i < 8; i++)
                {
                    float angle = 17 * MathExt.PI2 / 24 + Calc.Random.NextFloat(MathExt.PI2 / 12);
                    float length = Calc.Random.Next(90, 180);
                    Vector2 speed = Calc.AngleToVector(angle, length);
                    Vector2 center = this.Center + Calc.AngleToVector(Calc.Random.NextAngle(), Calc.Random.Next(0, 10));
                    level.Add(new ClassicBadelineShot(center, speed, ClassicBadelineShotColorEnum.Yellow, 
                        gravity: 360, 
                        is_collide_walls: false));
                }
                RemoveSelf();
            }

            SandwichLava lava_sand = level.Entities.FindAll<SandwichLava>()
                .FirstOrDefault(t => t.CollideCheck(this));
            if (lava_sand != null && lava_sand.Collidable)
            {
                for (int i = 0; i < 8; i++)
                {
                    float angle = 17 * MathExt.PI2 / 24 + Calc.Random.NextFloat(MathExt.PI2 / 12);
                    float length = Calc.Random.Next(90, 180);
                    int gravity = 1; //(level.CoreMode == Session.CoreModes.Hot ? 1 : -1);
                    Vector2 speed = Calc.AngleToVector(angle * gravity, length);
                    Vector2 center = this.Center + Calc.AngleToVector(Calc.Random.NextAngle(), Calc.Random.Next(0, 10));
                    level.Add(new ClassicBadelineShot(center, speed, ClassicBadelineShotColorEnum.Yellow, 
                        gravity: 360 * gravity, 
                        is_collide_walls: false
                        ));
                }
                RemoveSelf();
            }
        }
        
        protected override void CollideWall(Platform platform)
        {
            if (platform is DreamBlock)
                return;
            
            if (block_dream_current != null)
            {
                Audio.Play(SFX.game_assist_dreamblockbounce);
                Center -= Engine.DeltaTime * Speed;
                Speed *= -1.4f;
                is_dream_reflected = true;
                return;
            }

            base.CollideWall(platform);
            Explode();
        }

        void Explode()
        {
            List<Entity> list_spinners = level.Tracker.GetEntities<CrystalStaticSpinner>()
                .FindAll(t => Vector2.Distance(this.Center, t.Center) <= radius_explosion);
            if (list_spinners.Count > 0)
            {
                //if (list_spinners.Count >= 3)
                    Audio.Play(SFX.game_06_boss_spikes_burst);
                foreach (CrystalStaticSpinner spinner in list_spinners)
                    spinner.Destroy();
            }


            List<Snowball> list_snowballs = level.Entities.FindAll<Snowball>()
                .FindAll(t => Vector2.Distance(this.Center, t.Center) <= radius_explosion && t.Collidable);
            foreach (Snowball snowball in list_snowballs)
            {
                Audio.Play(SFX.game_04_snowball_impact);
                DynData<Snowball> dyn = new DynData<Snowball>(snowball);
                Sprite sprite = dyn.Get<Sprite>("sprite");
                snowball.Collidable = false;
                sprite.Play("break");

                //Vector2 speed = dyn.Get<Vector2>("Speed");
                //speed = 1.4f * speed.Length() * Vector2.Normalize(this.Center - snowball.Center);
            }
            


            // Jellyfishes from Farewell
            List<Glider> list_jellyfishes = level.Entities.FindAll<Glider>()
                .FindAll(t => Vector2.Distance(this.Center, t.Center) <= radius_explosion && t.Collidable);
            foreach (Glider jellyfish in list_jellyfishes)
            {
                Audio.Play(SFX.game_10_glider_engage);
                //DynData<Glider> dyn = new DynData<Glider>(jellyfish);
                //Sprite sprite = dyn.Get<Sprite>("sprite");
                //sprite.Play("fall");

                Vector2 speed_new = Vector2.Normalize(jellyfish.Center - this.Center);
                float distance = (jellyfish.Center - this.Center).Length();
                float length = 60 + 3 * (radius_explosion - distance);
                jellyfish.Speed = speed_new * length;
                if (jellyfish.Speed.Y > -20f)
                    jellyfish.Speed.Y = -20f;
                //Vector2 speed = dyn.Get<Vector2>("Speed");
                //speed = 1.4f * speed.Length() * Vector2.Normalize(this.Center - snowball.Center);
            }


            // Fishes!
            List<Puffer> list_puffers = level.Entities.FindAll<Puffer>()
                .FindAll(t => Vector2.Distance(t.Center, this.Center) <= radius_explosion && t.Collidable);
            foreach (Puffer puffer in list_puffers)
                InteractionController.PufferExplode(puffer);


            // Overpowered badeline?!!!!
            /*
            if (false)
            {
                // Deflect baddy shots in some radius
                // UPD: no, that's too confusing... probably?
                foreach (FinalBossShot shot in level.Entities.FindAll<FinalBossShot>())
                {
                    Vector2 from_player = shot.Center - this.Center;
                    float length = from_player.Length();
                    float explode_power = 120 * (length <= 40 ? 1 : ((120 - length) / 80));
                    //Methods.DebugLogHiddenFields(typeof(FinalBossShot), shot);
                    if (explode_power > 0)
                        InteractionController.SetBadelineBossShotSpeed(shot, explode_power * Vector2.Normalize(from_player));
                }

                // Remove trash from hotel
                List<DustStaticSpinner> list_blocks_bunnies = level.Entities.FindAll<DustStaticSpinner>()
                    .FindAll(t => Vector2.Distance(this.Center, t.Center) <= radius_explosion);
                foreach (DustStaticSpinner bnuuy in list_blocks_bunnies)
                    bnuuy.RemoveSelf();

                List<DustTrackSpinner> list_tracks_bunnies = level.Entities.FindAll<DustTrackSpinner>()
                    .FindAll(t => Vector2.Distance(this.Center, t.Center) <= radius_explosion);
                foreach (DustTrackSpinner bnuuy in list_tracks_bunnies)
                    bnuuy.RemoveSelf();
                
                List<DustRotateSpinner> list_rotate_bunnies = level.Entities.FindAll<DustRotateSpinner>()
                    .FindAll(t => Vector2.Distance(this.Center, t.Center) <= radius_explosion);
                foreach (DustRotateSpinner bnuuy in list_rotate_bunnies)
                    bnuuy.RemoveSelf();

                // Clutter blocks from ch3
                //List<ClutterBlock> list_blocks_clutter = level.Entities.FindAll<ClutterBlock>()
                //    .FindAll(t => Vector2.Distance(this.Center, t.Center) <= radius_explosion && t.Collidable);
                //foreach (ClutterBlock block in list_blocks_clutter)
                //    block.RemoveSelf();
                
                //List<ClutterBlockBase> list_base_clutter = level.Entities.FindAll<ClutterBlockBase>()
                //    .FindAll(t => Vector2.Distance(this.Center, t.Center) <= radius_explosion && t.Collidable);
                //foreach (ClutterBlockBase block in list_base_clutter)
                //{
                //    block.RemoveSelf();
                //    foreach (ClutterBlock block_ in level.Entities.FindAll<ClutterBlock>().FindAll(t => t.CollideCheck(block)))
                //        block_.RemoveSelf();
                //}
            }
            */

            level.Displacement.AddBurst(Center, 0.4f, 8, 64, 0.5f, Ease.QuadOut, Ease.QuadOut);
            RemoveSelf();
        }
    }
}
