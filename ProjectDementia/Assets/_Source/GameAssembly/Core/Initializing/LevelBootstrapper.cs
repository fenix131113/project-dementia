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
        private readonly IObjectResolver _resolver;

        [Inject]
        public LevelBootstrapper(ItemsContainer itemsContainer, PlayersInventory playersInventory,
            IObjectResolver resolver)
        {
            _itemsContainer = itemsContainer;
            _playersInventory = playersInventory;
            _resolver = resolver;
        }

        public void Start()
        {
            SemiFunc.Init(_resolver);
        }
    }
}