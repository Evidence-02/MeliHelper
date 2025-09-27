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


    enum DirectionEnum { Left, Right, Up, Down }
    enum TextAlignment { Left, Center, Right }
    public enum ConditionEnum { PlayerInside, PlayerOnLeft, PlayerOnRight, BerryCollected }

}
