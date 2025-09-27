using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    class FieldDebugPlayerTracker : Entity
    {
        Field field;
        Player player;
        Rectangle[] mass_rect_debug;

        public FieldDebugPlayerTracker(Field field)
        {
            this.field = field;
            Depth = DepthController.DEFAULT_UI;
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            Level level = scene as Level;
            player = level.Tracker.GetEntity<Player>();
            mass_rect_debug = new Rectangle[4];
        }

        public override void Update()
        {
            base.Update();
            if (Methods.PlayerIsAlive(player))
            {
                bool is_can_destroy_steel = ProgressController.isPlayerCanDestroySteel();
                mass_rect_debug[0] = field.GetWallCollisionsToRender(player.Center, new Vector2(+1, 0), is_can_destroy_steel);
                mass_rect_debug[1] = field.GetWallCollisionsToRender(player.Center, new Vector2(-1, 0), is_can_destroy_steel);
                mass_rect_debug[2] = field.GetWallCollisionsToRender(player.Center, new Vector2(0, +1), is_can_destroy_steel);
                mass_rect_debug[3] = field.GetWallCollisionsToRender(player.Center, new Vector2(0, -1), is_can_destroy_steel);
            }
        }

        public override void Render()
        {
            base.Render();
            if (Methods.PlayerIsAlive(player))
            {
                Draw.Rect(mass_rect_debug[0], Color.Red * 0.8f);
                Draw.Rect(mass_rect_debug[1], Color.Blue * 0.8f);
                Draw.Rect(mass_rect_debug[2], Color.Green * 0.8f);
                Draw.Rect(mass_rect_debug[3], Color.Yellow * 0.8f);
            }
        }
    }
}
