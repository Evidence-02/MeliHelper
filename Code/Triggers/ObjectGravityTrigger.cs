using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Celeste.Mod.MeliHelper._Lani;

namespace Celeste.Mod.MeliHelper
{
    [CustomEntity("MeliHelper/ObjectGravityTrigger")]
    class ObjectGravityTrigger : Trigger
    {
        Level level;
        Entity entity;
        string objtype;
        float timer, period;

        public ObjectGravityTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            // - types:
            // Player
            // TheoCrystal
            // LaniHoldable
            objtype = data.Attr("objectType", "Player");

            period = 0.16f;
            timer = 0;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = scene as Level;
        }

        public override void Update()
        {
            base.Update();

            if (entity == null)
            {
                entity = TryGetEntity();
                if (entity != null)
                    MoveObjectToBottom();
            }
            else
            {
                if (   entity.CenterY >= Bottom 
                    || Vector2.Distance(entity.Center, this.BottomCenter) <= 4 && GetSpeed().Length() <= 10)
                    RemoveSelf();
                else
                {
                    timer += Engine.DeltaTime;
                    if (timer >= period)
                    {
                        timer = 0;
                        MoveObjectToBottom();
                    }
                }
            }
        }

        public void MoveObjectToBottom(float koefX = 0.24f)
        {
            // S = S0 + V0*x + (a*x^2)/2
            // need to find time at first

            // equation for Y:
            // BottomCenter.Y = obj.Center.Y + obj.Speed.Y * [t] + 320 * Engine.DeltaTime * [t] * [t] / 2
            Vector2 speed = GetSpeed();
            float a = 320 * Engine.DeltaTime * Engine.DeltaTime / 2;
            float b = speed.Y * Engine.DeltaTime;
            float c = entity.Center.Y - BottomCenter.Y;
            float D = b * b - 4 * a * c;
            //Logger.Log("ObjectGravityTrigger", $"a={a};  b={b};  c={c};  D={D}");
            if (D >= 0)
            {
                float x1 = (-b + (float)Math.Sqrt(D)) / (2 * a);
                float x2 = (-b - (float)Math.Sqrt(D)) / (2 * a);
                float x = (x1 != float.NaN && !float.IsInfinity(x1) && x1 > 0) ? x1 : 
                          (x1 != float.NaN && !float.IsInfinity(x1) && x2 > 0) ? x2 : 0;
                //Logger.Log("ObjectGravityTrigger", $"x1={x1};  x2={x2};  x={x}");
                if (x <= 0 || x == float.NaN || float.IsInfinity(x))
                {
                    //Logger.Log("ObjectGravityTrigger", $"destroy! (a={a};  b={b};  c={c};  D={D};  x1={x1};  x2={x2};  x={x})");
                    RemoveSelf();
                }


                // now set speed.X for holdable thing
                // equation for X (accet only for Y): 
                // bottom.X = obj.Center.X + holdable.Speed.X * [t]
                float destX = (BottomCenter.X - entity.Center.X) / (x * Engine.DeltaTime);
                float speedX = destX * koefX + speed.X * (1 - koefX);
                //Logger.Log("ObjectGravityTrigger", $"obj.Speed.X={speed.X};  destX={destX};  speedX={speedX}");
                SetSpeed(new Vector2(speedX, speed.Y));
            }
        }

        Entity TryGetEntity()
        {
            switch (objtype)
            {
                case "Player": return level.Entities.FindAll<Player>().FirstOrDefault(t => CollidePoint(t.Center));
                case "TheoCrystal": return level.Entities.FindAll<TheoCrystal>().FirstOrDefault(t => CollidePoint(t.Center));
                case "LaniHoldable": return level.Entities.FindAll<LaniThrowableObject>().FirstOrDefault(t => !t.isHolding && CollidePoint(t.Center));
            }
            return null;
        }

        Vector2 GetSpeed()
        {
            if (entity == null) return Vector2.Zero;
            switch (objtype)
            {
                case "Player": return (entity as Player).Speed;
                case "TheoCrystal": return (entity as TheoCrystal).Speed;
                case "LaniHoldable": return (entity as LaniThrowableObject).Speed;
                default: return Vector2.Zero;
            }
        }

        void SetSpeed(Vector2 value)
        {
            switch (objtype)
            {
                case "Player": (entity as Player).Speed = value; break;
                case "TheoCrystal": (entity as TheoCrystal).Speed = value; break;
                case "LaniHoldable": (entity as LaniThrowableObject).Speed = value; break;
            }
        }

    }
}
