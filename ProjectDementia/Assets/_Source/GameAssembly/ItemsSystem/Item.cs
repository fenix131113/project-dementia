using ItemsSystem.Data;
using UnityEngine;

namespace ItemsSystem
{
    public class Item
    {
        public int ID { get; private set; }
        public string ItemName { get; private set; }
        public Sprite Icon { get; private set; }

        public Item(int id, ItemSO itemSO)
        {
            ID = id;
            ItemName = itemSO.ItemName;
            Icon = itemSO.Icon;
        }
    }
}