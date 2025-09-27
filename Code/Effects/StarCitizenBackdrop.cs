using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class StarCitizenBackdrop : Backdrop
    {
        const int MAX_WIDTH  = 320; // I always forget!
        const int MAX_HEIGHT = 240;

        List<StarCitizenStar> list_stars;
        List<StarCitizenBlackHole> list_holes;

        public StarCitizenBackdrop(MapData map, BinaryPacker.Element data, BinaryPacker.Element super)
        {
            int count_points = data.AttrInt("stars");
            float points_width_min = data.AttrFloat("widthMin");
            float points_width_max = data.AttrFloat("widthMax");
            if (points_width_max < points_width_min) points_width_max = points_width_min;

            MTexture[] mass_textures = {
                 GFX.Game["Evidence02/objects_melihelper/backdrop/starBlueBig"],
                 GFX.Game["Evidence02/objects_melihelper/backdrop/starRedBig"],
                 GFX.Game["Evidence02/objects_melihelper/backdrop/starYellow"],
            };

            list_stars = new List<StarCitizenStar>();
            for (int i = 0; i < count_points; i++)
            {
                MTexture texture = mass_textures[Calc.Random.Next(0, mass_textures.Length)];
                list_stars.Add(new StarCitizenStar(
                    texture: texture,
                    location: new Vector2(Calc.Random.Next(10, MAX_WIDTH - 10), Calc.Random.Next(10, MAX_HEIGHT - 10)),
                    speed: Calc.AngleToVector(Calc.Random.NextAngle(), 40f),
                    scale: (points_width_min + Calc.Random.NextFloat(points_width_max - points_width_min)) / texture.Width
                    ));
            }


            list_holes = new List<StarCitizenBlackHole>();
            for (int i = 0; i < 2; i++)
            {
                list_holes.Add(new StarCitizenBlackHole(
                    center: new Vector2(MAX_WIDTH / 2, MAX_HEIGHT / 2), 
                    speed: Calc.AngleToVector(Calc.Random.NextAngle(), Calc.Random.Next(40, 80)),
                    radius: 80, 
                    power: 6.2f
                    ));
            }
        }

        public override void Update(Scene scene)
        {
            base.Update(scene);
            foreach (var item in list_holes)
                item.Update(list_stars);
            foreach (var item in list_stars)
                item.Update();
        }

        public override void Render(Scene scene)
        {
            base.Render(scene);
            foreach (var item in list_stars)
                item.Render();
        }

        
        class StarCitizenStar
        {
            MTexture texture;
            Vector2 location, speed;
            float angle, angle_del, scale;
            
            public StarCitizenStar(MTexture texture, Vector2 location, Vector2 speed, float scale = 1)
            {
                this.texture = texture;
                this.location = location;
                this.speed = Vector2.Zero;
                this.scale = scale;
                this.angle_del = 1f * Calc.Random.NextFloat(5f);
            }

            public void Update()
            {
                angle += angle_del * Engine.DeltaTime;
                location += speed * Engine.DeltaTime;
                speed.X *= 0.995f;
                speed.Y *= 0.98f;
                if (Math.Abs(speed.X) >= 10) speed.X *= 0.9f;
                if (Math.Abs(speed.Y) >= 10) speed.Y *= 0.9f;
                speed.Y += 0.4f;


                if (location.X < 0)
                {
                    location.X = 0;
                    if (speed.X < 0) speed.X *= -1;
                }
                if (location.X > MAX_WIDTH)
                {
                    location.X = MAX_WIDTH;
                    if (speed.X > 0) speed.X *= -1;
                }

                if (location.Y > MAX_HEIGHT)
                {
                    location.Y = MAX_HEIGHT;
                    if (speed.Y > 0) speed.Y *= -1;
                }

                /*
                if (location.Y < 0)
                {
                    location.Y = 0;
                    if (speed.Y < 0) speed.Y *= -1;
                }
                if (location.Y > MAX_HEIGHT)
                {
                    location.Y = MAX_HEIGHT;
                    if (speed.Y > 0) speed.Y *= -1;
                }
                */
            }

            public void Render()
            {
                texture.DrawCentered(location, Color.White, scale, angle);
            }

            public Vector2 Loc
            {
                get
                {
                    return location;
                }
                set
                {
                    location = value;
                }
            }

            public Vector2 GetSpeed
            {
                get
                {
                    return speed;
                }
                set
                {
                    speed = value;
                }
            }

            public float GetAngle
            {
                get
                {
                    return angle;
                }
                set
                {
                    angle = value;
                }
            }
        }

        class StarCitizenBlackHole
        {
            Vector2 center, speed;
            float radius, power;
            
            public StarCitizenBlackHole(Vector2 center, Vector2 speed, float radius, float power)
            {
                this.center = center;
                this.speed = speed;
                this.radius = radius;
                this.power = power;
            }

            public void Update(List<StarCitizenStar> list_stars)
            {
                center += speed * Engine.DeltaTime;
                if (center.X < -radius            ) center.X += MAX_WIDTH + 2 * radius;
                if (center.X > MAX_WIDTH + radius ) center.X -= MAX_WIDTH + 2 * radius;
                if (center.Y < -radius            ) center.Y -= MAX_HEIGHT + 2 * radius;
                if (center.Y > MAX_HEIGHT + radius) center.Y -= MAX_HEIGHT + 2 * radius;

                foreach (StarCitizenStar star in list_stars)
                {
                    Vector2 to_star = star.Loc - center;
                    if (to_star.Length() <= radius && to_star.Length() > 1)
                    {
                        star.GetSpeed *= 1.1f;
                        star.GetSpeed += power * Vector2.Normalize(to_star);
                        star.GetAngle += (float)Math.Atan2(to_star.Y, to_star.X) / MathExt.DegreesToRadians;
                    }
                }

            }
        }
    }
}
