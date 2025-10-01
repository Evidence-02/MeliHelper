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
    [CustomEntity("MeliHelper/LaniHookSet")]
    class LaniHookSet : Entity
    {
        LaniHookParams hook_params;
        EntityID id;
        bool is_load_once;

        public LaniHookSet(EntityData data, Vector2 offset, EntityID id)
        {
            this.id = id;
            is_load_once = data.Bool("loadOnce", true);
            hook_params = LaniController.GetHookParamsFromData(data);
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            LaniController.SetHook(hook_params);
            if (is_load_once)
                (scene as Level).Session.DoNotLoad.Add(id);
            RemoveSelf();
        }
    }
}
