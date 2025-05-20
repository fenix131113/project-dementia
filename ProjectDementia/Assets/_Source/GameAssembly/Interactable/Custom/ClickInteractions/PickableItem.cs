using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Interactable.Base;
using InventorySystem;
using ItemsSystem;
using ItemsSystem.Data;
using Photon.Pun;
using UnityEngine;
using VContainer;

// ReSharper disable CoVariantArrayConversion

namespace Interactable.Custom.ClickInteractions
{
    public class PickableItem : AInteractableObject
    {
        [field: SerializeField] public ItemSO Item { get; private set; }

        public List<string> customItemData;

        public override event Action OnInteract;

        private PlayersInventory _playersInventory;
        private ItemsContainer _itemsContainer;

        [Inject]
        private void Construct(PlayersInventory playersInventory, ItemsContainer itemsContainer)
        {
            _playersInventory = playersInventory;
            _itemsContainer = itemsContainer;
            Debug.Log($"inventories {playersInventory != null}");
            Debug.Log($"container {itemsContainer != null}");
        }

        /// <summary>
        /// Replace custom data with given
        /// </summary>
        public void InitCustomData(List<string> customData) =>
            PhotonView.RPC(nameof(InitCustomData_RPC), RpcTarget.All, customData.ToArray());

        [PunRPC]
        private void InitCustomData_RPC(string[] customData) => customItemData = customData.ToList();

        public override void Interact()
        {
            base.Interact();
            PhotonView.RPC(nameof(TakeItem), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
        }

        [PunRPC]
        private void TakeItem(int actorNumber)
        {
            Debug.Log($"Players Inventory: {_playersInventory != null}");
            Debug.Log($"Items Container: {_itemsContainer != null}");
            Debug.Log($"Item: {Item != null}");
            Debug.Log($"Custrom data: {customItemData != null}");
            
            _playersInventory.GetPlayerInventory(SemiFunc.GetPlayerByActorNumber(actorNumber))
                .AddItem(_itemsContainer.GetItemBySO(Item), customItemData);
            
            OnInteract?.Invoke();
            Destroy(gameObject);
        }
    }
}