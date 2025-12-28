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
    [CustomEntity("MeliHelper/WYSTurretRotate")]
    class TurretRotate : Entity
    {
        Level level;
        Player player;
        Sprite sprite;
        Dictionary<DirectionEnum, bool> directions_available;
        Vector2 destination;
        DirectionEnum dir_current;
        CustomTimer timer_wait, timer_move, timer_shot;
        Color color_bullets;
        string state;
        int count_bullets, count_bullets_max;
        float player_distance, speed_bullets, bullet_scatter;
        bool is_moving_clockwise;

        public TurretRotate(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            Add(sprite = GFX.SpriteBank.Create("MeliHelper_WYS_Turret"));
            player_distance = data.Float("playerDistance", 32);
            dir_current = (DirectionEnum)Enum.Parse(typeof(DirectionEnum), data.Attr("startDirection", "Up"));
            count_bullets_max = data.Int("bullets");
            count_bullets = count_bullets_max;
            speed_bullets = data.Float("bulletSpeed", 120);
            bullet_scatter = data.Float("bulletScatter", 0);
            color_bullets = Methods.GetColorFromString(data.Attr("bulletColor", "0000FF"));
            is_moving_clockwise = data.Bool("moveClockwise", false);
            state = "00:Waiting";

            directions_available = new Dictionary<DirectionEnum, bool>();
            directions_available[DirectionEnum.Left ] = data.Bool("left");
            directions_available[DirectionEnum.Right] = data.Bool("right");
            directions_available[DirectionEnum.Up   ] = data.Bool("up");
            directions_available[DirectionEnum.Down ] = data.Bool("down");

            timer_wait = new CustomTimer(data.Float("delayShoot",  1.2f));
            timer_move = new CustomTimer(data.Float("moveTime",    0.2f));
            timer_shot = new CustomTimer(data.Float("periodShoot", 0.12f));
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            level = scene as Level;
            player = level.Tracker.GetEntity<Player>();
            if (directions_available.Count(t => t.Value) == 0)
                RemoveSelf();
        }

        public override void Update()
        {
            base.Update();
            switch (state)
            {
                case "00:Waiting":
                    if (timer_wait.Tick())
                    {
                        state = "01:Moving";
                        ChooseNextDirection();
                        sprite.Rotation = Methods.DirectionToVector(dir_current).Angle();
                        destination = player.Center + player_distance * Methods.DirectionToVector(dir_current);
                    }
                    break;

                case "01:Moving":
                    Center += 0.2f * (destination - Center);
                    if (timer_move.Tick())
                    {
                        state = "02:Shoot";
                        count_bullets = count_bullets_max;
                        Center = destination;
                    }
                    break;

                case "02:Shoot":
                    if (timer_shot.Tick())
                    {
                        //Audio.Play();

                        // Shot
                        float angle = Methods.DirectionToVector(dir_current).Angle() 
                            - bullet_scatter + Calc.Random.NextFloat(2f * bullet_scatter);
                        Vector2 speed = Calc.AngleToVector(angle, 1f);
                        level.Add(new Bullet(
                            position: this.Position + 6 * speed,
                            speed:    this.speed_bullets * speed,
                            color:    color_bullets,
                            is_horiz: dir_current == DirectionEnum.Left || dir_current == DirectionEnum.Right
                            ));

                        if (--count_bullets <= 0)
                            state = "00:Waiting";
                    }
                    break;
            }
        }

        void ChooseNextDirection()
        {
            do
            {
                switch (dir_current)
                {
                    case DirectionEnum.Up:    dir_current = is_moving_clockwise ? DirectionEnum.Right : DirectionEnum.Left;  break;
                    case DirectionEnum.Right: dir_current = is_moving_clockwise ? DirectionEnum.Down  : DirectionEnum.Up;    break;
                    case DirectionEnum.Down:  dir_current = is_moving_clockwise ? DirectionEnum.Left  : DirectionEnum.Right; break;
                    case DirectionEnum.Left:  dir_current = is_moving_clockwise ? DirectionEnum.Up    : DirectionEnum.Down;  break;
                }
            }
            while (!directions_available[dir_current]);
        }

    }
}
