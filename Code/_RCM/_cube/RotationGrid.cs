using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class RotationGrid
    {
        float[] grid_coords;

        public RotationGrid(params float[] values)
        {
            this.grid_coords = values;
        }

        public Vector3 CorrectVector3(Vector3 v)
        {
            return new Vector3(CorrectValue(v.X), CorrectValue(v.Y), CorrectValue(v.Z));
        }

        public float CorrectValue(float val)
        {
            float min_res = float.MaxValue;
            float min_del = float.MaxValue;
            foreach (float g in grid_coords)
            {
                float del = Math.Abs(g - val);
                if (min_del > del)
                {
                    min_del = del;
                    min_res = g;
                }
            }
            return min_res;
        }

    }
}
