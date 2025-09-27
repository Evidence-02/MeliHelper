using Celeste;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;
using Celeste.Mod.Entities;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    [CustomEntity("MeliHelper/BattleCityEnemySpawnpoint")]
    class EnemySpawnpoint : Entity
    {
        Sprite sprite;
        int order, id_enemy;
        float timer_active;
        

        public EnemySpawnpoint(EntityData data, Vector2 offset) : base(data.Position + offset) 
        {
            this.order = data.Int("order");
            Add(sprite = GFX.SpriteBank.Create("MeliHelper_BC_Spawnpoint"));
            Visible = false;
        }

        public void Activate(float delay, int id_enemy)
        {
            this.Visible = true;
            this.sprite.Play("idle");
            this.timer_active = delay;
            this.id_enemy = id_enemy;
        }

        public override void Update()
        {
            base.Update();
            if (timer_active > 0)
            {
                timer_active -= Engine.DeltaTime;
                if (timer_active <= 0)
                {
                    Visible = false;
                    //sprite.Play("stop");
                    Field.Instance.GetEnemiesComponent.GenerateEnemy(this.Position, id_enemy);
                }
            }
        }

        public int GetOrder
        {
            get
            {
                return order;
            }
        }
    }
}
