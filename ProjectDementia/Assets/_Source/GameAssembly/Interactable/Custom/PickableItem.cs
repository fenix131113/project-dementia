﻿using System;
using Interactable.Base;
using InventorySystem;
using ItemsSystem;
using ItemsSystem.Data;
using Photon.Pun;
using UnityEngine;
using VContainer;

namespace Interactable.Custom
{
    public class PickableItem : AInteractableObject
    {
        [SerializeField] private ItemSO item;

        public override event Action OnInteract;

        private PlayersInventory _playersInventory;
        private ItemsContainer _itemsContainer;

        [Inject]
        private void Construct(PlayersInventory playersInventory, ItemsContainer itemsContainer)
        {
            _playersInventory = playersInventory;
            _itemsContainer = itemsContainer;
        }
        
        public override void Interact() => PhotonView.RPC(nameof(TakeItem), RpcTarget.AllBuffered);
        
        [PunRPC]
        private void TakeItem()
        {
            _playersInventory.GetPlayerInventory(PhotonNetwork.LocalPlayer).AddItem(_itemsContainer.GetItemBySO(item));
            OnInteract?.Invoke();
            gameObject.SetActive(false);
        }
    }
}