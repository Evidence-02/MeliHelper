using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._WYS
{
    [CustomEntity("MeliHelper/WYSMovingBlock")]
    class MovingBlock : Solid
    {
        Color color_border, color_inside;
        DirectionEnum dir_current;
        string direction;
        float value, value_add_by_dash, value_add_by_shot, alpha_insides;
        bool is_activated, kill_player_on_moving;

        Vector2 dir_speed;
        float speed_current, speed_max;

        public MovingBlock(EntityData data, Vector2 offset) 
            : base(data.Position + offset, data.Width, data.Height, false)
        {
            color_border = Methods.GetColorFromString(data.Attr("color", "FF6600"));
            color_inside = Methods.GetColorFromString(data.Attr("colorInside", "181818"));

            value_add_by_dash = data.Float("incByDash", 0.334f); // 3 dashes to activate
            value_add_by_shot = data.Float("incByShot", 0.143f); // 7 shots to activate

            //value_add_by_dash = 1.001f / MeliHelperModule.Settings.WYS_MovingBlock_DashesToActivate;
            //value_add_by_shot = 1.001f / MeliHelperModule.Settings.WYS_MovingBlock_ShotsToActivate;
            kill_player_on_moving = data.Bool("killPlayerOnMoving");

            direction = data.Attr("direction", "Up");   // Left, Right, Up, Down, Vertical, Horizontal, Any

            speed_max = data.Float("speed", 120);
            alpha_insides = 0.6f;

            OnDashCollide += onDashCollide;
        }

        public override void Update()
        {
            base.Update();
            if (alpha_insides > 0.6f)
                alpha_insides -= Engine.DeltaTime / 0.7f;

            if (is_activated)
            {
                // Increase speed
                if (speed_current < speed_max)
                {
                    //speed_current += 10 * MeliHelperModule.Settings.WYS_MovingBlock_SpeedAdd10 * Engine.DeltaTime;
                    speed_current += 10 * 24 * Engine.DeltaTime;
                    if (speed_current > speed_max)
                        speed_current = speed_max;
                }

                // Move
                if (dir_speed.X != 0) MoveH(dir_speed.X * speed_current * Engine.DeltaTime);
                if (dir_speed.Y != 0) MoveV(dir_speed.Y * speed_current * Engine.DeltaTime);

                // Collide with other blocks
                Solid solid = CollideFirst<Solid>();
                if (solid != null)
                {
                    // TODO: increase drama!
                    if (solid is MovingBlock)
                        solid.RemoveSelf();

                    for (int i = 0; i < 4; i++)
                        SceneAs<Level>().ParticlesBG.Emit(ParticleTypes.Dust, Center, color_border);
                    RemoveSelf();
                }
            }
        }

        public override void Render()
        {
            base.Render();

            Draw.Rect(this.Position, this.Width, this.Height, color_inside);
            Draw.HollowRect(this.Position + new Vector2(1, 1), this.Width - 2, this.Height - 2, color_border);
            if (is_activated)
                Draw.Rect(  this.Position + new Vector2(2, 2), this.Width - 4, this.Height - 4, color_border * 0.8f);

            if (!is_activated && value > 0)
            {
                Color color_based = color_border * alpha_insides;
                float pixels = value * (Height - 4);
                int len = (int)pixels;
                if (len > 0)
                    switch (dir_current)
                    {
                        case DirectionEnum.Up:    Draw.Rect(this.Position + new Vector2(2, 2),                     this.Width - 4,  len, color_based); break;
                        case DirectionEnum.Down:  Draw.Rect(this.Position + new Vector2(2, this.Height - 2 - len), this.Width - 4,  len, color_based); break;
                        case DirectionEnum.Left:  Draw.Rect(this.Position + new Vector2(2, 2),                     len, this.Height - 4, color_based); break;
                        case DirectionEnum.Right: Draw.Rect(this.Position + new Vector2(this.Width - 2 - len,  2), len, this.Height - 4, color_based); break;
                    }

                float alpha = pixels - len;
                if (alpha > 0)
                    switch (dir_current)
                    {
                        case DirectionEnum.Up:    Draw.Rect(this.Position + new Vector2(2, 2 + len),               this.Width - 4,  1, color_based * alpha); break;
                        case DirectionEnum.Down:  Draw.Rect(this.Position + new Vector2(2, this.Height - 3 - len), this.Width - 4,  1, color_based * alpha); break;
                        case DirectionEnum.Left:  Draw.Rect(this.Position + new Vector2(2 + len,               2), 1, this.Height - 4, color_based * alpha); break;
                        case DirectionEnum.Right: Draw.Rect(this.Position + new Vector2(this.Width - 2 - len,  2), 1, this.Height - 4, color_based * alpha); break;
                    }
            }

        }



        protected virtual DashCollisionResults onDashCollide(Player player, Vector2 dir)
        {
            if (!is_activated)
            {
                AddValue(value_add_by_dash, player.Center, player.Speed);
                return DashCollisionResults.Bounce;
            }

            return DashCollisionResults.NormalCollision;
        }
        
        public void AddValue(float add, Vector2 obj_center, Vector2 obj_speed, string obj_direction = "")
        {
            // Calculate new direction
            DirectionEnum dir_new;
            switch (direction)
            {
                case "Left":  dir_new = DirectionEnum.Left;  break;
                case "Right": dir_new = DirectionEnum.Right; break;
                case "Up":    dir_new = DirectionEnum.Up;    break;
                case "Down":  dir_new = DirectionEnum.Down;  break;

                case "Horizontal":
                case "Vertical":
                case "Any":
                    string block_direction = direction;
                    if (block_direction == "Any")
                        block_direction = (obj_direction != "") ? obj_direction : 
                                          (obj_speed.Y == 0) ? "Horizontal" : "Vertical";
                    switch (block_direction)
                    {
                        case "Horizontal": dir_new = (obj_speed.X > 0) ? DirectionEnum.Left : DirectionEnum.Right; break;
                        case "Vertical":   dir_new = (obj_speed.Y > 0) ? DirectionEnum.Up   : DirectionEnum.Down;  break;
                        default: throw new Exception("Unknown block_direction type: " + direction);
                    }
                    break;
                    
                default: throw new Exception("Unknown direction type: " + direction);
            }

            // Clear value if direction is new
            if (dir_new != dir_current)
            {
                dir_current = dir_new;
                value = 0;
            }
            alpha_insides += 0.3f;
            if (alpha_insides > 1)
                alpha_insides = 1;

            value += add;
            if (value >= 1)
                Activate();
        }

        void Activate()
        {
            if (is_activated)
                return;

            value = 1;
            is_activated = true;
            dir_speed = Methods.DirectionToVector(dir_current);
            //Audio.Play();
        }

        public float GetValueAddByShot
        {
            get
            {
                return value_add_by_shot;
            }
        }




    }
}
