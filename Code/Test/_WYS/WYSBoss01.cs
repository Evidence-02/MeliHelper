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
    [CustomEntity("MeliHelper/WYSBoss01")]
    class WYSBoss01 : Entity
    {
        Level level;
        Player player;
        PlayerChaseTracker player_chaser;
        Sprite sprite;
        CustomTimer timer_shot;
        Color color_bullets;
        float speed_bullets, delay_follow, shot_angle;
        
        public WYSBoss01(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            Add(sprite = GFX.SpriteBank.Create("MeliHelper/WYSBoss01"));
            speed_bullets = data.Float("bulletSpeed", 120);
            color_bullets = Methods.GetColorFromString(data.Attr("bulletColor", "0000FF"));
            delay_follow = data.Float("followDelay, 0.5f");
            timer_shot = new CustomTimer(data.Float("periodShoot", 0.12f));
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            level = scene as Level;
            player = level.Tracker.GetEntity<Player>();
            player_chaser = new PlayerChaseTracker(player, delay_follow);
            shot_angle = Calc.Angle(this.Center, player.Center);
        }

        public override void Update()
        {
            base.Update();
            shot_angle = Calc.Angle(this.Center, player_chaser.GetNextPlayerCenter());
            sprite.Rotation = shot_angle;

            if (timer_shot.Tick())
            {
                // Double shot
                Vector2 speed = Calc.AngleToVector(shot_angle, 1f);
                bool is_horiz = Math.Abs(speed.X) >= Math.Abs(speed.Y);
                level.Add(new Bullet(
                    position: this.Center + 8 * speed,
                    speed: this.speed_bullets * speed,
                    color: color_bullets,
                    is_horiz: is_horiz
                    ));

                level.Add(new Bullet(
                    position: this.Center - 8 * speed,
                    speed: -this.speed_bullets * speed,
                    color: color_bullets,
                    is_horiz: is_horiz
                    ));
            }
        }
    }
}
