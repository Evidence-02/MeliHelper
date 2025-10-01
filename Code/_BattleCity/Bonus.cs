using Celeste;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    class Bonus : Entity
    {
        protected BCEnum_BonusType type;
        protected Image image;
        
        public Bonus(Vector2 center, BCEnum_BonusType type) : base(center - new Vector2(8,8)) 
        {
            this.type = type;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            Add(new PlayerCollider(onPlayer, new Hitbox(Field.PIX_CELL, Field.PIX_CELL)));
            Add(image = new Image(GFX.Game["Evidence02/objects_bc/bonuses/item" + type.ToString().LikeName()]));
        }

        protected virtual void onPlayer(Player player)
        {
            Audio.Play(type == BCEnum_BonusType.ExtraLife ? SoundController.BC_POWERUP_1UP : SoundController.BC_POWERUP_OBTAINED);
            ActivateBonus(player);
        }

        protected virtual void ActivateBonus(Player player)
        {
            Level level = this.SceneAs<Level>();
            Field field = Field.Instance;
            EventUI ui = field.GetEventUI;
            BattleCityPlayerInfo info = ProgressController.GetCurrentPlayerInfo();
            Color color_text = Color.White;
            string type_text = "ordinary";
            bool is_gen_new_bonus = false;
            switch (type)
            {
                //------------------------
                // Vanilla
                case BCEnum_BonusType.Star:
                    if (info.Stars < 5)
                    {
                        int id_star = info.Stars++;
                        Vector2 pos = Methods.CoordsToHUD(level, this.Center);
                        Vector2 dest = field.GetEventUI.GetStarPosition(id_star);
                        Field.Instance.GetEventUI.StarSetDelay(id_star, true);
                        level.Add(new StarHUD(pos, dest, 900 * Vector2.Normalize(Calc.Perpendicular(dest - pos)), id_star));
                    }
                    else is_gen_new_bonus = true;
                    type_text = ""; // Visuals are already here!
                    break;

                case BCEnum_BonusType.Grenade:
                    foreach (Enemy enemy in level.Entities.FindAll<Enemy>())
                        enemy.Die(false);
                    break;

                case BCEnum_BonusType.ExtraLife:
                    info.Lifes++;
                    type_text = "gradient";
                    break;

                case BCEnum_BonusType.Shield: ui.AddEvent(player, BCEnum_BonusEvent.Shield); break;
                case BCEnum_BonusType.Shovel: ui.AddEvent(player, BCEnum_BonusEvent.Shovel); break;
                case BCEnum_BonusType.TimeStop: ui.AddEvent(player, BCEnum_BonusEvent.TimeStop); break;

                //------------------------
                // Custom
                case BCEnum_BonusType.EMI: ui.AddEvent(player, BCEnum_BonusEvent.EMI); break;
                case BCEnum_BonusType.Duality: ui.AddEvent(player, BCEnum_BonusEvent.Duality); break;
                case BCEnum_BonusType.UnlimitedShooting: ui.AddEvent(player, BCEnum_BonusEvent.UnlimitedShooting); break;
                case BCEnum_BonusType.HomingBullets: ui.AddEvent(player, BCEnum_BonusEvent.HomingBullets); break;
                case BCEnum_BonusType.MoveThroughWater:
                    if (!info.MoveThroughWater)
                    {
                        if (!ProgressController.PlayerCanMoveThroughWater())
                            foreach (FieldCellWater cell_water in level.Entities.FindAll<FieldCellWater>())
                                cell_water.SetMoveThrough(true);
                        info.MoveThroughWater = true;
                    }
                    else
                    {
                        is_gen_new_bonus = true;
                    }
                    break;

                //------------------------
                // Items
                case BCEnum_BonusType.DirtBall: field.GetItemComponent.AddItem(new ItemDirtBall(field, player)); color_text = Color.Brown; break;
                case BCEnum_BonusType.Mine:     field.GetItemComponent.AddItem(new ItemMine    (field, player)); color_text = Color.Yellow; break;
                case BCEnum_BonusType.DemolitionBomb: level.Add(new Dynamite(this.Center)); color_text = Color.Red; break;
            }


            string dialogue_id = "EVIDENCE02_BATTLECITY_ITEMNAME_" + type.ToString().ToUpper();
            if (Dialog.Has(dialogue_id))
            {
                Vector2 pos = this.Position + new Vector2(0, -10);
                string floaty_label = Dialog.Clean(dialogue_id);
                switch (type_text)
                {
                    case "gradient": level.Add(new TextOutlineEntityGradient(pos, floaty_label)); break;
                    case "ordinary": level.Add(new TextOutlineEntity(pos, floaty_label, color_text)); break;
                }
            }

            if (is_gen_new_bonus)
            {
                level.Add(new TextOutlineEntityGradient(this.Position + new Vector2(0, 6), "Reroll!"));
                this.Scene.Add(new BonusDefault(
                    Field.Instance.GetPositionForBonus(),
                    BonusesController.GetRandomBonus(),
                    12f
                    ));
            }
        }
    }
}
