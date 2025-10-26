using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._Lani
{
    [CustomEntity("MeliHelper/LaniHookClear")]
    class LaniHookClear : Entity
    {
        EntityID id;
        bool is_load_once;

        public LaniHookClear(EntityData data, Vector2 offset, EntityID id)
        {
            this.id = id;
            is_load_once = data.Bool("loadOnce", true);
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            LaniController.ClearHook();
            if (is_load_once)
                (scene as Level).Session.DoNotLoad.Add(id);
            RemoveSelf();
        }
    }
}
