using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    class EventBC : Component
    {
        protected MTexture texture;
        protected BCEnum_BonusEvent type;
        protected int seconds;

        public EventBC(BCEnum_BonusEvent type, int seconds) : base(true, true)
        {
            this.type = type;
            this.seconds = seconds;
            this.texture = GFX.Gui[@"Evidence02/bc/event" + type];
        }

        public void Render(int x, int y)
        {
            texture.DrawCentered(new Vector2(x - 35, y));
            FontController.ShowTextNES(seconds.ToString(), new Vector2(x + 8, y - 0.75f * FontController.GetTextHeight() / 2), 
                Color.White, TextAlignment.Left, koef_size: 0.75f);
        }

        public virtual void Clear()
        {
            RemoveSelf();
        }

        public virtual void TtlUpdated()
        {

        }

        public BCEnum_BonusEvent GetEventType
        {
            get
            {
                return type;
            }
        }

        public int GetSeconds
        {
            get
            {
                return seconds;
            }
            set
            {
                seconds = value;
            }
        }

        public bool isDead
        {
            get
            {
                return seconds < 0;
            }
        }

    }

}
