using System;
using InventorySystem.Data;
using ItemsSystem;
using Photon.Pun;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace InventorySystem
{
    public class ItemSelector : IStartable
    {
        public InventoryItem SelectedItem { get; private set; }
        public int SelectedItemInventoryIndex { get; private set; }
        public InventorySelectionChangeSide LastChangeSide { get; private set; }

        private readonly Inventory _inventory;

        public event Action OnBeforeSelectedItemChanged;
        public event Action OnSelectedItemChangedScroll;
        public event Action OnSelectedItemChangedNative;
        public event Action OnSelectedItemFirstTimeAdded;

        [Inject]
        public ItemSelector(PlayersInventory playerInventory)
        {
            _inventory = playerInventory.GetPlayerInventory(PhotonNetwork.LocalPlayer);
        }

        ~ItemSelector() => Expose();

        public void Start() => Bind();

        public void SelectItemNext()
        {
            if (_inventory.Items.Count <= 1)
                return;

            OnBeforeSelectedItemChanged?.Invoke();

            SelectedItemInventoryIndex =
                (SelectedItemInventoryIndex - 1 + _inventory.Items.Count) % _inventory.Items.Count;

            SelectedItem = _inventory.Items[SelectedItemInventoryIndex];

            LastChangeSide = InventorySelectionChangeSide.RIGHT;
            OnSelectedItemChangedScroll?.Invoke();
        }

        public void SelectItemPrevious()
        {
            if (_inventory.Items.Count <= 1)
                return;

            OnBeforeSelectedItemChanged?.Invoke();

            SelectedItemInventoryIndex = (SelectedItemInventoryIndex + 1) % _inventory.Items.Count;

            SelectedItem = _inventory.Items[SelectedItemInventoryIndex];

            LastChangeSide = InventorySelectionChangeSide.LEFT;
            OnSelectedItemChangedScroll?.Invoke();
        }

        private void OnItemRemovedFromInventory(InventoryItem item)
        {
            if (_inventory.Items.Count > 0)
            {
                SelectedItemInventoryIndex = _inventory.Items.Count - 1;
                SelectedItem = _inventory.Items[SelectedItemInventoryIndex];
            }
            else
            {
                SelectedItem = null;
                SelectedItemInventoryIndex = -1;
            }

            OnSelectedItemChangedNative?.Invoke();
        }

        private void OnItemAddedFromInventory(InventoryItem item)
        {
            if (SelectedItem != null)
                return;
            
            SelectedItem = item;
            SelectedItemInventoryIndex = 0;
            OnSelectedItemFirstTimeAdded?.Invoke();
        }

        private void Bind()
        {
            _inventory.OnItemRemoved += OnItemRemovedFromInventory;
            _inventory.OnItemAdded += OnItemAddedFromInventory;
        }

        private void Expose()
        {
            _inventory.OnItemRemoved -= OnItemRemovedFromInventory;
            _inventory.OnItemAdded -= OnItemAddedFromInventory;
        }
    }
}