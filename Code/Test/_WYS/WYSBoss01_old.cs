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
    [CustomEntity("MelHelper/WYSBoss01")]
    class WYSBoss01_old : Entity
    {
        Level level;
        Player player;
        PlayerChaseTracker player_chaser;
        Vector2 player_chase_spot;
        Sprite sprite;
        CustomTimer timer_wait, timer_shot;
        Color color_bullets;
        string state;
        int count_bullets, count_bullets_max;
        float speed_bullets, delay_follow;
        
        public WYSBoss01_old(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            Add(sprite = GFX.SpriteBank.Create("MeliHelper/WYSBoss01"));
            count_bullets_max = data.Int("bullets");
            count_bullets = count_bullets_max;
            speed_bullets = data.Float("bulletSpeed", 120);
            color_bullets = Methods.GetColorFromString(data.Attr("bulletColor", "0000FF"));
            delay_follow = data.Float("followDelay");

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
            player_chase_spot = player_chaser.GetNextPlayerCenter();

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
                        // Shot
                        float angle = Calc.Angle(this.Center, player_chase_spot); 
                        Vector2 speed = Calc.AngleToVector(angle, 1f);
                        bool is_horiz = Math.Abs(speed.X) >= Math.Abs(speed.Y);

                        level.Add(new Bullet(
                            position: this.Center + 8 * speed,
                            speed:    this.speed_bullets * speed,
                            color:    color_bullets,
                            is_horiz: is_horiz
                            ));

                        level.Add(new Bullet(
                            position: this.Center - 8 * speed,
                            speed: -this.speed_bullets * speed,
                            color: color_bullets,
                            is_horiz: is_horiz
                            ));

                        if (--count_bullets <= 0)
                            state = "00:Waiting";
                    }
                    break;
            }
        }
    }
}
