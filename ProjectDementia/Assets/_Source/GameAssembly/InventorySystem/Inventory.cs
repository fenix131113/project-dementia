using System;
using System.Collections.Generic;
using ItemsSystem;

namespace InventorySystem
{
    public class Inventory
    {
        public IReadOnlyList<Item> Items => _items.AsReadOnly();
        private readonly List<Item> _items = new();

        public event Action<Item> OnItemAdded;
        public event Action<Item> OnItemRemoved;

        public void AddItem(Item item)
        {
            _items.Add(item);
            OnItemAdded?.Invoke(item);
        }

        public bool TryRemoveItem(Item item)
        {
            if (!_items.Contains(item))
                return false;

            _items.Remove(item);
            OnItemRemoved?.Invoke(item);
            return true;
        }
    }
}