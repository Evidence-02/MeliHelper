using Celeste;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;
using Celeste.Mod.Entities;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    [CustomEntity("MeliHelper/BattleCityCustomCellTextures")]
    class CustomCellTextures : Entity
    {
        Dictionary<BCEnum_CellType, MTexture> dict_textures;

        public CustomCellTextures(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            dict_textures = new Dictionary<BCEnum_CellType, MTexture>();
            foreach (BCEnum_CellType cell_type in Enum.GetValues(typeof(BCEnum_CellType)))
            {
                string texture = data.Attr(cell_type.ToString().ToLower());
                if (texture != null) dict_textures[cell_type] = GFX.Game[texture];
            }
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            foreach (var item in dict_textures)
                TextureController.SetCellTypeTexture(item.Key, item.Value);
            RemoveSelf();
        }
    }
}
