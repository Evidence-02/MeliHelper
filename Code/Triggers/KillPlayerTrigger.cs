using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;
using System.Collections;

namespace Celeste.Mod.MeliHelper
{
    [CustomEntity("MeliHelper/KillPlayerTrigger")]
    class KillPlayerTrigger : Trigger
    {
        string action;
        float timer, period;

        public KillPlayerTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            action = data.Attr("action", "OnEnter");
            period = data.Float("stayTime", 0);
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            timer = 0;
            if (action == "OnEnter")
                KillPlayer(player);

        }

        public override void OnStay(Player player)
        {
            base.OnStay(player);
            if (action == "OnStay")
            {
                timer += Engine.DeltaTime;
                if (timer >= period)
                    KillPlayer(player);
            }
        }

        public override void OnLeave(Player player)
        {
            base.OnLeave(player);
            if (action == "OnLeave")
                KillPlayer(player);
        }

        void KillPlayer(Player player)
        {
            if (Methods.PlayerIsAlive(player))
                player.Die(Vector2.Normalize(player.Center - this.Center));
        }
    }
}
