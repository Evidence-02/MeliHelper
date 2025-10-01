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

namespace Celeste.Mod.MeliHelper._Lani
{
    [CustomEntity("MeliHelper/ObjectGravityTrigger")]
    class ObjectGravityTrigger : Trigger
    {
        Level level;
        LaniThrowableObject holdable;
        float timer, period;

        public ObjectGravityTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            // - types:
            // TheoCrystal
            // LaniThrowableObject

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

            if (holdable == null)
            {
                holdable = level.Entities.FindAll<LaniThrowableObject>().FirstOrDefault(t => !t.isHolding && CollidePoint(t.Center));
                if (holdable != null)
                    MoveObjectToBottom(holdable);
            }

            if (holdable != null)
            {
                if (   holdable.CenterY >= Bottom 
                    || Vector2.Distance(holdable.Center, this.BottomCenter) <= 4 && holdable.Speed.Length() <= 10)
                    RemoveSelf();
                else
                {
                    timer += Engine.DeltaTime;
                    if (timer >= period)
                    {
                        timer = 0;
                        MoveObjectToBottom(holdable);
                    }
                }
            }
        }

        public void MoveObjectToBottom(LaniThrowableObject obj, float koefX = 0.24f)
        {
            // S = S0 + V0*x + (a*x^2)/2
            // need to find time at first

            // equation for Y:
            // BottomCenter.Y = obj.Center.Y + obj.Speed.Y * [t] + 320 * Engine.DeltaTime * [t] * [t] / 2
            float a = 320 * Engine.DeltaTime * Engine.DeltaTime / 2;
            float b = obj.Speed.Y * Engine.DeltaTime;
            float c = obj.Center.Y - BottomCenter.Y;
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
                float destX = (BottomCenter.X - obj.Center.X) / (x * Engine.DeltaTime);
                float speedX = destX * koefX + obj.Speed.X * (1 - koefX);
                //Logger.Log("ObjectGravityTrigger", $"obj.Speed.X={obj.Speed.X};  destX={destX};  speedX={speedX}");
                obj.Speed = new Vector2(speedX, obj.Speed.Y);
                

                
            }
        }
    }
}
