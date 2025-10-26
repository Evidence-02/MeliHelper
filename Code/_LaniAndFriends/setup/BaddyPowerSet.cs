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
    [CustomEntity("MeliHelper/BaddyPowerSet")]
    class BaddyPowerSet : Entity
    {
        BadelinePowerParams _params;
        EntityID id;
        bool is_load_once;

        public BaddyPowerSet(EntityData data, Vector2 offset, EntityID id)
        {
            this.id = id;
            is_load_once = data.Bool("loadOnce", true);
            _params = BaddyController.GetHookParamsFromData(data);
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            BaddyController.SetPower(scene as Level, _params);
            if (is_load_once)
                (scene as Level).Session.DoNotLoad.Add(id);
            RemoveSelf();
        }
    }
}
