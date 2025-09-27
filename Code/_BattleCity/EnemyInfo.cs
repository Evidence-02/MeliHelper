using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    class EnemyInfo
    {
        EnemyTypeOptions opts;
        Vector2 position;
        char id;
        int num;
        bool is_containing_bonus;
        public Enemy GetEnemy { get; set; }
        public bool isSpawned { get; set; }
        public bool isAppeared { get; set; }
        public bool isDestroyed { get; set; }


        public EnemyInfo(char id, int num, bool is_contains_bonus)
        {
            this.id = id;
            this.num = num;
            this.position = new Vector2(1655 + (num % 2) * 72, 150  + (num / 2) * 72);
            this.is_containing_bonus = is_contains_bonus;
        }

        public EnemyInfo(Enemy enemy, int num, bool is_contains_bonus)
        {
            this.GetEnemy = enemy;
            this.isSpawned = true;
            this.isAppeared = true;
            this.opts = enemy.GetOpts;
            this.num = num;
            this.position = new Vector2(1655 + (num % 2) * 72, 150 + (num / 2) * 72);
            this.is_containing_bonus = is_contains_bonus;
        }

        public void UpdateOpts()
        {
            if (opts == null)
            {
                this.opts = EnemyTypesController.GetEnemyTypeByID(id);
                if (opts == null)
                    throw new Exception("Not found options for type \"" + id + "\"");
            }
        }

        public void RenderUI()
        {
            TextureController.GetEnemyTextureUI(opts.type, is_containing_bonus).DrawCentered(position, Color.White * (isAppeared ? 1f : 0.4f));
            if (isDestroyed)
                GFX.Gui["Evidence02/bc/tankDestroyed"].DrawCentered(position, Color.White);
        }

        public bool isContainsBonus()
        {
            return is_containing_bonus;
        }

        public EnemyTypeOptions GetOpts
        {
            get
            {
                return opts;
            }
        }
    }
}
