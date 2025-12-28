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
    [CustomEntity("MeliHelper/WYSTurretChase")]
    class TurretChase : Entity
    {
        Level level;
        Player player;
        PlayerChaseTracker player_chaser;

        Sprite sprite;
        DirectionEnum dir;
        Vector2 dir_vector;
        CustomTimer timer_wait, timer_shot;
        Color color_bullets;
        string state;
        int count_bullets, count_bullets_max;
        float player_distance, speed_bullets, bullet_scatter, delay_follow;
        
        public TurretChase(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            Add(sprite = GFX.SpriteBank.Create("MeliHelper_WYS_Turret"));
            player_distance = data.Float("playerDistance", 32);
            dir = (DirectionEnum)Enum.Parse(typeof(DirectionEnum), data.Attr("direction", "Up"));
            dir_vector = Methods.DirectionToVector(dir);
            count_bullets_max = data.Int("bullets");
            count_bullets = count_bullets_max;
            speed_bullets = data.Float("bulletSpeed", 120);
            bullet_scatter = data.Float("bulletScatter", 0);
            color_bullets = Methods.GetColorFromString(data.Attr("bulletColor", "0000FF"));
            sprite.Rotation = dir_vector.Angle();
            delay_follow = data.Float("followDelay", 0.5f);
            state = "00:Waiting";

            timer_wait = new CustomTimer(data.Float("delayShoot",  1.2f));
            timer_shot = new CustomTimer(data.Float("periodShoot", 0.12f));
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            level = scene as Level;
            player = level.Tracker.GetEntity<Player>();
            player_chaser = new PlayerChaseTracker(player, delay_follow);
        }

        public override void Update()
        {
            base.Update();
            if (Methods.PlayerIsAlive(player))  
                Center = player_chaser.GetNextPlayerCenter() - player_distance * dir_vector;

            switch (state)
            {
                case "00:Waiting":
                    if (timer_wait.Tick())
                    {
                        state = "01:Shoot";
                        count_bullets = count_bullets_max;
                    }
                    break;
                    
                case "01:Shoot":
                    if (timer_shot.Tick())
                    {
                        //Audio.Play();

                        // Shot
                        float angle = dir_vector.Angle() - bullet_scatter + Calc.Random.NextFloat(2f * bullet_scatter);
                        Vector2 speed = Calc.AngleToVector(angle, 1f);
                        level.Add(new Bullet(
                            position: this.Position + 6 * speed,
                            speed:    this.speed_bullets * speed,
                            color:    color_bullets,
                            is_horiz: dir == DirectionEnum.Left || dir == DirectionEnum.Right
                            ));

                        if (--count_bullets <= 0)
                            state = "00:Waiting";
                    }
                    break;
            }
        }

        //public override void Render()
        //{
        //    base.Render();
        //    ActiveFont.Draw(delay_follow.ToString("0.00"), 
        //        position: this.Position, justify: new Vector2(0.5f), scale: new Vector2(0.15f), color: Color.White);
        //}
    }
}
