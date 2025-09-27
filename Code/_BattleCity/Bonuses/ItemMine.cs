using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._BattleCity._Bonuses
{
    class ItemMine : Item
    {
        public ItemMine(Field field, Player player) : base(field, player)
        {
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            Add(GFX.SpriteBank.Create("MeliHelper_BC_ItemMine"));
            Depth = DepthController.BC_ITEM_MINE;
        }

        public override void Update()
        {
            base.Update();
            if (!is_connected)
            {


            }
        }

        public override void Disconnect()
        {
            base.Disconnect();
            //Audio.Play();
        }
        
        public void Explode()
        {
            //Audio.Play();
            int cx = field.GetCellCX(Position);
            int cy = field.GetCellCY(Position);

            //for (int i = -2; i <= 2; i++)
            //    for (int j = -2; j <= 2; j++)
            for (int i = 0; i <= 0; i++)
                for (int j = 0; j <= 0; j++)
                {
                    int tx1 = 4 * cx + 4 * i;
                    int ty1 = 4 * cy + 4 * j;
                    if (Math.Abs(i) + Math.Abs(j) < 4 && field.isInField(tx1, ty1))
                    {
                            // Fill field cell with a blocks!
                            for (int ii = 0; ii < 4; ii++)
                                for (int jj = 0; jj < 4; jj++)
                                    if (field.GetCellType(tx1 + ii, ty1 + jj) == BCEnum_CellType.Empty)
                                        field.AddCellBrickTile(tx1 + ii, ty1 + jj);
                    }
                }

            RemoveSelf();
        }

        public override void Render()
        {
            // !!! debug !!!
            base.Render();
            if (true)
            {
                int cx = field.GetCellCX(Position);
                int cy = field.GetCellCY(Position);
                field.ShowDebugInfo(cx + "," + cy);
            }
            else
            {
            }
        }

    }
}
