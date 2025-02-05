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
        public Item SelectedItem { get; private set; }
        public int SelectedItemInventoryIndex { get; private set; }
        public InventorySelectionChangeSide LastChangeSide { get; private set; }

        private readonly Inventory _inventory;

        public event Action OnBeforeSelectedItemChanged;
        public event Action OnSelectedItemChanged;
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
            OnSelectedItemChanged?.Invoke();
        }

        public void SelectItemPrevious()
        {
            if (_inventory.Items.Count <= 1)
                return;

            OnBeforeSelectedItemChanged?.Invoke();

            SelectedItemInventoryIndex = (SelectedItemInventoryIndex + 1) % _inventory.Items.Count;

            SelectedItem = _inventory.Items[SelectedItemInventoryIndex];

            LastChangeSide = InventorySelectionChangeSide.LEFT;
            OnSelectedItemChanged?.Invoke();
        }

        private void OnItemRemovedFromInventory(Item item)
        {
            if (_inventory.Items.Count > 0 && SelectedItemInventoryIndex > _inventory.Items.Count - 1)
            {
                SelectedItemInventoryIndex = _inventory.Items.Count - 1;
                SelectedItem = _inventory.Items[SelectedItemInventoryIndex];
            }
            else
            {
                SelectedItem = null;
                SelectedItemInventoryIndex = -1;
                OnSelectedItemChanged?.Invoke();
            }
        }

        private void OnItemAddedFromInventory(Item item)
        {
            if (SelectedItem == null)
            {
                SelectedItem = item;
                SelectedItemInventoryIndex = 0;
                OnSelectedItemFirstTimeAdded?.Invoke();
            }
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