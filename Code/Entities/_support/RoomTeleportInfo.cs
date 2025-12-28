using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class RoomTeleportInfo
    {
        string room_name;
        Vector2 spawnpoint;
        Player.IntroTypes intro_type;

        public RoomTeleportInfo(EntityData data, string room_param_name = "roomTeleport")
        {
            room_name = data.Attr(room_param_name);
            spawnpoint = (data.Has("spawnpointX")) ? new Vector2(data.Int("spawnpointX"), data.Int("spawnpointY")) : Vector2.Zero;
            intro_type = (data.Has("introTypes" )) ? (Player.IntroTypes)Enum.Parse(typeof(Player.IntroTypes), data.Attr("introTypes", "WakeUp")) : Player.IntroTypes.None;
        }

        public RoomTeleportInfo(string room_name) : this(room_name, Vector2.Zero, Player.IntroTypes.None) { }

        public RoomTeleportInfo(string room_name, Vector2 spawnpoint, Player.IntroTypes intro_type)
        {
            this.room_name = room_name;
            this.spawnpoint = spawnpoint;
            this.intro_type = intro_type;
        }

        public void OnRoomEnd(Level level, Player player)
        {
            //Methods.PlayerLock(player, false);
            level.OnEndOfFrame += (Action)(() => {
                level.Remove(player);
                level.UnloadLevel();
                level.Session.Dreaming = false;

                CustomLogger.Log("RoomTeleportInfo", "room_name=" + room_name);
                level.Session.Level = room_name;
                
                //Leader.RestoreStrawberries(player.Leader);

                //There's only 1 spawnpoint on every level anyway
                //level.Session.RespawnPoint = level.GetSpawnPoint(new Vector2(level.Bounds.Left, level.Bounds.Top));
                level.Session.RespawnPoint = level.GetSpawnPoint(spawnpoint);

                // Unlock player moves
                Methods.PlayerLock(player, false);

                level.LoadLevel(intro_type);
                //Leader.RestoreStrawberries(level.Tracker.GetEntity<Player>().Leader);
            });
        }

        public string GetRoomName
        {
            get
            {
                return room_name;
            }
        }



    }
}
