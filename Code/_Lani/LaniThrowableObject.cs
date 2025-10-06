using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._Lani
{
    abstract class LaniThrowableObject : Actor
    {
        protected static Random rand = new Random();
        protected Level level;
        protected Vector2 speed;
        protected bool is_holding;
        bool wasOnScreen;
        Holdable holdable;
        
        public LaniThrowableObject(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            Collider = CreateCollider();
        }

        public LaniThrowableObject(Vector2 position, Vector2 speed) : base(position)
        {
            Collider = CreateCollider();
            this.speed = speed;
        }

        protected virtual Collider CreateCollider()
        {
            return new Hitbox(12, 12, -6, -4);
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = scene as Level;
        }

        public override void Update()
        {
            base.Update();
            MoveH(speed.X * Engine.DeltaTime, onCollideH);
            MoveV(speed.Y * Engine.DeltaTime, onCollideV);
            if (!is_holding)
            {
                //Center += Speed * Engine.DeltaTime;
                speed.Y += 320 * Engine.DeltaTime;
                if (!wasOnScreen)
                {
                    if (level.IsInBounds(this))
                        wasOnScreen = true;
                }
                else if (!level.IsInBounds(this))
                    RemoveSelf();
            }

        }

        protected virtual void onCollideH(CollisionData data)
        {
            if (is_holding) return;
            
            Center -= speed * Engine.DeltaTime;
            speed.X *= -0.75f;
        }

        protected virtual void onCollideV(CollisionData data)
        {
            if (is_holding) return;

            //Center -= Speed * Engine.DeltaTime;
            speed.X *= (data.Hit is LaniIceBlock) ? 0.76f : 0.4f;
            if (Math.Abs(speed.Y) < 20)
                speed.Y = 0;
            else
                speed.Y *= -0.4f;
        }

        public void SetHoldable(bool value = true)
        {
            is_holding = value;
            if (value)
            {
                if (holdable == null)
                    Add(holdable = new Holdable()
                    {
                        PickupCollider = new Hitbox(12f, 24f, -6, 6),
                        OnPickup = new Action(OnPickup),
                        OnRelease = new Action<Vector2>(OnRelease)
                    });
            }
            else
            {
                if (holdable != null)
                {
                    Remove(holdable);
                    holdable = null;
                }
            }
        }

        protected virtual void OnPickup()
        {
            //Logger.Log("LaniThrowableObject", "OnPickup");
            AddTag(Tags.Persistent);
            speed = new Vector2(0f, 0f);
            //is_holding = true;
        }

        protected virtual void OnRelease(Vector2 force)
        {
            //Logger.Log("LaniThrowableObject", "OnRelease");
            speed = new Vector2(Math.Sign(force.X) * 90, -60);
            RemoveTag(Tags.Persistent);
            SetHoldable(false);
        }

        public Vector2 Speed
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

        public Holdable GetHoldable
        {
            get
            {
                return holdable;
            }
        }

        public bool isHolding
        {
            get
            {
                return is_holding;
            }
        }
    }
}
