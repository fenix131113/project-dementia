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
        [SerializeField] private List<string> customItemData = new();

        public IReadOnlyCollection<string> CustomData => customItemData;

        public override event Action OnInteract;

        private PlayersInventory _playersInventory;
        private ItemsContainer _itemsContainer;

        [Inject]
        private void Construct(PlayersInventory playersInventory, ItemsContainer itemsContainer)
        {
            _playersInventory = playersInventory;
            _itemsContainer = itemsContainer;
        }

        private void OnDestroy() => OnInteract = null;

        /// <summary>
        /// Replace custom data with given
        /// </summary>
        public void InitCustomData(List<string> customData)
        {
            if (customData == null)
                return;

            PhotonView.RPC(nameof(InitCustomData_RPC), RpcTarget.All, new object[] { customData.ToArray() });
        }

        public void AddCustomDataTag(string dataTag) // TODO: Replace with single logic (interface, class or something)
        {
            if (CustomData.Contains(dataTag))
                return;

            customItemData.Add(dataTag);
        }

        public bool TryRemoveDataTag(string dataTag)
        {
            if (!CustomData.Contains(dataTag))
                return false;

            customItemData.Remove(dataTag);
            return true;
        }

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
            _playersInventory.GetPlayerInventory(SemiFunc.GetPlayerByActorNumber(actorNumber))
                .AddItem(_itemsContainer.GetItemBySO(Item), customItemData);

            OnInteract?.Invoke();
            Destroy(gameObject);
        }
    }
}