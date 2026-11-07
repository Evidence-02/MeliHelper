using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class CubeProjection3 : CubeProjection
    {
        public CubeProjection3(Vector2 center, List<Sticker> list_stickers, float dx = 0, float dy = 0, float size = 1, bool is_show_borders = true)
            : base(center, list_stickers, dx: dx, dy: dy, size: size, is_show_borders: is_show_borders)
        {
            list_projections = new List<StickerProjection>();
            foreach (Sticker sticker in list_stickers)
                list_projections.Add(new StickerProjection3(this, sticker));
        }
        
        public override void Render()
        {
            List<StickerProjectionElement> list_elems = new List<StickerProjectionElement>();
            foreach (var item in list_projections)
                list_elems.AddRange(item.GetListElements);
            foreach (var item in list_elems.OrderBy(t => t.GetZ))
                item.Render();
        }
    }
}
