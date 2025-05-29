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
    public class Stove : MonoBehaviourPun // TODO: Combine chemistry stuff together (with abstract class)
    {
        [SerializeField] private SelectedItemChecker selectedItemChecker;
        [SerializeField] private Collider itemCheckerCollider;
        [SerializeField] private Transform bottleSpawnPivot;
        [SerializeField] private float cookTime;
        [SerializeField] private string resultTag;

        [Inject] private PlayersInventory _inventories;
        [Inject] private ItemsContainer _itemsContainer;

        private PickableItem _spawnedPickable;
        private InventoryItem _currentInventoryItem;

        private void Start() => Bind();

        private void OnDestroy() => Expose();

        private void OnItemAccepted(InventoryItem item, int inventoryId) // Calls on both clients
        {
            itemCheckerCollider.enabled = false;
            _currentInventoryItem = item;

            var soItem = _itemsContainer.GetSOByID(item.Item.ID);

            if (PhotonNetwork.IsMasterClient)
            {
                var spawned = PhotonNetwork.Instantiate(
                    $"{FoldersPaths.PICKABLE_PREFABS_PATH}{soItem.AdditionalFolderPath}/{soItem.Prefab.name}",
                    bottleSpawnPivot.position, Quaternion.identity);
                photonView.RPC(nameof(InitSpawnedObject), RpcTarget.All, spawned.GetComponent<PhotonView>().ViewID);

                StartCoroutine(CookCoroutine());
            }

            _inventories.GetPlayerInventory(SemiFunc.GetPlayerByActorNumber(inventoryId)).TryRemoveItem(item);
        }

        private void OnItemTaken()
        {
            itemCheckerCollider.enabled = true;
            _currentInventoryItem = null;
        }

        [PunRPC]
        private void CompleteCook()
        {
            _spawnedPickable.AddCustomDataTag(resultTag);
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

        private void Bind() => selectedItemChecker.OnItemAccepted += OnItemAccepted;

        private void Expose() => selectedItemChecker.OnItemAccepted -= OnItemAccepted;

        private IEnumerator CookCoroutine() // Calls only on master
        {
            yield return new WaitForSeconds(cookTime);
            
            photonView.RPC(nameof(CompleteCook), RpcTarget.All);
        }
    }
}