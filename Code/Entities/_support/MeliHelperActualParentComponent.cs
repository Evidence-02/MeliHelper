using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper
{
    class MeliHelperActualParentComponent : Component
    {
        Entity ActualParent { get; set; }
        
        public MeliHelperActualParentComponent(Entity parent) : base(false, false)
        {
            ActualParent = parent;
        }

        public static Entity GetActualParent(Entity entity)
        {
            IEnumerable<MeliHelperActualParentComponent> list_components = entity.Components.GetAll<MeliHelperActualParentComponent>();
            return (list_components.Count() == 0) ? null : list_components.First().ActualParent;
        }
    }
}
