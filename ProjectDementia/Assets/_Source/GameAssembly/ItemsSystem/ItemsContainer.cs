using System.Collections.Generic;
using System.Linq;
using ItemsSystem.Data;
using UnityEngine;
using Utils.ResourcesLoading;
using VContainer;

namespace ItemsSystem
{
    public class ItemsContainer
    {
        private readonly DataRepository<ScriptableObject> _itemsRepo;
        private readonly Dictionary<ItemSO, Item> _itemsPair = new();

        [Inject]
        public ItemsContainer(DataRepository<ScriptableObject> itemsRepo)
        {
            _itemsRepo = itemsRepo;
            
            GenerateItems();
        }

        public Item GetItemBySO(ItemSO itemSO) => _itemsPair[itemSO];

        public Item GetItemByID(int id) => _itemsPair.First(item => item.Value.ID == id).Value;

        public void GenerateItems()
        {
            _itemsPair.Clear();
            
            var itemID = 0;

            var items = _itemsRepo.GetItem<ItemSO>();
            foreach (var itemSO in items)
            {
                var createdItem = new Item(itemID, itemSO);
                _itemsPair.Add(itemSO, createdItem);
                itemID++;
            }
        }
    }
}