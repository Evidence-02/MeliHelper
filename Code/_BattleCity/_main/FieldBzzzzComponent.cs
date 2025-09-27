using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    class FieldBzzzzComponent : Component
    {
        Player player;
        Vector2 prev_position;
        CustomTimer timer;

        float timer__debug;

        public FieldBzzzzComponent() : base(true, false)
        {
            timer = new CustomTimer(0.12f);
        }

        public override void Added(Entity entity)
        {
            base.Added(entity);
            player = entity.SceneAs<Level>().Tracker.GetEntity<Player>();
            prev_position = player.Position;
        }

        public override void Update()
        {
            base.Update();

            //if (timer.Tick())
            timer__debug += Engine.DeltaTime;
            if (timer__debug >= 0.12f)
            {
                timer__debug = 0;
                if (Vector2.Distance(player.Position, prev_position) <= 8)
                    Audio.Play(SoundController.BC_PLAYER_IDLE);
                else
                    Audio.Play(SoundController.BC_PLAYER_WALK);
                prev_position = player.Position;
            }
        }



    }
}
