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
    public class ItemObjectPlaceZone : AInteractableObject
    {
        [SerializeField] private PickableItem spawnedItem;
        [SerializeField] private Collider touchCollider;
        [SerializeField] private PlaceZoneObjectOffset[] placeZoneObjectOffsets;

        [Inject] private ItemsContainer _itemsContainer;
        [Inject] private PlayersInventory _inventories;
        [Inject] private ItemSelector _itemSelector;

        private InventoryItem _currentInventoryItem;

        public override event Action OnInteract;

        private void Start()
        {
            if (!spawnedItem)
                return;

            touchCollider.enabled = false;
            spawnedItem.OnInteract += OnObjectTaken;
        }

        public override void Interact()
        {
            base.Interact();
            var handsItem = _itemSelector.SelectedItem;
            PlaceObject(PhotonNetwork.LocalPlayer.ActorNumber,
                _inventories.GetPlayerInventory(SemiFunc.GetPlayerByActorNumber(PhotonNetwork.LocalPlayer.ActorNumber))
                    .Items.ToList().IndexOf(handsItem));
        }

        public void PlaceObject(int playerActorNumber, int inventoryID)
        {
            if (spawnedItem || _itemSelector.SelectedItem == null)
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

        private void SpawnObject()
        {
            var spawnPos = transform.position;
            var spawnRot = Quaternion.identity;

            if (placeZoneObjectOffsets.Any(x =>
                    _itemsContainer.GetItemBySO(x.Item).ID == _currentInventoryItem.Item.ID))
            {
                var offsetItem = placeZoneObjectOffsets.First(x =>
                    _itemsContainer.GetItemBySO(x.Item).ID == _currentInventoryItem.Item.ID);
                
                spawnPos = transform.position + offsetItem.PositionOffset;
                spawnRot = Quaternion.Euler(offsetItem.Rotation);
            }

            var spawned = PhotonNetwork.Instantiate(
                $"Prefabs/Items/{_itemsContainer.GetSOByID(_currentInventoryItem.Item.ID).Prefab.name}",
                spawnPos,
                spawnRot).GetComponent<PickableItem>();

            spawnedItem = spawned;
            SemiFunc.InjectObject(spawned.gameObject);

            spawned.InitCustomData(_currentInventoryItem.CustomData);
            spawned.GetComponent<AInteractableObject>().OnInteract += OnObjectTaken;
        }

        [PunRPC]
        private void TakeObject_RPC()
        {
            spawnedItem = null;
            touchCollider.enabled = true;

            if (spawnedItem)
                Destroy(spawnedItem.gameObject);
            else
                Debug.LogWarning("Empty item place!");
        }

        private void OnObjectTaken() => PhotonView.RPC(nameof(TakeObject_RPC), RpcTarget.All);

        [Serializable]
        public class PlaceZoneObjectOffset
        {
            [field: SerializeField] public ItemSO Item { get; private set; }
            [field: SerializeField] public Vector3 PositionOffset { get; private set; }
            [field: SerializeField] public Vector3 Rotation { get; private set; }
        }
    }
}