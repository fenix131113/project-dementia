using InventorySystem;
using ItemsSystem;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core.Initializing
{
    public class LevelBootstrapper : IStartable
    {
        private readonly ItemsContainer _itemsContainer;
        private readonly PlayersInventory _playersInventory;
        
        [Inject]
        public LevelBootstrapper(ItemsContainer itemsContainer, PlayersInventory playersInventory)
        {
            _itemsContainer = itemsContainer;
            _playersInventory = playersInventory;
        }
        
        public void Start()
        {
            Debug.Log("Start");
        }
    }
}