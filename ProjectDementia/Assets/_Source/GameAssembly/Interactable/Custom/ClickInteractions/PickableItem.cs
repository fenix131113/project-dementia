using System;
using System.Collections.Generic;
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
    public class PickableItem : AInteractableObject
    {
        [SerializeField] private ItemSO item;

        public List<string> customItemData;

        public override event Action OnInteract;

        private PlayersInventory _playersInventory;
        private ItemsContainer _itemsContainer;

        [Inject]
        private void Construct(PlayersInventory playersInventory, ItemsContainer itemsContainer)
        {
            _playersInventory = playersInventory;
            _itemsContainer = itemsContainer;
        }

        /// <summary>
        /// Replace custom data with given
        /// </summary>
        public void InitCustomData(List<string> customData) =>
            PhotonView.RPC(nameof(InitCustomData_RPC), RpcTarget.All, customData);

        [PunRPC]
        private void InitCustomData_RPC(List<string> customData) => customItemData = customData;

        public override void Interact()
        {
            base.Interact();
            PhotonView.RPC(nameof(TakeItem), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
        }

        [PunRPC]
        private void TakeItem(int actorNumber)
        {
            _playersInventory.GetPlayerInventory(SemiFunc.GetPlayerByActorNumber(actorNumber))
                .AddItem(_itemsContainer.GetItemBySO(item), customItemData);
            
            gameObject.SetActive(false);
            OnInteract?.Invoke();
        }
    }
}