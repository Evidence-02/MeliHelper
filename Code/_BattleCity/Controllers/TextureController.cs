using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    class TextureController
    {
        static Dictionary<BCEnum_CellType, MTexture> dict_cells;
        static Dictionary<BCEnum_EnemyType, MTexture> dict_enemies_ui;
        static Dictionary<BCEnum_EnemyType, MTexture> dict_enemies_bonuses_ui;

        public static void Load()
        {
            if (dict_cells == null)
                dict_cells = new Dictionary<BCEnum_CellType, MTexture>();
            dict_cells[BCEnum_CellType.Brick]   = GFX.Game["Evidence02/objects_bc/tiles/brick01"];
            dict_cells[BCEnum_CellType.Steel]   = GFX.Game["Evidence02/objects_bc/tiles/steel00"];
            dict_cells[BCEnum_CellType.Grass]   = GFX.Game["Evidence02/objects_bc/tiles/grass00"];
            dict_cells[BCEnum_CellType.Water]   = GFX.Game["Evidence02/objects_bc/tiles/water00"];
            dict_cells[BCEnum_CellType.Dirt ]   = GFX.Game["Evidence02/objects_bc/tiles/dirt00"];
            dict_cells[BCEnum_CellType.Brick02] = GFX.Game["Evidence02/objects_bc/tiles/brick02"];

            if (dict_enemies_ui == null)
            {
                dict_enemies_ui = new Dictionary<BCEnum_EnemyType, MTexture>();
                foreach (BCEnum_EnemyType item in Enum.GetValues(typeof(BCEnum_EnemyType)))
                    dict_enemies_ui[item] = GFX.Gui["Evidence02/bc/tank" + item];

                dict_enemies_bonuses_ui = new Dictionary<BCEnum_EnemyType, MTexture>();
                foreach (BCEnum_EnemyType item in Enum.GetValues(typeof(BCEnum_EnemyType)))
                    dict_enemies_bonuses_ui[item] = GFX.Gui["Evidence02/bc/tank" + item + "Bonus"];
            }
        }

        public static MTexture GetCellTypeTexture(BCEnum_CellType cell_type)
        {
            return dict_cells.ContainsKey(cell_type) ? dict_cells[cell_type] : null;
        }

        public static MTexture GetBrickTile(bool is_first_brick)
        {
            return is_first_brick ? dict_cells[BCEnum_CellType.Brick] : dict_cells[BCEnum_CellType.Brick02];
        }

        public static void SetCellTypeTexture(BCEnum_CellType cell_type, MTexture texture)
        {
            dict_cells[cell_type] = texture;
        }

        public static MTexture GetEnemyTextureUI(BCEnum_EnemyType enemy_type, bool is_containing_bonus = false)
        {
            return is_containing_bonus ? dict_enemies_bonuses_ui[enemy_type] : dict_enemies_ui[enemy_type];
        }


        /*
        static Dictionary<string, MTexture[,]> dict_custom_textures;
        public static void RegisterCustomTexture(string path)
        {
            //dict_custom_textures = new Dictionary<string, MTexture[,]>();

            MTexture origin = GFX.Game[path];
            MTexture[,] mass_textures = new MTexture[4,4];
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    mass_textures[i, j] = new MTexture(origin, 4 * i, 4 * j, 4, 4);
            dict_custom_textures[path] = mass_textures;
        }
        */
    }
}
