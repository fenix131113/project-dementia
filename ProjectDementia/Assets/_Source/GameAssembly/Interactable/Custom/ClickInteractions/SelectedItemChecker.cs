using System;
using System.Linq;
using Core;
using Interactable.Base;
using InventorySystem;
using ItemsSystem;
using ItemsSystem.Data;
using Photon.Pun;
using UnityEngine;
using VContainer;

namespace Interactable.Custom.ClickInteractions
{
    public class SelectedItemChecker : AInteractableObject //TODO: Add items custom data check
    {
        public bool CanCheckItem { get; private set; } = true;

        [SerializeField] private bool anyInteractItem = true;
        [SerializeField] public ItemSO[] interactItems;

        private PlayersInventory _inventory;
        private ItemsContainer _items;
        private ItemSelector _selector;

        public override event Action OnInteract; // Invokes locally
        public event Action<InventoryItem, int> OnItemAccepted; // Invokes with rpc (target: all)

        [Inject]
        private void Construct(PlayersInventory inventory, ItemsContainer items, ItemSelector selector)
        {
            _inventory = inventory;
            _selector = selector;
            _items = items;
        }

        public void SetCheckAbility_RPC(bool canCheck) =>
            PhotonView.RPC(nameof(SetCheckAbility), RpcTarget.All, canCheck);

        [PunRPC]
        public void SetCheckAbility(bool canCheck) => CanCheckItem = canCheck;

        public override void Interact()
        {
            base.Interact();
            
            OnInteract?.Invoke();

            if (!CanCheckItem || _selector.SelectedItem == null || (!anyInteractItem &&
                                                                    !interactItems.Select(x => _items.GetItemBySO(x))
                                                                        .Contains(_selector.SelectedItem.Item)))
                return;

            PhotonView.RPC(nameof(CheckItem_RPC), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber,
                _inventory.GetPlayerInventory(PhotonNetwork.LocalPlayer).Items.ToList()
                    .IndexOf(_selector.SelectedItem));
        }

        [PunRPC]
        private void CheckItem_RPC(int id, int inventoryItemIndex)
        {
            var interactInventory = _inventory.GetPlayerInventory(SemiFunc.GetPlayerByActorNumber(id));
            var itemInstance = interactInventory.Items[inventoryItemIndex];

            OnItemAccepted?.Invoke(itemInstance, id);
        }
    }
}