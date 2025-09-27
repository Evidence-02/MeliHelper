using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    class Dynamite : Actor
    {
        Holdable hold;
        Vector2 speed;

        public Dynamite(Vector2 position) : base(position)
        {
            Add(hold = new Holdable(0.1f));
            hold.PickupCollider = new Hitbox(18f, 22f, -9f, -16f);
            hold.SlowRun = true;
            hold.OnPickup = OnPickup;
            hold.OnRelease = OnRelease;
            hold.SpeedGetter = () => speed;
        }

        public override void Update()
        {
            base.Update();

            speed.Y = Calc.Approach(speed.Y, 200f, Engine.DeltaTime);
            MoveH(speed.X * Engine.DeltaTime, onCollideH);
            MoveV(speed.Y * Engine.DeltaTime, onCollideV);
        }

        void onCollideH(CollisionData data)
        {
            //Audio.Play("event:/game/05_mirror_temple/crystaltheo_hit_side", Position);

            speed.X *= -0.4f;
        }

        void onCollideV(CollisionData data)
        {
            //Audio.Play("event:/game/05_mirror_temple/crystaltheo_hit_side", Position);

            speed.X *= 0.15f;
            if (speed.Y > 0) speed.Y *= -0.3f;
            else speed.Y = 0;

        }



        void OnPickup()
        {
            AddTag(Tags.Persistent);
            AllowPushing = false;
        }

        void OnRelease(Vector2 force)
        {
            RemoveTag(Tags.Persistent);
            AllowPushing = true;
            speed = new Vector2(90 * Math.Sign(force.X), -60);
        }

    }
}
