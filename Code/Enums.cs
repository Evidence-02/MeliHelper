using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    enum BCEnum_GameState { Normal, Pause, Win, Gameover }
    enum BCEnum_Goal { Nothing, KillEnemies, KillBoss, CollectStorby }
    enum BCEnum_FinishEvent { Nothing, Endscreen, FastTeleport }
    enum BCEnum_BackgroundType { Default, None }


    enum BCEnum_CellType {
        Empty,
        Brick, Steel, Water, Grass, Dirt,
        Brick02,
        Blocked
    }
    
    enum BCEnum_BonusType {
        Star, Grenade, Shield, Shovel, ExtraLife, TimeStop,
        EMI, Duality, HomingBullets, UnlimitedShooting, MoveThroughWater,
        DirtBall, DemolitionBomb, Mine
    }

    enum BCEnum_BonusEvent {
        Shield, Shovel, TimeStop,
        EMI, Duality, HomingBullets, UnlimitedShooting
    }

    enum BCEnum_EnemyState { Wait, Move }
    enum BCEnum_EnemyBehaviour { Random, PrioritizePlayer, PrioritizeFlag, FreakyPlayer, FreakyFlag }
    enum BCEnum_EnemyType { Basic, Fast, Power, Armored }


    enum Minesweeper_CellMark { None, Flag, Question }




    enum LaniEnum_State { Start, Wait, MovePlayer, HoldToSolid, Release, Cooldown }
    enum LaniEnum_Grab
    {
        Nothing, Bounce, Explode,
        HookSolid, HookSolidTop, HookSolidBottom, HookSolidUpward, HookSolidDownward,
        HookEntity, HookAir,
        GrabObject, HoldObject
    }






    enum RCMEnum_CubeType
    {
        DinoCube, Skewb, CloverCube,
        FloppyCube, Cube222, Cube223, Cube233, Cube333, Cube234,
        Mirror, Mirror22, MirrorFloppy,
        PetalPyraminx,
        Kilominx, Megaminx,
        // unused
        AxisCube
    }
    enum RCMEnum_CubeState
    {
        Normal, Scramble, Descramble, RestorePrevious, Solved
    }
    enum RCMEnum_AxisType
    {
        Front, Back, Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight,
        AxisX, AxisY, AxisZ,
        RotateCenter,
        None
    }
    enum RCMEnum_AxisDirection
    {
        All, Normal
    }
    enum RCMEnum_ControlScheme
    {
        Main, Scheme1, Scheme2, Scheme3
    }



    enum DirectionEnum { Left, Right, Up, Down }
    enum TextAlignment { Left, Center, Right }
    enum ClassicBadelineShotColorEnum { Red, Violet, Yellow, Blue, White, Black }
    public enum ConditionEnum { PlayerInside, PlayerOnLeft, PlayerOnRight, BerryCollected }
    enum UIButtonEnum { Dash, Talk, Up, Down, Left, Right }
    enum UIButtonActionEnum { GoNext, GoPrev, GoTo0, Save, Exit, Another, None }
}
