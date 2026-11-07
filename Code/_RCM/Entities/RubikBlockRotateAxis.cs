using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;

namespace Celeste.Mod.MeliHelper
{
    [CustomEntity("MeliHelper/RubikBlockRotateAxis")]
    class RubikBlockRotateAxis : Solid
    {
        RCMEnum_AxisType edge;
        string rotate_direction;

        Sprite sprite;
        string animation_play;

        public RubikBlockRotateAxis(EntityData data, Vector2 offset) : base(data.Position + offset, data.Width, data.Height, true)
        {
            Add(sprite = GFX.SpriteBank.Create(data.Attr("sprite")));
            sprite.Position += new Vector2(sprite.Width / 2, sprite.Height / 2);
            animation_play = data.Attr("animationOnDash", "");
            rotate_direction = (data.Bool("clockwise", true) ? "Clockwise" : "Otherwise");

            string axis = data.Attr("axis", "X");
            switch (axis)
            {
                case "X": edge = RCMEnum_AxisType.AxisX; break;
                case "Y": edge = RCMEnum_AxisType.AxisY; break;
                case "Z": edge = RCMEnum_AxisType.AxisZ; break;
                default: edge = RCMEnum_AxisType.None; break;
            }
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            OnDashCollide = new DashCollision(onDashCollide);
        }

        protected virtual DashCollisionResults onDashCollide(Player player, Vector2 dir)
        {
            Level level = player.SceneAs<Level>();
            foreach (RubikCube cube in level.Entities.FindAll<RubikCube>())
                cube.TryRotateAxis(edge, rotate_direction);
            if (animation_play != "")
                sprite.Play(animation_play);
            return DashCollisionResults.Bounce;
        }
    }
}
