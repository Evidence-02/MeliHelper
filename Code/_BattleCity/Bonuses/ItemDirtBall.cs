using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    class ItemDirtBall : Item
    {
        Vector2 speed, destination;

        public ItemDirtBall(Field field, Player player) : base(field, player)
        {
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            Sprite sprite = GFX.SpriteBank.Create("MeliHelper_BC_ItemDirtBall");
            sprite.Scale = new Vector2(0.6f);
            Add(sprite);
            Depth = DepthController.BC_ITEM_DIRTBALL;

            destination = player.Center;
            Position = destination;
        }

        public override void Update()
        {
            base.Update();
            if (is_connected)
            {
                dir = (Input.MenuUp.Check ? new Vector2(0, -1) : player.Facing == Facings.Left ? new Vector2(-1, 0) : new Vector2(1, 0));
                destination = player.Center + dist * dir;
                Position += 0.2f * (destination - Position);
                //if (MeliHelperModule.Settings.BC_Shoot.Check)
                //    Shoot();
            }
            else
            {
                //Position += speed * Engine.DeltaTime;
                int tx = field.GetTileCX(Position);
                int ty = field.GetTileCY(Position);
                if (!field.isInField(tx, ty) || field.isActualSolid(tx, ty))
                {
                    Position -= speed * Engine.DeltaTime;
                    Explode();
                }
                else if (level.Entities.FindAll<Enemy>().Exists(t => t.CollidePoint(this.Center)))
                    Explode();
                else
                {
                    Position += speed * Engine.DeltaTime;
                }
            }
        }

        public override void Disconnect()
        {
            base.Disconnect();
            speed = 300 * dir;
            //Position = destination; // ?
        }
        
        public void Explode()
        {
            List<Enemy> list_enemies = level.Entities.FindAll<Enemy>();
            for (int i = 0; i <= 3; i++)
                for (int j = 0; j <= 3; j++)
                    if (!(i % 3 == 0 && j % 3 == 0))
                    {
                        Vector2 pos = Position + new Vector2(8 * i - 12 - Position.X % 8 + 4, 8 * j - 12 - Position.Y % 8 + 4);
                        if (field.isInField(pos) && field.GetCellType(pos) != BCEnum_CellType.Steel)
                        {
                            field.RemoveCell(pos);
                            field.AddCell(BCEnum_CellType.Brick, pos);
                        }

                        if (i > 0 && i < 3 && j > 0 && j < 3)
                        {
                            foreach (var item in list_enemies.FindAll(t => t.CollidePoint(pos)))
                                item.Die(true);
                        }
                    }

            RemoveSelf();
        }

    }
}
