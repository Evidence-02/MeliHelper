using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class CutsceneRoomTeleport : CutsceneEntity
    {
        Player.IntroTypes intro_type;
        Player player;
        Vector2 spawnpoint;
        string next_level, dialogue_before;
        float alpha_black;

        public CutsceneRoomTeleport(string next_level, Vector2 spawnpoint, Player.IntroTypes intro_type = Player.IntroTypes.None, 
            string dialogue_before = "")
        {
            this.next_level = next_level;
            this.spawnpoint = spawnpoint;
            this.intro_type = intro_type;
            this.dialogue_before = dialogue_before;
            Tag = Tags.HUD;
        }

        public override void OnBegin(Level level)
        {
            player = level.Tracker.GetEntity<Player>();
            if (Methods.PlayerIsAlive(player))
                Methods.PlayerLock(player);
            
            Add(new Coroutine(Cutscene(level)));
        }

        private IEnumerator Cutscene(Level level)
        {
            // 
            while (!player.OnGround())
                yield return null;
            player.StateMachine.State = Player.StDummy;

            // Dialogue
            if (dialogue_before != null)
                yield return Textbox.Say(dialogue_before);



            // Sleep animation
            player.Facing = Facings.Right;
            yield return 1.0f;
            player.DummyAutoAnimate = false;
            player.Sprite.Play("sleep");

            Audio.Play("event:/char/madeline/campfire_sit", player.Position);
            

            // Screen went dark
            yield return 0.4f;
            while (alpha_black < 1)
            {
                alpha_black += 3 * Engine.DeltaTime;
                yield return null;
            }
            yield return 0.8f;
            
            // cutscene end
            EndCutscene(level);
        }

        public override void Render()
        {
            base.Render();
            if (alpha_black > 0)
                Draw.Rect(new Rectangle(-2, -2, 1924, 1084), Color.Black * alpha_black);
        }

        public override void OnEnd(Level level)
        {
            //Methods.PlayerLock(player, false);
            level.OnEndOfFrame += (Action)(() => {
                level.Remove(player);
                level.UnloadLevel();
                level.Session.Dreaming = false;
                level.Session.Level = next_level;

                //Leader.RestoreStrawberries(player.Leader);

                //There's only 1 spawnpoint on every level anyway
                //level.Session.RespawnPoint = level.GetSpawnPoint(new Vector2(level.Bounds.Left, level.Bounds.Top));
                level.Session.RespawnPoint = level.GetSpawnPoint(spawnpoint);

                // Unlock player moves
                player.StateMachine.Locked = false;
                player.StateMachine.State = 0;
                player.ForceCameraUpdate = false;

                level.LoadLevel(intro_type);
                //Leader.RestoreStrawberries(level.Tracker.GetEntity<Player>().Leader);
            });
        }
    }
}
