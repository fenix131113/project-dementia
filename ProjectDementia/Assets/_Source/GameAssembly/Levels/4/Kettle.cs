using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Interactable.Custom.ClickInteractions;
using ItemsSystem.Data;
using Photon.Pun;
using UnityEngine;

namespace Levels._4
{
    public class Kettle : MonoBehaviourPun
    {
        [SerializeField] private ItemObjectPlaceZone firstItemZone;
        [SerializeField] private ItemObjectPlaceZone secondItemZone;
        [SerializeField] private ItemSO trashReturnItem;
        [SerializeField] private ItemSO emptyReturnItem;
        [SerializeField] private Transform firstItemPivot;
        [SerializeField] private Transform secondItemPivot;
        [SerializeField] private List<MixGroup> recipes = new();

        private PickableItem _firstItem;
        private PickableItem _secondItem;

        private bool _firstResultPlaceClear = true;
        private bool _secondResultPlaceClear = true;

        private void Start() => Bind();

        private void OnDestroy() => Expose();

        private void CheckItems() // Called on both clients (network method)
        {
            if (!_firstItem || !_secondItem)
                return;

            var result = recipes.FirstOrDefault(x =>
                (x.firstIngredient == _firstItem.Item && x.secondIngredient == _secondItem.Item) ||
                (x.secondIngredient == _firstItem.Item && x.firstIngredient == _secondItem.Item));

            Destroy(_firstItem.gameObject);
            Destroy(_secondItem.gameObject);

            if (result == null)
                ReturnTrash();
            else
                ReturnResult(result.result);
        }

        private void ReturnResult(ItemSO result)
        {
            SpawnEmpty();
            SpawnResult(result);
        }

        private void ReturnTrash()
        {
            SpawnEmpty();
            SpawnResult(trashReturnItem);
        }

        private void CheckResultSpace()
        {
            if(!_firstResultPlaceClear || !_secondResultPlaceClear)
                return;
            
            firstItemZone.ActivatePlaceZone();
            secondItemZone.ActivatePlaceZone();
        }

        private void OnFirstResultTaken()
        {
            _firstResultPlaceClear = true;
            CheckResultSpace();
        }
        
        private void OnSecondResultTaken()
        {
            _secondResultPlaceClear = true;
            CheckResultSpace();
        }

        private void SpawnEmpty()
        {
            _firstResultPlaceClear = false;
            
            if (!PhotonNetwork.IsMasterClient)
                return;

            var spawnedView = PhotonNetwork.Instantiate(
                $"{FoldersPaths.PICKABLE_PREFABS_PATH}{emptyReturnItem.AdditionalFolderPath}/{emptyReturnItem.Prefab.name}",
                secondItemPivot.position, secondItemPivot.rotation).GetComponent<PhotonView>();

            photonView.RPC(nameof(InitSpawnedPickable), RpcTarget.All, spawnedView.ViewID, 2);
        }

        private void SpawnResult(ItemSO result)
        {
            _secondResultPlaceClear = false;
            
            if (!PhotonNetwork.IsMasterClient)
                return;

            var spawnedView = PhotonNetwork.Instantiate(
                $"{FoldersPaths.PICKABLE_PREFABS_PATH}{result.AdditionalFolderPath}/{result.Prefab.name}",
                firstItemPivot.position, firstItemPivot.rotation).GetComponent<PhotonView>();

            photonView.RPC(nameof(InitSpawnedPickable), RpcTarget.All, spawnedView.ViewID, 1);
        }

        [PunRPC]
        private void InitSpawnedPickable(int viewID, int resultPlaceNumber)
        {
            var spawned = PhotonView.Find(viewID);
            SemiFunc.InjectObject(spawned.gameObject);

            if (resultPlaceNumber == 1)
                spawned.GetComponent<PickableItem>().OnInteract += OnFirstResultTaken;
            else
                spawned.GetComponent<PickableItem>().OnInteract += OnSecondResultTaken;
        }

        private void OnFirstItemPlaced(PickableItem item) // Called on both clients (network method)
        {
            item.SetInteractable(false);
            _firstItem = item;
            CheckItems();
        }

        private void OnSecondItemPlaced(PickableItem item) // Called on both clients (network method)
        {
            item.SetInteractable(false);
            _secondItem = item;
            CheckItems();
        }

        private void Bind()
        {
            firstItemZone.OnObjectPlaced += OnFirstItemPlaced;
            secondItemZone.OnObjectPlaced += OnSecondItemPlaced;
        }

        private void Expose()
        {
            firstItemZone.OnObjectPlaced -= OnFirstItemPlaced;
            secondItemZone.OnObjectPlaced -= OnSecondItemPlaced;
        }

        [Serializable]
        private class MixGroup
        {
            [field: SerializeField] public ItemSO firstIngredient;
            [field: SerializeField] public ItemSO secondIngredient;
            [field: SerializeField] public ItemSO result;
        }
    }
}