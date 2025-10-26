using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._Baddy
{
    [CustomEntity("MeliHelper/BaddyPowerClear")]
    class BaddyPowerClear : Entity
    {
        EntityID id;
        bool is_load_once;

        public BaddyPowerClear(EntityData data, Vector2 offset, EntityID id)
        {
            this.id = id;
            is_load_once = data.Bool("loadOnce", true);
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            BaddyController.ClearPower();
            if (is_load_once)
                (scene as Level).Session.DoNotLoad.Add(id);
            RemoveSelf();
        }
    }
}
