using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    class EventUI : Entity
    {
        List<EventBC> list_events;
        bool[] list_delayed_stars;
        MTexture texture_life, texture_star, texture_water;
        CustomTimer timer_events;
        string type_ui, debug1, debug2, debug3;

        public EventUI(string type_ui)
        {
            list_events = new List<EventBC>();
            texture_life = GFX.Gui["Evidence02/bc/uiLife"];
            texture_star = GFX.Gui["Evidence02/bc/uiStar"];
            texture_water = GFX.Gui["Evidence02/bc/uiWater"];
            list_delayed_stars = new bool[5];
            this.type_ui = type_ui;
            this.timer_events = new CustomTimer(1f);

            Depth = DepthController.DEFAULT_UI;
            Tag = Tags.TransitionUpdate | TagsExt.SubHUD;
        }

        public override void Update()
        {
            base.Update();
            if (timer_events.Tick())
            {
                foreach (var item in list_events)
                    item.GetSeconds--;
                
                for (int i = 0; i < list_events.Count; i++)
                    if (list_events[i].isDead)
                    {
                        list_events[i].Clear();
                        list_events.RemoveAt(i);
                    }
            }
            
            //foreach (var item in list_events)
            //    item.Update();

        }

        public override void Render()
        {
            base.Render();
            int center_x = 265;
            int y = 160;

            BattleCityPlayerInfo info = ProgressController.GetCurrentPlayerInfo();

            // 1 Lifes
            if (info.Lifes > 3)
            {
                texture_life.DrawCentered(new Vector2(center_x - 60, y), Color.White);
                FontControllerNES.ShowTextNES("X", new Vector2(center_x, y - FontControllerNES.GetTextHeight() / 2), Color.White, TextAlignment.Center);
                FontControllerNES.ShowTextNES(info.Lifes.ToString(), new Vector2(center_x + 30, y - FontControllerNES.GetTextHeight() / 2), Color.White, TextAlignment.Left);
            }
            else
            {
                for (int i = 0; i < 3; i++)
                    texture_life.DrawCentered(new Vector2(center_x + 60 * (i - 1), y), Color.White * (i < info.Lifes ? 1 : 0.3f));
            }

            // 2 Stars
            //y = 225;    // bruh
            int count_stars = ProgressController.GetPlayerPower();
            for (int i = 0; i < 5; i++)
                texture_star.DrawCentered(GetStarPosition(i), Color.White * (i < count_stars && !list_delayed_stars[i] ? 1f : 0.3f));

            // 2.1 Move through water
            if (ProgressController.PlayerCanMoveThroughWater())
                texture_water.DrawCentered(new Vector2(center_x + 132, y), Color.White);



            // 3 Events
            y = 300;
            for (int i = 0; i < list_events.Count; i++)
                list_events[i].Render(center_x, y + 60 * i);


            // 4 Points
            y = Math.Max(510, 300 + 60 * list_events.Count + 15);
            int x_points = center_x + FontControllerNES.GetTextWidth("POINTS") / 2;
            FontControllerNES.ShowTextNES("POINTS", new Vector2(x_points, y), Color.White, TextAlignment.Right);
            FontControllerNES.ShowTextNES(info.Points.ToString(), new Vector2(x_points, y + 12 + FontControllerNES.GetTextHeight()), Color.White, TextAlignment.Right);
            
            // 5 Tanks (right side)
            if (Field.Instance != null && Field.Instance.GetEnemiesComponent != null)
                Field.Instance.GetEnemiesComponent.RenderUI();



            /*
            if (MeliHelperModule.Settings.Debug_ShowPlayerInfo)
            {
                BattleCityPlayerInfo pinfo;
                pinfo = MeliHelperModule.Instance.Session.BattleCity_PlayerInfo;
                string s1 = string.Format("pinfo: points {0}, stars {1}, lifes, {2}, water {3}", pinfo.Points, pinfo.Stars, pinfo.Lifes, pinfo.MoveThroughWater);
                pinfo = MeliHelperModule.Instance.Session.BattleCity_PlayerInfoSaved;
                string s2 = string.Format("saved: points {0}, stars {1}, lifes, {2}, water {3}", pinfo.Points, pinfo.Stars, pinfo.Lifes, pinfo.MoveThroughWater);
                //pinfo = MeliHelperModule.Instance.Session.BattleCity_PlayerStandalone;
                //string s3 = string.Format("stand: points {0}, stars {1}, lifes, {2}, water {3}", pinfo.Points, pinfo.Stars, pinfo.Lifes, pinfo.MoveThroughWater);
                ActiveFont.Draw(s1, new Vector2(10, 630), Color.White);
                ActiveFont.Draw(s2, new Vector2(10, 690), Color.White);
                //ActiveFont.Draw(s3, new Vector2(10, 750), Color.White);
            }
            */



            // Debug stuff
            if (debug1 != "") ActiveFont.Draw(debug1, new Vector2(10, 810), Color.Red);
            if (debug2 != "") ActiveFont.Draw(debug2, new Vector2(10, 900), Color.White);
            if (debug3 != "") ActiveFont.Draw(debug3, new Vector2(10, 990), Color.White);
        }

        public Vector2 GetStarPosition(int i)
        {
            return new Vector2(265 + 40 * (i - 2), 225);
        }

        public void StarSetDelay(int id, bool value)
        {
            list_delayed_stars[id] = value;
        }


        public bool isEventExists(BCEnum_BonusEvent type)
        {
            return list_events.Exists(t => t.GetEventType == type);
        }

        public void AddEvent(Player player, BCEnum_BonusEvent type)
        {
            int sec = 15;
            switch (type)
            {
                case BCEnum_BonusEvent.Shield:   sec = 25; break; // 20
                case BCEnum_BonusEvent.Shovel:   sec = 30; break;
                case BCEnum_BonusEvent.TimeStop: sec = 15; break;
                case BCEnum_BonusEvent.EMI:      sec = 20; break;
                case BCEnum_BonusEvent.Duality:              sec = 30; break;
                case BCEnum_BonusEvent.HomingBullets:        sec = 25; break;
                case BCEnum_BonusEvent.UnlimitedShooting:    sec = 15; break;
            }

            EventBC ev = list_events.FirstOrDefault(t => t.GetEventType == type);
            if (ev != null)
            {
                ev.GetSeconds += sec;
                ev.TtlUpdated();
            }
            else
            {
                switch (type)
                {
                    case BCEnum_BonusEvent.Shield: ev = new EventShield(player, sec); break;
                    case BCEnum_BonusEvent.Shovel: ev = new EventShovel(sec); break;
                    case BCEnum_BonusEvent.TimeStop: ev = new EventTimeStop(sec); break; 
                    case BCEnum_BonusEvent.EMI: ev = new EventEMI(player, sec); break;
                    default: ev = new EventBC(type, sec); break;
                }
                if (ev != null)
                {
                    Add(ev);
                    list_events.Add(ev);
                }
            }

            list_events = list_events.OrderByDescending(t => t.GetSeconds).ToList();
        }

        public void Clear()
        {
            for (int i = 0; i < list_events.Count; i++)
                list_events[i].Clear();
            list_events.Clear();
        }


        
        public void SetError(string err)
        {
            debug1 += err + "; ";
        }

        public void SetInfo(string err)
        {
            debug2 = err;
        }

        public void SetInfo2(string err)
        {
            debug3 = err;
        }

    }
}
