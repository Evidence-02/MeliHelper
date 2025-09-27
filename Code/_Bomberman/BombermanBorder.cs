using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    [CustomEntity("MeliHelper/BombermanStation")]
    class BombermanStation : Entity
    {
        Sprite sprite;
        Solid solid_center;

        public BombermanStation(EntityData data, Vector2 offset) 
            : base(data.Position + offset + new Vector2(-24, -16))
        {
            Add(sprite = GFX.SpriteBank.Create("MeliHelper_BombermanStation"));
            sprite.Position += new Vector2(24, 16);
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);

            Level level = scene as Level;
            level.Add(new Solid(this.Position + new Vector2(  0,   0), 48, 16, true));    // top
            level.Add(new Solid(this.Position + new Vector2(  0, +16), 12, 32, true));    // left
            level.Add(new Solid(this.Position + new Vector2(+36, +16), 12, 32, true));    // right
            level.Add(solid_center = new Solid(this.Position + new Vector2(+12, +16), 24, 24, true));    // center

            if (_BattleCity.Field.Instance != null)
            {
                for (int i = 0; i < 6; i++)
                    for (int j = 0; j < 6; j++)
                        _BattleCity.Field.Instance.AddCell(BCEnum_CellType.Blocked, Position + 8 * new Vector2(i, j));
            }
        }

        public void Open()
        {
            Audio.Play("event:/meli/bomberman_capsule_destroyed");
            sprite.Play("open");
            solid_center.RemoveSelf();

        }



    }
}
