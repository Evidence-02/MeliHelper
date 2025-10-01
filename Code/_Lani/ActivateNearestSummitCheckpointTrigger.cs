using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;

namespace Celeste.Mod.PlayBaddy
{
    [CustomEntity("MeliHelper/ActivateNearestSummitCheckpointTrigger")]
    class ActivateNearestSummitCheckpointTrigger : Trigger
    {
        public ActivateNearestSummitCheckpointTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);

            List<SummitCheckpoint> list_checkpoints = player.SceneAs<Level>().Entities.FindAll<SummitCheckpoint>();
            if (list_checkpoints.Count > 0)
            {
                SummitCheckpoint checkpoint = list_checkpoints.OrderBy(t => Vector2.Distance(t.Center, this.Center)).First();
                checkpoint.Activated = true;
            }
        }
    }
}
