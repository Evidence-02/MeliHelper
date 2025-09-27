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
    [CustomEntity("MeliHelper/BattleCityCustomEnemyType")]
    class CustomEnemyType : Entity
    {
        EnemyTypeOptions type;

        public CustomEnemyType(EntityData data, Vector2 offset) : base(data.Position + offset) 
        {
            type = new EnemyTypeOptions(
                id:         data.Char("tankID"),
                type:       (BCEnum_EnemyType)Enum.Parse(typeof(BCEnum_EnemyType), data.Attr("tankType", "Basic")),
                points:     data.Int("points", 100),
                health:     data.Int("health", 1),
                speed_move: data.Float("speedMove", 1),
                speed_bullets: data.Float("speedBullets", 1),
                shoot_frequency: data.Float("shootFrequency", 2.4f),
                is_can_break_through_steel: data.Bool("canDamageSteel")
                );
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            EnemyTypesController.Register(type);
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            RemoveSelf();
        }
    }
}
