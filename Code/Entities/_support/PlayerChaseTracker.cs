using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class PlayerChaseTracker
    {
        Player player;
        List<PlayerLocationSnapshot> list_snapshots;
        float current_time, delay, frequency;

        public PlayerChaseTracker(Player player, float delay, float frequency = 0.02f)
        {
            list_snapshots = new List<PlayerLocationSnapshot>();
            this.player = player;
            this.delay = delay;
            this.frequency = frequency;
            current_time = 0;
            list_snapshots.Add(new PlayerLocationSnapshot() { center = player.Center, time = current_time });
        }

        public Vector2 GetNextPlayerCenter()
        {
            current_time += Engine.DeltaTime;
            if (frequency == 0 || player.Scene.OnInterval(frequency))
            {
                list_snapshots.Add(new PlayerLocationSnapshot() { center = player.Center, time = current_time });
                if (current_time >= list_snapshots[0].time + delay)
                    list_snapshots.RemoveAt(0);
            }
            return list_snapshots[0].center;
        } 


        struct PlayerLocationSnapshot
        {
            public Vector2 center;
            public float time;
        }
    }
}
