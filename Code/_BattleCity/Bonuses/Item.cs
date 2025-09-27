using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    class Item : Entity
    {
        Level level;
        protected Field field;
        Player player;
        protected Vector2 dir;
        float dist;
        int id;
        protected bool is_connected;

        public Item(Field field, Player player) : base()
        {
            this.field = field;
            this.player = player;
            this.is_connected = true;
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            level = scene as Level;
        }

        public override void Update()
        {
            base.Update();
            if (is_connected)
            {
                dir = (Input.MenuUp.Check ? new Vector2(0, -1) : player.Facing == Facings.Left ? new Vector2(-1, 0) : new Vector2(1, 0));
                Position = player.Center + dist * dir;
                //if (MeliHelperModule.Settings.BC_Shoot.Check)
                //    Shoot();
            }
        }

        public void SetPosition(int id)
        {
            this.id = id;
            this.dist = 16 + 8 * id;
        }

        public virtual void Disconnect()
        {
            is_connected = false;
        }

        public Player GetParent
        {
            get
            {
                return player;
            }
        }
    }
}
