using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;
using System.Collections;

namespace Celeste.Mod.MeliHelper
{
    [CustomEntity("MeliHelper/MeliHelperConditionTrigger")]
    class MeliHelperConditionTrigger : Trigger
    {
        Level level;
        Rectangle rect;
        Coroutine coroutine;
        List<ConditionEnum> list_conditions;
        CustomTimer timer;
        string trigger, action, param;
        bool is_one_use, is_activated, is_timer_trigger;

        public MeliHelperConditionTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            rect = new Rectangle((int)(data.Position.X + offset.X), (int)(data.Position.Y + offset.Y), data.Width, data.Height);
            action = data.Attr("action");
            is_one_use = data.Bool("oneUse");
            trigger = data.Attr("trigger");
            param = data.Attr("param");
            is_timer_trigger = true;

            float timer_period = 999f;
            switch (trigger)
            {
                case "Every 0.1 sec": timer_period = 0.1f; break;
                case "Every 1 sec": timer_period = 1f; break;
                case "Every 5 sec": timer_period = 5f; break;
                case "Every 15 sec": timer_period = 15f; break;
                case "Every 1 minute": timer_period = 60f; break;
                default: is_timer_trigger = false; break;
            }
            timer = new CustomTimer(timer_period); 
            

            Array enumConditions = Enum.GetValues(typeof(ConditionEnum));
            list_conditions = new List<ConditionEnum>();
            foreach (ConditionEnum cond in enumConditions)
                if (data.Bool(cond.ToString(), false))
                    list_conditions.Add(cond);
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = scene as Level;
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            if (trigger == "OnPlayerEnter")
                CheckAction();
        }

        public override void OnLeave(Player player)
        {
            base.OnLeave(player);
            if (trigger == "OnPlayerLeave")
                CheckAction();
        }

        public override void Update()
        {
            base.Update();
            if (is_activated && coroutine != null && coroutine.Finished)
            {
                coroutine = null;
                is_activated = false;
            }

            if (!is_activated && is_timer_trigger && timer.Tick())
                CheckAction();
        }

        public bool CheckAction()
        {
            foreach (ConditionEnum cond in list_conditions)
                if (!CheckCondition(cond))
                    return false;

            Action();
            return true;
        }

        public bool CheckCondition(ConditionEnum cond)
        {
            switch (cond)
            {
                case ConditionEnum.PlayerInside:
                    return level.Entities.FindAll<Player>().Exists(t => t.CollideRect(rect));

                case ConditionEnum.PlayerOnLeft:
                    return level.Entities.FindAll<Player>().Exists(t => t.Center.X < this.Center.X);

                case ConditionEnum.PlayerOnRight:
                    return level.Entities.FindAll<Player>().Exists(t => t.Center.X > this.Center.X);

                case ConditionEnum.BerryCollected:
                    return level.Entities.FindAll<Strawberry>().Count == 0;
            }

            return true;
        }

        void Action()
        {
            Player player = level.Tracker.GetEntity<Player>();
            coroutine = null;
            is_activated = true;
            switch (action)
            {
                case "Teleport": level.Add(new CutsceneRoomTeleport(param, Vector2.Zero)); break;
                case "Dialogue": if (Methods.PlayerIsAlive(player)) level.Add(new DialogCutscene(param, player, false)); break;
            }

            if (coroutine == null)
                is_activated = false;
            if (is_one_use)
                RemoveSelf();
        }
    }
}
