using System.Collections;
using Core;
using Interactable.Custom.ClickInteractions;
using InventorySystem;
using ItemsSystem;
using Photon.Pun;
using UnityEngine;
using VContainer;

namespace Levels._4
{
    public class Centrifuge : MonoBehaviourPun // TODO: Combine chemistry stuff together
    {
        [SerializeField] private SelectedItemChecker selectedItemChecker;
        [SerializeField] private Transform bottlePoint;
        [SerializeField] private Collider centrifugeCollider;
        [SerializeField] private float completeTime;
        [SerializeField] private string completeTag;

        [Inject] private PlayersInventory _inventories;
        [Inject] private ItemsContainer _itemsContainer;
        
        private PickableItem _spawnedPickable;
        private InventoryItem _currentInventoryItem;

        private void Start() => Bind();

        private void OnDestroy() => Expose();

        private void OnItemTaken()
        {
            centrifugeCollider.enabled = true;
            _currentInventoryItem = null;
        }
        
        [PunRPC]
        private void OnComplete()
        {
            _spawnedPickable.AddCustomDataTag(completeTag);
            _spawnedPickable.OnInteract += OnItemTaken; // Expose in PickableItem
            _spawnedPickable.SetInteractable(true);
        }
        
        [PunRPC]
        private void InitSpawnedObject(int viewID)
        {
            var spawned = PhotonView.Find(viewID).GetComponent<PickableItem>();

            _spawnedPickable = spawned;
            SemiFunc.InjectObject(spawned.gameObject);
            spawned.InitCustomData(_currentInventoryItem.CustomData);
            _spawnedPickable.SetInteractable(false);
        }
        
        private void OnItemAccepted(InventoryItem item, int inventoryId)
        {
            centrifugeCollider.enabled = false;
            _currentInventoryItem = item;

            var soItem = _itemsContainer.GetSOByID(item.Item.ID);

            if (PhotonNetwork.IsMasterClient)
            {
                var spawned = PhotonNetwork.Instantiate(
                    $"{FoldersPaths.PICKABLE_PREFABS_PATH}{soItem.AdditionalFolderPath}/{soItem.Prefab.name}",
                    bottlePoint.position, bottlePoint.rotation);
                photonView.RPC(nameof(InitSpawnedObject), RpcTarget.All, spawned.GetComponent<PhotonView>().ViewID);

                StartCoroutine(CompleteCoroutine());
            }

            _inventories.GetPlayerInventory(SemiFunc.GetPlayerByActorNumber(inventoryId)).TryRemoveItem(item);
        }

        private void Bind() => selectedItemChecker.OnItemAccepted += OnItemAccepted;

        private void Expose() => selectedItemChecker.OnItemAccepted -= OnItemAccepted;

        private IEnumerator CompleteCoroutine()
        {
            yield return new WaitForSeconds(completeTime);
            
            photonView.RPC(nameof(OnComplete), RpcTarget.All);
        }
    }
}