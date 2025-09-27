using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class SoundController
    {
        static string BC_PREFIX = "event:/meli/bc_";

        public static string BC_STARTUP              = BC_PREFIX + "startup";                               // +
        public static string BC_PAUSE                = BC_PREFIX + "pause";
        public static string BC_PLAYER_TANK_FIRING        = BC_PREFIX + "player_tank_firing";               // +
        public static string BC_FIRING_AT_THE_WALL        = BC_PREFIX + "firing_at_the_wall";               // ?
        public static string BC_FIRING_AT_THE_BRICKS      = BC_PREFIX + "firing_at_the_bricks";             // ?
        public static string BC_FIRING_THE_ENEMY_BIG_TANK = BC_PREFIX + "firing_the_enemy_big_tank";        // +
        public static string BC_SLIDING          = BC_PREFIX + "sliding";
        public static string BC_ENEMY_DESTROYED  = BC_PREFIX + "enemy_destroyed";                           // +
        public static string BC_POWERUP_APPEARED = BC_PREFIX + "powerup_appeared";                          // +
        public static string BC_POWERUP_OBTAINED = BC_PREFIX + "powerup_obtained";                          // +
        public static string BC_POWERUP_1UP      = BC_PREFIX + "powerup_1up";                               // +
        public static string BC_PLAYER_TANK_DESTROYED = BC_PREFIX + "player_tank_destroyed";                // +
        public static string BC_SCORING_TANK  = BC_PREFIX + "scoring_tank";                                 // +
        public static string BC_SCORING       = BC_PREFIX + "scoring";
        public static string BC_SCORING_BONUS = BC_PREFIX + "scoring_bonus";
        public static string BC_GAMEOVER      = BC_PREFIX + "gameover";                                     

        public static string BC_PLAYER_IDLE = BC_PREFIX + "player_tank_idle";
        public static string BC_PLAYER_WALK = BC_PREFIX + "player_tank_moving";


        public static void Play(string path)
        {
            Audio.Play(path);
        }


        public static void PlayDebugSound01()
        {
            Audio.Play(SFX.char_bad_appear);
        }
    }
}
