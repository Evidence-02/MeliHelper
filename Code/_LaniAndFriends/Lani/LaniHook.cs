using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._Lani
{   
    class LaniHook : Entity
    {
        private static MethodInfo playerClimbBegin = typeof(Player).GetMethod("ClimbBegin", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo playerCarryOffset = typeof(Player).GetField("carryOffset", BindingFlags.NonPublic | BindingFlags.Instance);
        private static FieldInfo playerMoveX = typeof(Player).GetField("moveX", BindingFlags.NonPublic | BindingFlags.Instance);
        
        Level level;
        Player player;
        LaniHookParams _params;
        LaniEnum_State state;
        LaniEnum_Grab state_grab;
        Solid grab_solid, grab_solid_fly;
        Entity grab_entity;
        LaniThrowableObject grab_object;
        Vector2 grab_offset, hold_solid_player_center;

        Vector2 direction, hook_origin, loc_start, loc_end;
        float max_length_cur;
        float state_timer, koef_sinusoidal, length, length_grab, hook_speed_pkoef, cooldown;
        float time_grab;
        bool is_player_already_jumped;


        public LaniHook(Player player, Vector2 direction, LaniHookParams hook_params)
        {
            this.player = player;
            this._params = hook_params;
            this.hook_origin = direction * 5;
            this.direction = direction;
            this.loc_start = player.Center + hook_origin;
            this.loc_end = loc_start;
            this.length = 0;
            this.max_length_cur = 40;
            this.hook_speed_pkoef = 1;
            this.cooldown = hook_params.Cooldown;
            this.koef_sinusoidal = 0;
            this.state = LaniEnum_State.Start;
            this.state_grab = LaniEnum_Grab.Nothing;
            // default color: Color.Pink * 0.77f; // Color.Blue * 0.5f

            //if (MeliHelperModule.Settings.Debug_LaniHookSpeed10 > 0)
            //    _params.Speed = 10 * MeliHelperModule.Settings.Debug_LaniHookSpeed10;
            //if (MeliHelperModule.Settings.Debug_LaniHookSpeedReturn10 > 0)
            //    _params.SpeedMovePlayer = 10 * MeliHelperModule.Settings.Debug_LaniHookSpeedReturn10;
            //if (MeliHelperModule.Settings.Debug_LaniHookLength10 > 0)
            //    _params.Length = 10 * MeliHelperModule.Settings.Debug_LaniHookLength10;
            
            Audio.Play(SFX.char_mad_dash_red_left);
            Depth = (MeliHelperModule.Settings.Debug_LaniHookShowInfo) ? -9999999 : (player.Depth - 1);
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = scene as Level;

            // Lock player inputs just like in cutscenes
            player.StateMachine.State = Player.StDummy;
            player.ForceCameraUpdate = true;

            // Move hook start at center of player, if already have something to grab
            state_grab = CheckCollision(loc_start, true, 0, ref grab_solid, ref grab_entity, ref grab_object);
            if (state_grab != LaniEnum_Grab.Nothing)
            {
                for (int i = 0; i < 10; i++)
                    if (grab_solid.CollidePoint(loc_start))
                        loc_start -= 2 * direction;
            }
        }

        public override void Update()
        {
            base.Update();
            if (level == null || player == null || player.Dead)
            {
                RemoveSelf();
                return;
            }

            time_grab += Engine.DeltaTime; 
            if (state != LaniEnum_State.Release && player.StateMachine.State != Player.StDummy)
                SetStateRelease();

            // States
            switch (state)
            {
                case LaniEnum_State.Start:
                    if (Input.Dash.Check && max_length_cur < _params.Length)
                        max_length_cur = (int)Math.Min(_params.Length, length + 16);
                    length += _params.Speed * Engine.DeltaTime;
                    loc_end = loc_start + direction * length;
                    player.Center = loc_start - hook_origin;
                    player.Speed = Vector2.Zero;
                    
                    state_grab = CheckCollision(loc_end, false, 2, ref grab_solid, ref grab_entity, ref grab_object);
                    if (state_grab == LaniEnum_Grab.Nothing && !level.IsInBounds(loc_end))
                        state_grab = LaniEnum_Grab.Bounce;
                    switch (state_grab)
                    {
                        case LaniEnum_Grab.Nothing:
                            if (length >= max_length_cur)
                                SetStateRelease(0.4f);
                            break;

                        case LaniEnum_Grab.Bounce:
                            SetStateRelease(0.5f);
                            break;

                        case LaniEnum_Grab.Explode:
                            // Nyooom!!!
                            Booster booster = grab_entity as Booster;
                            bool is_red = false;
                            float explode_power = is_red ? 600 : 480;
                            player.Speed = -explode_power * Vector2.Normalize(grab_entity.Center - player.Center);
                            SetStateRelease(0.1f);
                            break;

                        case LaniEnum_Grab.GrabObject:
                            SetStateRelease();
                            time_grab = 0;
                            break;

                        // All hook states: hook air, solid, solid top and solid bottom
                        default:
                            ActivateBlockAfterHookCollision(grab_solid);
                            if (state_grab == LaniEnum_Grab.HookAir)
                            {
                                if (grab_entity is LaniHookSpot)
                                    loc_end = grab_entity.Center;

                                // length from 12 to 48 --> speed koef from 0.1 to 1.0
                                float len = Vector2.Distance(loc_end, player.Center);
                                if (len < 48)
                                    hook_speed_pkoef = 0.1f + 0.9f * Math.Max(0, (len - 12)) / (48 - 12);
                            }


                            //Logger.Log("LaniHook", $"length before check collision={length}");
                            state_timer = 0.12f;
                            state = LaniEnum_State.Wait;
                            if (grab_solid != null)
                            {
                                for (int i = 0; i < 10; i++)
                                    if (grab_solid.CollidePoint(loc_end) && length > 8)
                                    {
                                        loc_end -= direction;
                                        length = Vector2.Distance(loc_end, loc_start);
                                    }
                                grab_offset = loc_end - grab_solid.Center;
                            }

                            length_grab = length;
                            //Logger.Log("LaniHook", $"length after  check collision={length};  length_grab={length_grab}");
                            if (grab_entity != null)
                                grab_offset = loc_end - grab_entity.Center;
                            break;
                    }
                    break;

                case LaniEnum_State.Wait:
                    player.Center = loc_start - hook_origin;
                    player.Speed = Vector2.Zero;
                    if (grab_solid != null)
                        loc_end = grab_solid.Center + grab_offset;
                    if (grab_entity != null)
                        loc_end = grab_entity.Center + grab_offset;

                    state_timer -= Engine.DeltaTime;
                    koef_sinusoidal += Engine.DeltaTime;    // 0 -> 0.12f
                    if (state_timer <= 0)
                        state = LaniEnum_State.MovePlayer;
                    break;

                case LaniEnum_State.MovePlayer:
                    Vector2 dir_speed = Vector2.Normalize(loc_end - loc_start);
                    loc_start += dir_speed * _params.SpeedMovePlayer * hook_speed_pkoef * Engine.DeltaTime;
                    length = Vector2.Distance(loc_end, loc_start);

                    if (koef_sinusoidal < 1f)
                        koef_sinusoidal += 2 * Engine.DeltaTime;    // 0.12f -> 0.92f
                    if (grab_solid != null)
                        loc_end = grab_solid.Center + grab_offset;
                    if (grab_entity != null)
                        loc_end = grab_entity.Center + grab_offset;
                    
                    // !debug!
                    //player.Speed.Y = 0;
                    // !!!!!!!

                    LaniEnum_Grab grab_error = LaniEnum_Grab.Nothing;
                    if (length >= 10)
                    {
                        grab_error = CheckCollision(loc_start, true, 0, ref grab_solid_fly, ref grab_entity, ref grab_object);
                        if (grab_error == LaniEnum_Grab.Nothing)
                            grab_error = CheckCollision(loc_start + new Vector2(0, 4), true, 0, ref grab_solid_fly, ref grab_entity, ref grab_object);
                        if (grab_error == LaniEnum_Grab.Nothing)
                            grab_error = CheckCollision(loc_start - new Vector2(0, 4), true, 0, ref grab_solid_fly, ref grab_entity, ref grab_object);
                    }

                    if (grab_error == LaniEnum_Grab.HookSolid && grab_solid != grab_solid_fly)
                    {
                        // player collided with another solid, release hook and player
                        SetStateRelease(0.2f);
                        ActivateBlockAfterPlayerCollision(grab_solid_fly);
                    }
                    else if (length > 0.66f * length_grab)
                    {
                        // Just move first 1/3 path on you're own
                        player.Speed = (loc_start - hook_origin * length / length_grab - player.Center) / Engine.DeltaTime;
                    }
                    else if (Input.Jump.Check && hook_speed_pkoef > 0.6f && !is_player_already_jumped)
                    {
                        // Jump!
                        Vector2 player_speed = new Vector2(
                            0.85f * dir_speed.X * _params.SpeedMovePlayer * hook_speed_pkoef, 
                            Math.Min(-200, dir_speed.Y * _params.SpeedMovePlayer * hook_speed_pkoef - 240)
                            );
                        //Logger.Log("LaniHook", $"PreHook Jump: dir_speed={dir_speed};  (before) player.Speed={player.Speed},  (after) {player_speed}");
                        SetStateRelease(0.2f);
                        player.Speed = player_speed;
                        is_player_already_jumped = true;
                    }
                    else if (!Input.Dash.Check)
                    {
                        // Release grab immediately
                        player.Speed = dir_speed * _params.SpeedMovePlayer * hook_speed_pkoef;
                        SetStateRelease(0.2f);
                    }
                    else
                    {
                        player.Speed = (loc_start - hook_origin * length / length_grab - player.Center) / Engine.DeltaTime;
                        if (length <= 4)
                        {
                            // Grab something
                            loc_start = loc_end;
                            ActivateBlockAfterPlayerCollision(grab_solid);
                            switch (state_grab)
                            {
                                case LaniEnum_Grab.HookSolid:
                                    if (player.StateMachine.State != Player.StDreamDash)
                                    {
                                        state = LaniEnum_State.HoldToSolid;
                                        for (int i = 0; i < 10; i++)
                                            if (!grab_solid.CollideCheck(player))
                                                player.Center += direction;
                                        player.Center -= direction;
                                        hold_solid_player_center = player.Center;

                                        // Actually Lani cannot climb, she have "No grabbing" option in variant mode
                                        // I made this for a beautiful sliding down the wall
                                        player.Ducking = false;
                                        player.StateMachine.State = Player.StClimb;
                                        playerClimbBegin.Invoke(player, new object[] { });
                                        Methods.PlayerFixSubpixels(player);
                                    }
                                    break;

                                case LaniEnum_Grab.HookSolidTop:
                                    Logger.Log("LaniHook", $"HookSolidTop, speed: {player.Speed.Length()}");
                                    //player.Center -= new Vector2(0, 6 + player.Height / 2);
                                    //player.Speed.X *= 1.25f;
                                    player.Center -= new Vector2(0, 6 + player.Height / 2);
                                    player.Speed.X *= 0.80f;
                                    SetStateRelease();
                                    break;

                                case LaniEnum_Grab.HookSolidBottom:
                                    Logger.Log("LaniHook", $"HookSolidBottom, speed: {player.Speed.Length()}");
                                    //player.Center += new Vector2(0, 6 + player.Height / 2);
                                    //player.Speed.X *= 1.4f;
                                    player.Center += new Vector2(0, 6 + player.Height / 2);
                                    player.Speed.X *= 0.75f;
                                    if (player.DuckFreeAt(new Vector2(0, -1)))
                                        player.Ducking = true;
                                    SetStateRelease();
                                    break;

                                case LaniEnum_Grab.HookSolidUpward:
                                    //Logger.Log("LaniHook", "HookSolidUpward");
                                    player.Speed.X *= 1.2f;
                                    SetStateRelease();
                                    break;

                                case LaniEnum_Grab.HookSolidDownward:
                                    //Logger.Log("LaniHook", "HookSolidDownward");
                                    player.Speed.X *= 1.1f;
                                    if (direction.X != 0 && player.DuckFreeAt(new Vector2(0, -1)))
                                        player.Ducking = true;
                                    SetStateRelease();
                                    break;

                                case LaniEnum_Grab.HookAir:
                                    player.Speed.X *= 1.1f;
                                    SetStateRelease(0.16f);
                                    break;
                            }
                        }
                    }
                    break;

                case LaniEnum_State.HoldToSolid:
                    // shouldn't work, lani cannot grab
                    //player.Center = hold_solid_player_center;
                    //player.Speed = Vector2.Zero;
                    if (!Input.Dash.Check)
                        SetStateRelease();
                    else if (Input.Jump.Pressed && !is_player_already_jumped)
                    {
                        player.Jump();
                        SetStateRelease();
                    }
                    break;


                case LaniEnum_State.Release:
                    if (koef_sinusoidal > 0)
                        koef_sinusoidal = Math.Max(0, koef_sinusoidal - 5 * Engine.DeltaTime);
                    switch (state_grab)
                    {
                        // Move grabbed object to player and hold after
                        case LaniEnum_Grab.GrabObject:
                            if (grab_object == null || grab_object.GetHoldable == null || !Input.Dash.Check)
                            {
                                if (grab_object != null)
                                {
                                    grab_object.SetHoldable(false);
                                    if (grab_object.Speed.Length() >= 135)
                                        grab_object.Speed = 135 * Vector2.Normalize(grab_object.Speed);
                                }
                                Finish();   // UPD: SetStateRelease instead, maybe?
                                return;
                            }

                            loc_start = player.Center + hook_origin;

                            float attract_koef = (time_grab < 0.1f) ? 0.2f :
                                                 (time_grab > 0.4f) ? 0.8f :
                                                 (0.2f + 0.6f * (time_grab - 0.1f) / 0.3f);
                            loc_end += attract_koef * (loc_start - loc_end);
                            

                            if (Vector2.Distance(loc_end, loc_start) > 8)
                                grab_object.Speed = (loc_end - grab_object.Center) / Engine.DeltaTime;
                            else
                            {
                                // Pickup grabbed object, and force-allow pickup
                                state_grab = LaniEnum_Grab.HoldObject;
                                loc_end = loc_start;
                                //playerOnPickup.Invoke(player, new object[] { grab_entity.GetHoldable });
                                //playerHoldTimer.SetValue(player, 0.05f);

                                // force-allow pickup
                                InteractionController.PlayerPickupHoldable(player, grab_object.GetHoldable);
                            }
                            break;
                            
                        case LaniEnum_Grab.HoldObject:
                            if (grab_object == null || grab_object.GetHoldable == null || player.Holding == null || !Input.Dash.Check)
                                Finish();
                            else
                                InteractionController.PlayerSetPickupTimer(player, 0.05f);
                            break;

                        // Destroy the hook after 0.4f seconds
                        default:
                            loc_start = player.Center + hook_origin;
                            loc_end += 0.12f * (loc_start - loc_end);
                            state_timer -= Engine.DeltaTime;
                            cooldown -= Engine.DeltaTime;
                            if (state_timer <= 0)
                            {
                                if (cooldown <= 0)
                                    Finish();
                                else
                                {
                                    state = LaniEnum_State.Cooldown;
                                    Visible = false;
                                }   
                            }
                            
                            if (Math.Abs(player.Speed.X) >= 300)
                                player.Speed.X *= 0.9f;

                            ////if (Math.Abs(player.Speed.X) >= 300)
                            //{
                            //    float koef = (Input.MenuLeft.Check && player.Speed.X > 0 || Input.MenuRight.Check && player.Speed.X < 0)
                            //        ? 0.86f : 0.98f;

                            //    Logger.Log("LaniHook", $"player.Speed.X={player.Speed.X};  left={Input.MenuLeft.Check};  right={Input.MenuRight.Check}; koef={koef}");

                            //    player.Speed.X *= koef;
                            //    if (koef < 0.9f && Math.Abs(player.Speed.X) > 420)
                            //        player.Speed.X *= 0.7f;
                            //}


                            // Player can jump after air hook
                            if (state_grab == LaniEnum_Grab.HookAir && Input.Jump.Check && hook_speed_pkoef > 0.6f && !is_player_already_jumped)
                            {
                                // Jump!
                                Vector2 player_speed = new Vector2(0.85f * player.Speed.X,
                                    Math.Min(-200, player.Speed.Y - 240));
                                //Logger.Log("LaniHook", $"AfterHook Jump: (before) player.Speed={player.Speed},  (after) {player_speed}");
                                player.Speed = player_speed;
                                is_player_already_jumped = true;
                                //Finish();
                            }
                            break;
                    }
                    break;

                case LaniEnum_State.Cooldown:
                    cooldown -= Engine.DeltaTime;
                    if (cooldown < 0)
                        Finish();
                    break;
            }
        }

        void Finish()
        {
            if (Methods.PlayerIsAlive(player))
                Methods.PlayerFixSubpixels(player);
            RemoveSelf();
        }

        void SetStateRelease(float timer = 0f)
        {
            state = LaniEnum_State.Release;
            state_timer = timer; //(cooldown > timer) ? cooldown : timer;
            player.RefillDash();
            if (player.StateMachine.State == Player.StDummy)
            {
                // Unlock player inputs, just like in cutscenes
                //player.StateMachine.Locked = false;
                player.StateMachine.State = 0;
                player.ForceCameraUpdate = false;
                Methods.PlayerFixSubpixels(player);
            }
        }

        public float GetCooldown
        {
            get
            {
                return cooldown;
            }
        }
        
        void ActivateBlockAfterHookCollision(Solid solid)
        {
            if (solid == null) return;

            if (grab_solid is ZipMover)
                InteractionController.ActivateBlock(grab_solid as ZipMover, player);

            if (grab_solid is MoveBlock)
                InteractionController.ActivateBlock(grab_solid as MoveBlock, player);

            if (grab_solid is SwapBlock)
                InteractionController.ActivateBlock(grab_solid as SwapBlock, player);

            if (grab_solid is DreamBlock)
                (grab_solid as DreamBlock).FootstepRipple(loc_end);
            


            // Entities
            if (grab_entity is HeartGem)
                InteractionController.ImitateHeartGemBounce(grab_entity as HeartGem, player, 80f * Vector2.Normalize(grab_entity.Center - loc_end));
            
            if (grab_entity is SummitGem)
                InteractionController.ImitateSummitGemBounce(grab_entity as SummitGem, player, 20f * Vector2.Normalize(grab_entity.Center - loc_end));
        }

        void ActivateBlockAfterPlayerCollision(Solid solid)
        {
            if (solid != null)
            {
                if (solid is DashBlock)
                    InteractionController.ActivateBlock(solid as DashBlock, player);

                if (solid is FallingBlock)
                    InteractionController.ActivateBlock(solid as FallingBlock, player);

                if (solid is TempleCrackedBlock)
                    InteractionController.ActivateBlock(solid as TempleCrackedBlock, player);

                if (solid is CrushBlock)
                    InteractionController.ActivateBlockKevin(solid as CrushBlock, player, 1f);

                if (solid is LightningBreakerBox)
                    InteractionController.ActivateBlock(solid as LightningBreakerBox, player, Vector2.Zero);

                if (solid is DashSwitch)
                    InteractionController.ActivateDashSwitch(solid as DashSwitch);

                if (solid is ClutterSwitch)
                    InteractionController.ActivateClutterSwitch(solid as ClutterSwitch, this.direction);

                if (solid is CrumblePlatform)
                {
                    // Break this crumble platform and 
                    InteractionController.ActivateCrumblePlatform(solid as CrumblePlatform, player);

                    Vector2 center = grab_solid.Center;
                    Vector2 perp = new Vector2(8 * Math.Sign(direction.Y), -8 * Math.Sign(direction.X));
                    for (int i = -2; i <= 2; i++)
                    {
                        CrumblePlatform block_crumble = level.Entities.FindAll<CrumblePlatform>()
                            .FirstOrDefault(t => t.CollidePoint(center + i * perp));
                        if (block_crumble != null)
                            InteractionController.ActivateCrumblePlatform(block_crumble, player);
                    }
                }

                if (solid is DreamBlock && InteractionController.DashIntoDreamBlock(solid as DreamBlock, player))
                    Finish();
            }


            // Entities
            if (grab_entity != null)
            {
                if (grab_entity is HeartGem)
                    InteractionController.ImitateHeartGemCollect(grab_entity as HeartGem, player);

                if (grab_entity is SummitGem)
                    InteractionController.ImitateSummitGemBounce(grab_entity as SummitGem, player, 160f * Vector2.Normalize(grab_entity.Center - player.Center));
            }
        }

        LaniEnum_Grab CheckCollision(Vector2 loc, bool is_check_solids_only, int perp_offset,
            ref Solid solid, ref Entity entity, ref LaniThrowableObject obj)
        {
            // Solid platforms
            solid = level.CollideFirst<Solid>(loc);
            if (solid == null && perp_offset > 0) solid = level.CollideFirst<Solid>(loc - perp_offset * Calc.Perpendicular(direction));
            if (solid == null && perp_offset > 0) solid = level.CollideFirst<Solid>(loc + perp_offset * Calc.Perpendicular(direction));
            if (solid != null)
            {
                if (solid is LaniIceBlock)
                    return LaniEnum_Grab.Bounce;

                //if (direction.Y == 0 && solid.Height >= 24)

                // solid.Height doesn't works because the size of ForegroundTiles is the whole map!
                // I deleted this because of different behaviour on 1-tile height Foreground Tiles and any custom block
                if (direction.Y == 0 && solid.Height >= 24 && _params.isAllowHypers && !(solid is DashBlock) && !(solid is CrushBlock))
                {
                    Vector2 loc_dist = new Vector2(0, 2);
                    if (level.CollideFirst<Solid>(loc - loc_dist) == null && level.CollideFirst<Solid>(loc - 2 * loc_dist) == null)
                        return LaniEnum_Grab.HookSolidTop;
                    if (level.CollideFirst<Solid>(loc + loc_dist) == null && level.CollideFirst<Solid>(loc + 2 * loc_dist) == null)
                        return LaniEnum_Grab.HookSolidBottom;
                }

                if (direction.Y < 0 && level.CollideFirst<Solid>(loc + new Vector2(0, 8)) == null)
                    return LaniEnum_Grab.HookSolidUpward;
                if (direction.Y > 0 && level.CollideFirst<Solid>(loc - new Vector2(0, 8)) == null)
                    return LaniEnum_Grab.HookSolidDownward;

                return LaniEnum_Grab.HookSolid;
            }

            if (is_check_solids_only) return LaniEnum_Grab.Nothing;



            // 2. Hook point - set loc_end after collision
            //                                                                                            // length 0 -> 120   ==>   radius 8 -> 14 
            //LaniHookPoint point = level.Entities.FindAll<LaniHookPoint>().FirstOrDefault(t => Vector2.Distance(loc, t.Center) <= 8 + length / 20);

            List<LaniHookSpot> list_spots = level.Entities.FindAll<LaniHookSpot>().FindAll(t => Vector2.Distance(loc, t.Center) <= t.GetRadius);
            if (list_spots.Count > 0)
            {
                entity = list_spots.OrderBy(t => Vector2.Distance(loc, t.Center)).ToList()[0];
                if (entity != null) return LaniEnum_Grab.HookAir;
            }
            


            // 3. Vanilla entities you can hook
            // used: DashSwitch, ClutterSwitch, Snowball, Seeker, Bumper, FireBall, Puffer, FinalBoss, HeartGem, SummitGem, Booster

            // DustTrackSpinner, DustRotateSpinner, TheoCrystal, BadelineBoost?
            // AngryOshiro, TempleBigEyeball
            // TrackSpinner, FlyFeather, , ,  
            //entity = level.Entities.FindAll<DashSwitch>().FirstOrDefault(t => Vector2.Distance(t.Center, loc) <= 12); // t.CollidePoint(loc));
            //if (entity != null) return HookGrabEnum.HookAir;

            //entity = level.Entities.FindAll<ClutterSwitch>().FirstOrDefault(t => Vector2.Distance(t.Center, loc) <= 12); //t.CollidePoint(loc));
            //if (entity != null) return HookGrabEnum.HookAir;

            entity = level.Entities.FindAll<Snowball>().FirstOrDefault(t => Vector2.Distance(t.Center, loc) <= 12);
            if (entity != null) return LaniEnum_Grab.HookAir;
            
            entity = level.Entities.FindAll<Seeker>().FirstOrDefault(t => Vector2.Distance(t.Center, loc) <= 12);
            if (entity != null) return LaniEnum_Grab.HookAir;

            entity = level.Entities.FindAll<Bumper>().FirstOrDefault(t => Vector2.Distance(t.Center, loc) <= 12);
            if (entity != null) return LaniEnum_Grab.HookAir;

            entity = level.Entities.FindAll<FireBall>().FirstOrDefault(t => Vector2.Distance(t.Center, loc) <= 12);
            if (entity != null) return LaniEnum_Grab.HookAir;

            entity = level.Entities.FindAll<Puffer>().FirstOrDefault(t => Vector2.Distance(t.Center, loc) <= 8);
            if (entity != null) return LaniEnum_Grab.HookAir;

            entity = level.Entities.FindAll<FinalBoss>().FirstOrDefault(t => Vector2.Distance(t.Center, loc) <= 16);
            if (entity != null) return LaniEnum_Grab.HookAir;

            entity = level.Entities.FindAll<HeartGem>().FirstOrDefault(t => Vector2.Distance(t.Center, loc) <= 16);
            if (entity != null) return LaniEnum_Grab.HookAir;

            entity = level.Entities.FindAll<SummitGem>().FirstOrDefault(t => Vector2.Distance(t.Center, loc) <= 8);
            if (entity != null) return LaniEnum_Grab.HookAir;

            
            // 3.1. Boosters! (explosion)
            entity = level.Entities.FindAll<Booster>().FirstOrDefault(t => Vector2.Distance(t.Center, loc) <= 12);
            if (entity != null && InteractionController.ExplodeBooster(entity as Booster, 0.01f)) return LaniEnum_Grab.Explode;

            





            // 4. Grabbable custom objects
            obj = level.Entities.FindAll<LaniThrowableObject>().FirstOrDefault(t => Vector2.Distance(loc, t.Center) <= 8);
            if (obj != null)
            {
                obj.SetHoldable();
                return LaniEnum_Grab.GrabObject;
            }

            return LaniEnum_Grab.Nothing;
        }

        public override void Render()
        {
            float dist = Vector2.Distance(loc_end, loc_start);
            Vector2 dir = Vector2.Normalize(loc_end - loc_start);
            Vector2 perp = (direction.Y == 0) ? new Vector2(0, 1) : new Vector2(1, 0);

            // Horizontal linear 
            for (int i = 0; i < dist; i++)
            {
                Vector2 loc = loc_end - dir * i;
                if (koef_sinusoidal == 0)
                    loc += new Vector2(0, 1 - (i / 40) % 2);
                else
                    loc += perp * koef_sinusoidal * (dist / 4) * (float)Math.Sin(4 * Math.PI * i / dist);

                Draw.Rect(loc, 1, 1, _params.Color);
                Draw.Rect(loc - perp, 1, 1, Color.White);
            }

            if (MeliHelperModule.Settings.Debug_LaniHookShowInfo)
            {
                Draw.Rect(loc_end - new Vector2(2, 2), 4, 4, Color.White);
                Draw.Rect(loc_end - new Vector2(1, 1), 2, 2, Color.Red);

                Draw.Rect(loc_start - new Vector2(2, 2), 4, 4, Color.White);
                Draw.Rect(loc_start - new Vector2(1, 1), 2, 2, Color.Violet);

                if (Methods.PlayerIsAlive(player))
                {
                    Draw.Rect(player.Center - new Vector2(2, 2), 4, 4, Color.White);
                    Draw.Rect(player.Center - new Vector2(1, 1), 2, 2, Color.Green);

                    FontControllerOutline.DrawText(player.Center + new Vector2(0, -48), this.state.ToString(), Color.White);
                    FontControllerOutline.DrawText(player.Center + new Vector2(0, -36), this.state_grab.ToString(), Color.White);
                    FontControllerOutline.DrawText(player.Center + new Vector2(0, -24), $"speed=({player.Speed.X}, {player.Speed.Y})", Color.White);
                }

            }

            //// debug
            //if (player != null)
            //{
            //    Methods.DrawText(player.Center + new Vector2(0, 00), state.ToString(), Color.White);
            //    Methods.DrawText(player.Center + new Vector2(0, 12), state_timer.ToString(), Color.White);
            //    Methods.DrawText(player.Center + new Vector2(0, 24), "cooldown: " + cooldown.ToString(), Color.White);
            //}
        }
    }
}
