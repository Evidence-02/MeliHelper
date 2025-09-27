using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._BattleCity._Bonuses
{
    class Shield : Entity
    {
        Level level;
        Entity parent;

        public Shield(Entity parent)
        {
            this.parent = parent;
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            level = scene as Level;
            Add(GFX.SpriteBank.Create("MeliHelper_BC_ItemShield"));
        }

        public override void Update()
        {
            base.Update();
            Center = parent.Center;
            foreach (Bullet bullet in level.Entities.FindAll<Bullet>().FindAll(t => !t.isFromPlayer
                && this.Center.X - 8 <= t.Center.X && t.Center.X <= this.Center.X + 8
                && this.Center.Y - 8 <= t.Center.Y && t.Center.Y <= this.Center.Y + 8))
                ReflectBullet(bullet);
        }

        public void ReflectBullet(Bullet bullet)
        {
            bullet.isFromPlayer = true;
            bullet.SetColor = Color.OrangeRed;
            bullet.GetSpeed *= -1;
        }


    }
}
