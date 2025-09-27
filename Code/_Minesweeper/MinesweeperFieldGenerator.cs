using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._Minesweeper
{
    class MinesweeperFieldGenerator
    {
        public static char[,] GenerateField(int w, int h, int count_bombs)
        {
            char[,] res = new char[w, h];
            int counter = 18;
            do
            {
                Calc.PushRandom(++MeliHelperModule.Instance.SaveData.MinesweeperFieldID);
                List<Point> list_empty_cells = new List<Point>();
                for (int i = 0; i < w; i++)
                    for (int j = 0; j < h; j++)
                    {
                        res[i, j] = '-';
                        list_empty_cells.Add(new Point(i, j));
                    }

                for (int i = 0; i < count_bombs; i++)
                {
                    int index = Calc.Random.Next(0, list_empty_cells.Count);
                    res[list_empty_cells[index].X, list_empty_cells[index].Y] = 'B';
                    list_empty_cells.RemoveAt(index);
                }
                Calc.PopRandom();
            }
            while (!isAppropriateField(w, h, res) && --counter >= 0);

            return res;
        }

        static bool isAppropriateField(int w, int h, char[,] res)
        {
            char[,] temp = new char[w, h];
            for (int i = 0; i < w; i++)
                for (int j = 0; j < h; j++)
                    temp[i, j] = res[i, j];

            // Find first cell
            List<Point> list_coords_check = new List<Point>();
            for (int i = 0; i < w; i++)
                if (list_coords_check.Count == 0)
                    for (int j = 0; j < h; j++)
                        if (temp[i, j] != 'B')
                        {
                            list_coords_check.Add(new Point(i, j));
                            temp[i, j] = 'C';
                            break;
                        }

            // Check all cells
            int dx = 0, dy = 0, nx = 0, ny = 0;
            while (list_coords_check.Count > 0)
            {
                int cx = list_coords_check[0].X;
                int cy = list_coords_check[0].Y;
                foreach (DirectionEnum dir in Enum.GetValues(typeof(DirectionEnum)))
                {
                    Methods.GetDirectionParams(dir, ref dx, ref dy);
                    nx = cx + dx;
                    ny = cy + dy;
                    if (nx >= 0 && nx < w && ny >= 0 && ny < h && temp[nx, ny] == '-')
                    {
                        list_coords_check.Add(new Point(nx, ny));
                        temp[nx, ny] = 'C';
                    }
                }
                list_coords_check.RemoveAt(0);
            }

            // Final check
            for (int i = 0; i < w; i++)
                for (int j = 0; j < h; j++)
                    if (temp[i, j] == '-')
                        return false;

            return true;
        }



    }
}
