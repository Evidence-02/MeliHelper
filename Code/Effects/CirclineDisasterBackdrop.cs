using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class CirclineDisasterBackdrop : Backdrop
    {
        const int MAX_WIDTH  = 320; // I always forget!
        const int MAX_HEIGHT = 240;

        List<CirclineBackdropPoint> list_points;
        List<CirclineBackdropLink> list_links;
        MTexture texture;

        public CirclineDisasterBackdrop(MapData map, BinaryPacker.Element data, BinaryPacker.Element super)
        {
            int count_points = data.AttrInt("points");
            int count_links = data.AttrInt("links");
            float points_width_min = data.AttrFloat("widthMin");
            float points_width_max = data.AttrFloat("widthMax");
            float speed_points_start = data.AttrFloat("startSpeed", 24);
            float speed_points_max   = data.AttrFloat("maxSpeed", 50);
            float link_dist_max = data.AttrFloat("linksMaxDistance", 100);
            int max_links_to_the_point = data.AttrInt("maxLinksOnPoint", 4);
            Color line_color = Methods.GetColorFromString(data.Attr("lineColor", "FFFFFF"));
            int line_thick = data.AttrInt("lineThickness", 2);
            
            texture = GFX.Game[data.Attr("texture", "Evidence02/objects_melihelper/bubsybackdrop/textureWhite")];
            if (points_width_max < points_width_min) points_width_max = points_width_min;

            list_points = new List<CirclineBackdropPoint>();
            for (int i = 0; i < count_points; i++)
            {
                list_points.Add(new CirclineBackdropPoint(
                    location: new Vector2(Calc.Random.Next(10, MAX_WIDTH - 10), Calc.Random.Next(10, MAX_HEIGHT - 10)),
                    speed: new Vector2(Calc.Random.NextFloat(2 * speed_points_start) - speed_points_start, Calc.Random.NextFloat(2 * speed_points_start) - speed_points_start),
                    color: line_color,
                    scale: (points_width_min + Calc.Random.NextFloat(points_width_max - points_width_min)) / texture.Width,
                    max_speed: speed_points_max
                    ));
            }

            
            List<string> list_possible_links = new List<string>();
            for (int i = 0; i < count_points; i++)
                for (int j = i + 1; j < count_points; j++)
                    list_possible_links.Add(i + "-" + j);

            list_links = new List<CirclineBackdropLink>();
            for (int i = 0; i < count_links; i++)
            {
                string id = list_possible_links[Calc.Random.Next(0, list_possible_links.Count)];
                string id1 = id.Before("-");
                string id2 = id.After ("-");
                CirclineBackdropPoint p1 = list_points[int.Parse(id1)];
                CirclineBackdropPoint p2 = list_points[int.Parse(id2)];
                list_links.Add(new CirclineBackdropLink(p1, p2, link_dist_max, line_color, line_thick));

                list_possible_links.Remove(id);
                if (++p1.GetCountLinks >= max_links_to_the_point)
                    list_possible_links.RemoveAll(t => t.StartsWith(id1 + "-"));
                if (++p2.GetCountLinks >= max_links_to_the_point)
                    list_possible_links.RemoveAll(t => t.EndsWith("-" + id2));
                if (list_possible_links.Count == 0)
                    break;
            }
        }

        public override void Update(Scene scene)
        {
            base.Update(scene);
            foreach (var item in list_links)
                item.Update();
            foreach (var item in list_points)
                item.Update();
        }

        public override void Render(Scene scene)
        {
            base.Render(scene);
            foreach (var item in list_links)
                item.Render();
            foreach (var item in list_points)
                item.Render(texture);
        }

        
        class CirclineBackdropPoint
        {
            Vector2 location, speed;
            float scale, max_speed;
            int count_links;
            Color color;

            public CirclineBackdropPoint(Vector2 location, Vector2 speed, Color color, float scale = 1, float max_speed = 50)
            {
                this.location = location;
                this.speed = speed;
                this.max_speed = max_speed;
                this.scale = scale;
                this.color = color;
            }

            public void Update()
            {
                location += speed * Engine.DeltaTime;
                if (Math.Abs(speed.X) > max_speed) speed.X *= 0.96f;
                if (Math.Abs(speed.Y) > max_speed) speed.Y *= 0.96f;

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
            }

            public void Render(MTexture texture)
            {
                texture.DrawCentered(location, color * 0.3f, scale);
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

            public int GetCountLinks
            {
                get
                {
                    return count_links;
                }
                set
                {
                    count_links = value;
                }
            }

            public float GetRadius
            {
                get
                {
                    return scale * 28.4f;
                }
            }
        }

        class CirclineBackdropLink
        {
            CirclineBackdropPoint p1, p2;
            Color color;
            float link_dist_max;
            int thickness;

            public CirclineBackdropLink(CirclineBackdropPoint p1, CirclineBackdropPoint p2, float link_dist_max, Color color, int thickness = 1)
            {
                this.p1 = p1;
                this.p2 = p2;
                this.link_dist_max = link_dist_max;
                this.color = color;
                this.thickness = thickness;
            }

            public void Update()
            {
                float dx = p2.Loc.X - p1.Loc.X;
                float dy = p2.Loc.Y - p1.Loc.Y;
                if (dx * dx + dy * dy > link_dist_max * link_dist_max)
                {
                    float dd = (float)Math.Sqrt(dx * dx + dy * dy);
                    float koef = 0.01f * (dd - link_dist_max) / dd;
                    p1.GetSpeed += koef * new Vector2(dx, dy);
                    p2.GetSpeed -= koef * new Vector2(dx, dy);
                }
            }

            public void Render()
            {
                Vector2 del = Vector2.Normalize(p2.Loc - p1.Loc);
                Draw.Line(p1.Loc + p1.GetRadius * del, p2.Loc - p2.GetRadius * del, color * 0.3f, thickness);
            }
        }
    }
}
