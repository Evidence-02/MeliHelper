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
    [CustomEntity("MeliHelper/BattleCityCustomBonusChances")]
    class CustomBonusChances : Entity
    {
        Dictionary<BCEnum_BonusType, float> dict_chances;

        public CustomBonusChances(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            dict_chances = new Dictionary<BCEnum_BonusType, float>();
            foreach (BCEnum_BonusType type in Enum.GetValues(typeof(BCEnum_BonusType)))
            {
                float val = data.Float(type.ToString(), -1);
                if (val >= 0) dict_chances[type] = val;
            }
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            BonusesController.SetCustom(dict_chances);
            RemoveSelf();
        }
    }
}
