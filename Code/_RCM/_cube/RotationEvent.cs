using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class RotationEvent
    {
        Dictionary<Vector3, Vector3> dict_moves;
        List<Sticker> list_stickers; 
        Axis axis;
        double angle, angle_res, ttl;
        public bool isFinished { get; private set; }
        
        RotationEvent(Axis axis, List<Sticker> list_stickers, double angle, float millis_ttl)
        {
            this.axis = axis;
            this.list_stickers = list_stickers;
            this.angle_res = angle;
            this.angle = 0;
            this.ttl = millis_ttl;
            this.dict_moves = new Dictionary<Vector3, Vector3>();
        }

        public RotationEvent(Axis axis, List<Sticker> list_stickers, double angle, float millis_ttl, string type_moves) 
            : this(axis, list_stickers, angle, millis_ttl)
        {
            List<Vector3> list_vectors_start = new List<Vector3>();
            foreach (Sticker sticker in list_stickers)
                list_vectors_start.AddRange(sticker.GetVertices);
            
            // Correct all vertices on sticker
            foreach (Sticker sticker in list_stickers)
                foreach (Vector3 v in sticker.GetVerticesSaved)
                {
                    Vector3 res = axis.RotatePointAroundAxis(v, angle_res);
                    Vector3 min_v = Vector3.Zero;
                    float temp_distance, min_distance = float.MaxValue;
                    foreach (Vector3 v_start in list_vectors_start)
                    {
                        temp_distance = (res - v_start).Length();
                        if (temp_distance < min_distance)
                        {
                            min_v = v_start;
                            min_distance = temp_distance;
                        }
                    }
                    dict_moves[v] = (min_distance < 0.01f) ? min_v : res;
                }
        }

        public RotationEvent(Axis axis, List<Sticker> list_stickers, double angle, float millis_ttl, RotationGrid grid)
            : this(axis, list_stickers, angle, millis_ttl)
        {
            foreach (Sticker sticker in list_stickers)
                foreach (Vector3 v in sticker.GetVertices)
                {
                    Vector3 res = axis.RotatePointAroundAxis(v, angle_res);
                    dict_moves[v] = (sticker.isFaceSticker) ? grid.CorrectVector3(res) : res;
                }
        }

        public void Update()
        {
            ttl -= Engine.DeltaTime;
            if (ttl <= 0)
                Finish();
            else
            {
                // temporal 
                angle += 0.33f * (angle_res - angle);
                foreach (var item in list_stickers)
                    item.Rotate(axis, angle);
            }
        }

        public void Finish()
        {
            isFinished = true;
            foreach (Sticker sticker in list_stickers)
            {
                for (int i = 0; i < sticker.GetVertices.Length; i++)
                    sticker.GetVertices[i] = dict_moves[sticker.GetVerticesSaved[i]];
                sticker.Save();
            }
        }



    }
}
