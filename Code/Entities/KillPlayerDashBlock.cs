using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Celeste.SummitCheckpoint;

namespace Celeste.Mod.MeliHelper
{
    [CustomEntity("MeliHelper/KillPlayerDashBlock")]
    class KillPlayerDashBlock : DashBlock
    {
        public KillPlayerDashBlock(EntityData data, Vector2 offset, EntityID id) : base(data, offset, id)
        {
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            KillPlayerDashBlock.Load();
        }



        #region Load

        static bool is_loaded;

        static void Load()
        {
            if (!is_loaded)
            {
                is_loaded = true;
                On.Celeste.DashBlock.Break_Vector2_Vector2_bool += onDashBlockBreak;
                On.Celeste.DashBlock.Break_Vector2_Vector2_bool_bool += onDashBlockBreak;
            }
        }

        static void onDashBlockBreak(On.Celeste.DashBlock.orig_Break_Vector2_Vector2_bool orig, DashBlock self, 
            Vector2 from, Vector2 direction, bool playSound)
        {
            orig(self, from, direction, playSound);
            if (self is KillPlayerDashBlock)
                TryKillPlayer(self);
        }

        static void onDashBlockBreak(On.Celeste.DashBlock.orig_Break_Vector2_Vector2_bool_bool orig, DashBlock self,
            Vector2 from, Vector2 direction, bool playSound, bool playDebrisSound)
        {
            orig(self, from, direction, playSound, playDebrisSound);
            if (self is KillPlayerDashBlock)
                TryKillPlayer(self);
        }

        static void TryKillPlayer(DashBlock self)
        {
            Level level = self.Scene as Level;
            Player player = level.Tracker.GetEntity<Player>();
            if (Methods.PlayerIsAlive(player))
                player.Die(Vector2.Normalize(player.Center - self.Center));
        }


        #endregion

    }
}
