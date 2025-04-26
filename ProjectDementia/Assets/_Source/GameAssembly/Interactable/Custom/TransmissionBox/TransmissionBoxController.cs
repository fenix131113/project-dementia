using System;
using Core;
using Interactable.Custom.ClickInteractions;
using InventorySystem;
using Photon.Pun;
using UnityEngine;
using VContainer;

namespace Interactable.Custom.TransmissionBox
{
    public class TransmissionBoxController : MonoBehaviourPun
    {
        [SerializeField] private SelectedItemChecker firstChecker;
        [SerializeField] private SelectedItemChecker secondChecker;

        private SelectedItemChecker _availableChecker;

        public InventoryItem CurrentItem { get; private set; }

        private PlayersInventory _inventory;
        private bool _isTakingItem;

        public event Action OnItemChanged;

        [Inject]
        private void Construct(PlayersInventory inventory)
        {
            _inventory = inventory;
            _availableChecker = firstChecker;

            secondChecker.SetInteractable(false);
        }

        private void Start() => Bind();

        private void OnDestroy() => Expose();

        // Called when player takes item from non-empty box (Local method)
        private void OnInteract()
        {
            if (CurrentItem == null)
                return;

            photonView.RPC(nameof(GiveItemToPlayer), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
        }

        [PunRPC]
        private void GiveItemToPlayer(int actorNumber)
        {
            _isTakingItem = true;
            firstChecker.SetCheckAbility(true);
            secondChecker.SetCheckAbility(true);

            _inventory.GetPlayerInventory(SemiFunc.GetPlayerByActorNumber(actorNumber)).AddItem(CurrentItem);

            CurrentItem = null;

            OnItemChanged?.Invoke();
        }

        // Works when player put item in empty box (Network method)
        private void OnItemAccepted(InventoryItem inventoryItem, int interactId)
        {
            if (CurrentItem != null)
                return;

            if (_isTakingItem)
            {
                _isTakingItem = false;
                return;
            }

            _availableChecker.SetInteractable(false);
            _availableChecker = _availableChecker == firstChecker ? secondChecker : firstChecker;
            _availableChecker.SetInteractable(true);

            firstChecker.SetCheckAbility(false);
            secondChecker.SetCheckAbility(false);

            if (!_inventory.GetPlayerInventory(SemiFunc.GetPlayerByActorNumber(interactId))
                    .TryRemoveItem(inventoryItem))
                Debug.LogWarning($"Can't remove item from player {interactId}(ActorNumber) inventory!");
            else
                CurrentItem = inventoryItem;

            OnItemChanged?.Invoke();
        }

        private void Bind()
        {
            firstChecker.OnItemAccepted += OnItemAccepted;
            secondChecker.OnItemAccepted += OnItemAccepted;
            firstChecker.OnInteract += OnInteract;
            secondChecker.OnInteract += OnInteract;
        }

        private void Expose()
        {
            firstChecker.OnItemAccepted -= OnItemAccepted;
            secondChecker.OnItemAccepted -= OnItemAccepted;
            firstChecker.OnInteract -= OnInteract;
            secondChecker.OnInteract -= OnInteract;
        }
    }
}