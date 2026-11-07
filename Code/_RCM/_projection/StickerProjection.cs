using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class StickerProjection
    {
        protected CubeProjection parent;
        protected Sticker sticker;
        protected int length;
        protected List<StickerProjectionElement> list_elements;
        public double GetZ { get; protected set; }
        
        public StickerProjection(CubeProjection parent, Sticker sticker)
        {
            this.parent = parent;
            this.sticker = sticker;
            this.length = sticker.GetVertices.Length;
            sticker.RegisterProjection(this);
        }

        public virtual void SetSlices(float length)
        {
        }

        public virtual void Recalculate()
        {
        }

        public virtual void Render()
        {
        }

        public List<StickerProjectionElement> GetListElements
        {
            get
            {
                return list_elements;
            }
        }
    }
}
