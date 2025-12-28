using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    class FieldEnemiesComponent : Component
    {
        Field field;
        List<EnemyInfo> list_enemy_info;
        EnemySpawnpoint[] mass_spawnpoints;
        Dictionary<BCEnum_EnemyType, List<int>> dict_enemy_points;
        float timer_spawn, delay_spawn, period_spawn;
        int id_enemy_to_spawn, id_spawnpoint, count_enemies_on_screen, max_enemies_on_screen;
        bool is_all_enemies_spawned;

        public FieldEnemiesComponent(Field field, EntityData data) : base(true, true)
        {
            this.field = field;
            period_spawn = data.Float("enemyRespawnPeriod", 2.7f);
            delay_spawn = data.Float("enemyRespawnDelay", 0.9f);
            max_enemies_on_screen = data.Int("enemiesOnScreen", 4);
            dict_enemy_points = new Dictionary<BCEnum_EnemyType, List<int>>();
            id_enemy_to_spawn = -1;
            foreach (BCEnum_EnemyType type in Enum.GetValues(typeof(BCEnum_EnemyType)))
                dict_enemy_points[type] = new List<int>();

            // Default enemy types: Basic (B), Fast (F), Power (P), Armored (A). All their params are inside of "EnemyTypesController" class
            // A few list format types:
            // "BBBBBBBBBBFFFPPPPP"   - by default
            // "BBBBBBBBBB,FFF,PPPPP" - literally the same thing but reads better
            // "Bx12,Fx3,Px5"           - learn how to count
            // "Bx12,FFF,PPPPP"         - mixed

            // THIS WILL NOT WORK:
            // "BFx10" - fast trying to "BFBFBFBFBFBFBFBFBFBF"
            // UPD: okay, maybe it works

            list_enemy_info = new List<EnemyInfo>();
            int temp;
            int[] list_bonuses_id = Array.ConvertAll(data.Attr("bonusList", "4,11,18").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries), int.Parse);
            string[] enemy_list = data.Attr("enemyList").Replace(" ", "").Split(',');
            foreach (string code in enemy_list)
            {
                string mass_enemies = code;
                if (code.Contains("x") && int.TryParse(code.After("x"), out temp))
                {
                    if (code.Before("x").Length == 1) mass_enemies = new string(code[0], temp);
                    else mass_enemies = string.Concat(Enumerable.Repeat(code, temp));  // experimental function, found on stack overflow
                }
                foreach (char ch in mass_enemies)
                    list_enemy_info.Add(new EnemyInfo(ch, list_enemy_info.Count, list_bonuses_id.Contains(list_enemy_info.Count + 1)));
            }
        }

        public override void EntityAwake()
        {
            base.EntityAwake();
            mass_spawnpoints = Entity.SceneAs<Level>().Entities.FindAll<EnemySpawnpoint>().OrderBy(t => t.GetOrder).ToArray();
            foreach (EnemyInfo info in list_enemy_info)
                info.UpdateOpts();
        }

        public override void Update()
        {
            if (field.GetGameState != BCEnum_GameState.Normal)
                return;

            if (mass_spawnpoints.Length > 0 && !is_all_enemies_spawned)
            {
                timer_spawn -= Engine.DeltaTime;
                if (timer_spawn <= 0 && count_enemies_on_screen < max_enemies_on_screen)
                {
                    while (++id_enemy_to_spawn < list_enemy_info.Count && list_enemy_info[id_enemy_to_spawn].isSpawned) { }
                    if (id_enemy_to_spawn < 0 || id_enemy_to_spawn >= list_enemy_info.Count)
                        return;

                    EnemyInfo info = list_enemy_info[id_enemy_to_spawn];
                    mass_spawnpoints[id_spawnpoint].Activate(delay_spawn, id_enemy_to_spawn);
                    count_enemies_on_screen++;
                    timer_spawn = period_spawn * (MeliHelperModule.Settings.DebugToolsBC.EnemiesFasterSpawn ? 0.1f : 1f);
                    if (++id_spawnpoint >= mass_spawnpoints.Length)
                        id_spawnpoint = 0;

                    if (!list_enemy_info.Exists(t => !t.isSpawned))
                        is_all_enemies_spawned = true;
                }
            }
        }

        public void RenderUI()
        {
            foreach (var item in list_enemy_info)
                item.RenderUI();
        }

        public void GenerateEnemy(Vector2 position, int id_enemy)
        {
            EnemyInfo info = list_enemy_info[id_enemy];
            Enemy enemy = new Enemy(field, position, info.GetOpts,
                is_contains_bonus: info.isContainsBonus() || MeliHelperModule.Settings.DebugToolsBC.BonusesEverytime);
            field.SceneAs<Level>().Add(enemy);
            info.GetEnemy = enemy;
            info.isAppeared = true;
        }

        public void KillEnemy(Enemy enemy, bool is_register_statistic)
        {
            count_enemies_on_screen--;
            EnemyInfo info = list_enemy_info.FirstOrDefault(t => t.GetEnemy == enemy);
            info.isDestroyed = true;
            if (is_register_statistic)
                dict_enemy_points[enemy.GetOpts.type].Add(enemy.GetOpts.points);
            if (!list_enemy_info.Exists(t => !t.isDestroyed))
                field.CheckFinish(BCEnum_Goal.KillEnemies);
        }
        
        public void RegisterEnemy(Enemy enemy)
        {
            list_enemy_info.Add(new EnemyInfo(enemy, list_enemy_info.Count, false));
        }

        public List<int> GetListCollectedPoints(BCEnum_EnemyType type)
        {
            return dict_enemy_points[type];
        }

    }
}
