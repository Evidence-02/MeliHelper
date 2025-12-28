using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    [CustomEntity("MeliHelper/DebugTeleportTrigger")]
    class DebugTeleportTrigger : Trigger
    {
        RoomTeleportInfo teleport_info;

        public DebugTeleportTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            teleport_info = new RoomTeleportInfo(data, "room");
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            player.SceneAs<Level>().Add(new CutsceneRoomTeleport(teleport_info));
            RemoveSelf();
        }

        public override void Render()
        {
            base.Render();
            ActiveFont.Draw(teleport_info.GetRoomName,
                position: new Vector2(Center.X, Position.Y - 20),
                justify: new Vector2(0.5f),
                scale: new Vector2(0.2f),
                color: Color.White
                );
        }
    }
}
