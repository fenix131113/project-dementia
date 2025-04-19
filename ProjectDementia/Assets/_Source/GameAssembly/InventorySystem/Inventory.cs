using System;
using System.Collections.Generic;
using System.Linq;
using ItemsSystem;

namespace InventorySystem
{
    public class Inventory
    {
        public IReadOnlyList<InventoryItem> Items => _items.AsReadOnly();
        private readonly List<InventoryItem> _items = new();

        public event Action<InventoryItem> OnItemAdded;
        public event Action<InventoryItem> OnItemRemoved;

        public void AddItem(Item item, List<string> customData = null)
        {
            var invItem = new InventoryItem(item, customData);
            _items.Add(invItem);
            OnItemAdded?.Invoke(invItem);
        }

        public bool TryRemoveItem(Item item)
        {
            if (!IsItemInInventory(item))
                return false;

            var toRemove = _items.First(x => x.Item == item);
            _items.Remove(toRemove);
            OnItemRemoved?.Invoke(toRemove);
            return true;
        }

        public bool IsItemInInventory(Item item) => _items.FirstOrDefault(x => x.Item == item) != null;
    }
}