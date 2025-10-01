using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._Bomberman
{
    [CustomEntity("MeliHelper/BombermanCapsule")]
    class BombermanCapsule : Solid
    {
        Sprite sprite;
        int lifes;
        bool is_opened;

        public BombermanCapsule(EntityData data, Vector2 offset) : base(data.Position + offset + new Vector2(-8, 0), 16, 16, true)
        {
            Add(sprite = GFX.SpriteBank.Create("MeliHelper_BombermanCapsule"));
            sprite.Position += new Vector2(8, 0);
            lifes = data.Int("lifes");
            this.OnDashCollide += onDashCollide;
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            UpdateBattleCityField(true);
        }

        protected virtual DashCollisionResults onDashCollide(Player player, Vector2 dir)
        {
            Hit();
            return DashCollisionResults.Bounce;
        }

        public void Hit()
        {
            lifes--;
            Audio.Play(SoundController.BC_FIRING_THE_ENEMY_BIG_TANK);
            sprite.Play("hit");
            if (lifes <= 0)
            {
                Level level = SceneAs<Level>();
                level.Add(new TextOutlineEntity(this.Center, "800", Color.White));
                is_opened = true;
                if (!level.Entities.FindAll<BombermanCapsule>().Exists(t => !t.is_opened))
                {
                    foreach (var item in level.Entities.FindAll<BombermanStation>())
                        item.Open();
                }
                UpdateBattleCityField(false);
                RemoveSelf();
            }
        }

        public void UpdateBattleCityField(bool is_blocked)
        {
            if (is_blocked)
                for (int i = 0; i < 2; i++)
                    for (int j = 0; j < 2; j++)
                        _BattleCity.Field.Instance.AddCell(BCEnum_CellType.Blocked, Position + 8 * new Vector2(i, j));
            else
                for (int i = 0; i < 2; i++)
                    for (int j = 0; j < 2; j++)
                        _BattleCity.Field.Instance.RemoveCell(Position + 8 * new Vector2(i, j));

        }



    }
}
