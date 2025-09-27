using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.MeliHelper._BattleCity
{
    class FieldItemComponent : Component
    {
        List<Item> list_items;

        public FieldItemComponent() : base(true, true)
        {
            list_items = new List<Item>();
        }

        public void AddItem(Item item)
        {
            // Add new item as first in the list and move the other ones
            Entity.SceneAs<Level>().Add(item);
            if (list_items.Count == 0) list_items.Add(item);
            else list_items.Insert(0, item);
            for (int i = 0; i < list_items.Count; i++)
                list_items[i].SetPosition(i);
        }

        public bool TryDisconnect()
        {
            if (list_items.Count == 0)
                return false;

            // Shoot the last one
            list_items[list_items.Count - 1].Disconnect();
            list_items.RemoveAt(list_items.Count - 1);
            return true;
        }

        public void Clear()
        {
            foreach (var item in list_items)
                item.RemoveSelf();
            list_items.Clear();
        }
    }


}
