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

namespace Interactable.Custom.ClickInteractions
{
    public class ItemObjectPlaceZone : AInteractableObject
    {
        [field: SerializeField] public PickableItem SpawnedItem { get; private set; }

        [SerializeField] private Collider touchCollider;
        [SerializeField] private bool anyObjectAccept = true;
        [SerializeField] private ItemSO[] allowedItems;
        [SerializeField] private PlaceZoneObjectOffset[] placeZoneObjectOffsets;

        [Inject] private ItemsContainer _itemsContainer;
        [Inject] private PlayersInventory _inventories;
        [Inject] private ItemSelector _itemSelector;

        private InventoryItem _currentInventoryItem;

        public override event Action OnInteract;
        public event Action<PickableItem> OnObjectPlaced;

        private void Start()
        {
            if (!SpawnedItem)
                return;

            touchCollider.enabled = false;
            SpawnedItem.OnInteract += OnObjectTaken;
        }

        public override void Interact()
        {
            base.Interact();
            var handsItem = _itemSelector.SelectedItem;

            if (!anyObjectAccept &&
                !allowedItems.Contains(_itemsContainer.GetSOByID(_itemSelector.SelectedItem.Item.ID)))
                return;

            PlaceObject(PhotonNetwork.LocalPlayer.ActorNumber,
                _inventories.GetPlayerInventory(SemiFunc.GetPlayerByActorNumber(PhotonNetwork.LocalPlayer.ActorNumber))
                    .Items.ToList().IndexOf(handsItem));
        }

        public void PlaceObject(int playerActorNumber, int inventoryID)
        {
            if (SpawnedItem || _itemSelector.SelectedItem == null)
                return;

            PhotonView.RPC(nameof(PlaceObject_RPC), RpcTarget.All, playerActorNumber, inventoryID);
        }

        [PunRPC]
        private void PlaceObject_RPC(int playerActorNumber, int inventoryID)
        {
            var inventory = _inventories.GetPlayerInventory(SemiFunc.GetPlayerByActorNumber(playerActorNumber));
            _currentInventoryItem = inventory.Items[inventoryID];

            inventory.TryRemoveItem(_currentInventoryItem);

            touchCollider.enabled = false;
            SpawnObject();
        }

        [PunRPC]
        private void InitSpawnedObject_RPC(int viewID)
        {
            var spawned = PhotonView.Find(viewID).GetComponent<PickableItem>();

            SpawnedItem = spawned;
            SemiFunc.InjectObject(spawned.gameObject);

            spawned.InitCustomData(_currentInventoryItem.CustomData);
            spawned.GetComponent<AInteractableObject>().OnInteract += OnObjectTaken;
            OnObjectPlaced?.Invoke(spawned);
        }

        // Call on both clients... But complete only on master to prevent spawning second object
        private void SpawnObject()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            var spawnPos = transform.position;
            var spawnRot = Quaternion.identity;

            if (placeZoneObjectOffsets.Any(x =>
                    x.Items.Any(y => _itemsContainer.GetItemBySO(y).ID == _currentInventoryItem.Item.ID)))
            {
                var offsetItem = placeZoneObjectOffsets.First(x =>
                    x.Items.First(y => _itemsContainer.GetItemBySO(y).ID == _currentInventoryItem.Item.ID));

                spawnPos = transform.position + offsetItem.PositionOffset;
                spawnRot = Quaternion.Euler(offsetItem.Rotation);
            }

            var soItem = _itemsContainer.GetSOByID(_currentInventoryItem.Item.ID);
            var spawned = PhotonNetwork.Instantiate(
                $"{FoldersPaths.PICKABLE_PREFABS_PATH}{soItem.AdditionalFolderPath}/{soItem.Prefab.name}",
                spawnPos,
                spawnRot).GetComponent<PhotonView>();

            PhotonView.RPC(nameof(InitSpawnedObject_RPC), RpcTarget.All, spawned.ViewID);
        }

        private void OnObjectTaken()
        {
            SpawnedItem = null;
            touchCollider.enabled = true;
        }

        [Serializable]
        public class PlaceZoneObjectOffset
        {
            [field: SerializeField] public List<ItemSO> Items { get; private set; }
            [field: SerializeField] public Vector3 PositionOffset { get; private set; }
            [field: SerializeField] public Vector3 Rotation { get; private set; }
        }
    }
}