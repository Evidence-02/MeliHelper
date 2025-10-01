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
        public static char[,] GenerateFieldFromSeed(int w, int h, int count_bombs, int seed)
        {
            char[,] res = new char[w, h];
            int nx, ny;

            Calc.PushRandom(seed);
            List<Point> list_empty_cells = new List<Point>();
            for (int i = 0; i < w; i++)
                for (int j = 0; j < h; j++)
                {
                    res[i, j] = '0';
                    list_empty_cells.Add(new Point(i, j));
                }

            for (int k = 0; k < count_bombs; k++)
            {
                int index = Calc.Random.Next(0, list_empty_cells.Count);
                int x = list_empty_cells[index].X;
                int y = list_empty_cells[index].Y;
                res[x, y] = 'B';    // mark cell as bomb

                // Add count of neighbors to... the neighbors?
                for (int dx = -1; dx <= 1; dx++)
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        nx = x + dx;
                        ny = y + dy;
                        if (nx >= 0 && nx < w && ny >= 0 && ny < h && res[nx, ny] != 'B')
                            res[x + dx, y + dy]++;
                    }

                list_empty_cells.RemoveAt(index);
            }
            Calc.PopRandom();

            return res;
        }

        public static bool isAppropriateField(int w, int h, char[,] res)
        {
            // States:
            // 'B' - Bomb
            // '-' - not checked
            // 'C' - checked cell, exists path there

            char[,] temp = new char[w, h];
            for (int i = 0; i < w; i++)
                for (int j = 0; j < h; j++)
                    temp[i, j] = (res[i, j] == 'B') ? 'B' : '-';
            
            // Check first cell
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

            // Try check all cells
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

            // If there any unchecked cells, it's a bad field
            for (int i = 0; i < w; i++)
                for (int j = 0; j < h; j++)
                    if (temp[i, j] == '-')
                        return false;

            return true;
        }



    }
}
