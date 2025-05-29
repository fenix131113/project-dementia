using Core;
using Interactable.Custom.ClickInteractions;
using InventorySystem;
using ItemsSystem;
using ItemsSystem.Data;
using UnityEngine;
using VContainer;

namespace Levels._4
{
    public class SelectedItemChanger : MonoBehaviour
    {
        [SerializeField] private SelectedItemChecker selectedItemChecker;
        [SerializeField] private ItemSO returnItem;

        [Inject] private PlayersInventory _inventories;
        [Inject] private ItemsContainer _itemsContainer;
        [Inject] private ItemSelector _itemSelector;

        private void Start() => Bind();

        private void OnDestroy() => Expose();

        // Execute on both clients
        private void SelectedItemCheckerOnOnItemAccepted(InventoryItem item, int inventoryIndex)
        {
            var inv = _inventories.GetPlayerInventory(SemiFunc.GetPlayerByActorNumber(inventoryIndex));
            inv.TryRemoveItem(item);
            inv.AddItem(new InventoryItem(_itemsContainer.GetItemBySO(returnItem), null));
        }

        private void Bind() => selectedItemChecker.OnItemAccepted += SelectedItemCheckerOnOnItemAccepted;

        private void Expose() => selectedItemChecker.OnItemAccepted -= SelectedItemCheckerOnOnItemAccepted;
    }
}