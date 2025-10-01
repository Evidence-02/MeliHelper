using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.PlayBaddy
{
    [CustomEntity("MeliHelper/StrawberryCollectTrigger")]
    class StrawberryCollectTrigger : Trigger
    {
        Level level;
        bool is_only_on_ground;
        bool is_golden;

        public StrawberryCollectTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            is_only_on_ground = data.Bool("onlyOnGround", true);
            is_golden = data.Bool("golden", false);
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = scene as Level;
        }

        public override void Update()
        {
            base.Update();

            //Strawberry berry = level.Entities.FindAll<Strawberry>().FirstOrDefault(t => CollidePoint(t.Center));
            //if (berry != null)
            //{
            //    Logger.Log("StrawberryCollectTrigger", $"berry.Follower.Leader={berry.Follower.Leader}");
            //    Methods.DebugLogHiddenFields(typeof(Strawberry), berry);

            //    //berry.OnCollect();
            //    RemoveSelf();
            //}

            Player player = level.Tracker.GetEntity<Player>();
            if (player != null && (player.OnGround() || !is_only_on_ground))
            {
                Strawberry berry = level.Entities.FindAll<Strawberry>().FirstOrDefault(t => CollidePoint(t.Center)
                    && player.Leader.Followers.Contains(t.Follower) 
                    && t.Golden == is_golden);
                if (berry != null)
                {
                    berry.OnCollect();
                    RemoveSelf();
                }
            }
        }
    }
}
