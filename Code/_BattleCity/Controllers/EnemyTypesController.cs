using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    static class EnemyTypesController
    {
        static List<EnemyTypeOptions> list_types_current;
        static List<EnemyTypeOptions> list_types_default;

        public static void Initialize()
        {
            list_types_current = new List<EnemyTypeOptions>();
            list_types_default = new List<EnemyTypeOptions>();

            //         !!! CAUTION !!!
            // Synchronized with meliLib.lua (also used in CustomEnemyType.lua, Tank.lua)
            list_types_default.Add(new EnemyTypeOptions('B', BCEnum_EnemyType.Basic,   100, 1, 36, 100, 2.4f));
            list_types_default.Add(new EnemyTypeOptions('F', BCEnum_EnemyType.Fast,    200, 1, 60, 150, 2.4f)); 
            list_types_default.Add(new EnemyTypeOptions('P', BCEnum_EnemyType.Power,   300, 1, 48, 200, 1.8f, true));
            list_types_default.Add(new EnemyTypeOptions('A', BCEnum_EnemyType.Armored, 400, 4, 48, 150, 2.4f));
            // custom types
            list_types_default.Add(new EnemyTypeOptions('Z', BCEnum_EnemyType.Basic, 100, 1, 36, 100, 2.4f));
            list_types_default.Add(new EnemyTypeOptions('Y', BCEnum_EnemyType.Basic, 100, 1, 36, 100, 2.4f));
            list_types_default.Add(new EnemyTypeOptions('X', BCEnum_EnemyType.Basic, 100, 1, 36, 100, 2.4f));
            list_types_default.Add(new EnemyTypeOptions('W', BCEnum_EnemyType.Basic, 100, 1, 36, 100, 2.4f));
            list_types_default.Add(new EnemyTypeOptions('V', BCEnum_EnemyType.Basic, 100, 1, 36, 100, 2.4f));
            list_types_default.Add(new EnemyTypeOptions('U', BCEnum_EnemyType.Basic, 100, 1, 36, 100, 2.4f));
            ResetToDefault();
        }

        public static void ResetToDefault()
        {
            list_types_current.Clear();
            list_types_current.AddRange(list_types_default);
        }

        public static void Register(EnemyTypeOptions type)
        {
            list_types_current.RemoveAll(t => t.id == type.id);
            list_types_current.Add(type);
        }

        public static EnemyTypeOptions GetEnemyTypeByID(char id)
        {
            return list_types_current.FirstOrDefault(t => t.id == id);
        }
    }

    class EnemyTypeOptions
    {
        public BCEnum_EnemyType type;
        public char id;
        public string id_sprite;
        public int points, health;
        public float speed_move, speed_bullets, shoot_frequency;
        public bool is_can_break_through_steel;

        public EnemyTypeOptions(char id, BCEnum_EnemyType type,
            int points, int health,
            float speed_move, float speed_bullets, float shoot_frequency, bool is_can_break_through_steel = false)
        {
            this.id = id;
            this.type = type;
            switch (type)
            {
                case BCEnum_EnemyType.Basic:   id_sprite = "4"; break;
                case BCEnum_EnemyType.Fast:    id_sprite = "5"; break;
                case BCEnum_EnemyType.Power:   id_sprite = "6"; break;
                case BCEnum_EnemyType.Armored: id_sprite = "7"; break;
            }
            this.points = points;
            this.health = health;
            this.speed_move = speed_move;
            this.speed_bullets = speed_bullets;
            this.shoot_frequency = shoot_frequency;
            this.is_can_break_through_steel = is_can_break_through_steel;
        }
    }
}
