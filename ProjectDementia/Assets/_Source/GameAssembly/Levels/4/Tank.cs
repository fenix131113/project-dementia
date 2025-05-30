using System;
using System.Collections.Generic;
using Core;
using Interactable.Custom.ClickInteractions;
using InventorySystem;
using InventorySystem.Data;
using ItemsSystem;
using ItemsSystem.Data;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace Levels._4
{
    public class Tank : MonoBehaviourPun
    {
        [SerializeField] private SelectedItemChecker firstItemTank;
        [SerializeField] private MeshRenderer[] firstTankIndicators;
        [SerializeField] private SelectedItemChecker secondItemTank;
        [SerializeField] private MeshRenderer[] secondTankIndicators;
        [SerializeField] private Material activatedIndicatorMat;
        [SerializeField] private List<TankAnswerGroup> answer;
        [SerializeField] private UnityEvent onLevelCompleted;

        [Inject] private ItemsContainer _itemsContainer;
        [Inject] private PlayersInventory _playersInventory;

        private int _currentPhaseIndex;

        private void Start() => Bind();

        private void OnDestroy() => Expose();

        private void CheckCompleteConditions(InventoryItem item, int inventoryID)
        {
            if (_currentPhaseIndex >= answer.Count - 1 ||
                _itemsContainer.GetItemBySO(answer[_currentPhaseIndex].Item).ID != item.Item.ID ||
                !new ItemTagsPair(_itemsContainer.GetSOByID(item.Item.ID), item.CustomData).CheckItemTags(
                    answer[_currentPhaseIndex].Item, answer[_currentPhaseIndex].Tags, ItemTagsCompare.EQUAL))
            {
                return;
            }

            secondTankIndicators[_currentPhaseIndex].material = activatedIndicatorMat;
            firstTankIndicators[_currentPhaseIndex].material = activatedIndicatorMat;
            _playersInventory.GetPlayerInventory(SemiFunc.GetPlayerByActorNumber(inventoryID)).TryRemoveItem(item);

            if (_currentPhaseIndex >= answer.Count - 1)
            {
                firstItemTank.SetInteractable(false);
                secondItemTank.SetInteractable(false);
                onLevelCompleted?.Invoke();
            }
            
            _currentPhaseIndex++;
        }

        private void Bind()
        {
            firstItemTank.OnItemAccepted += CheckCompleteConditions;
            secondItemTank.OnItemAccepted += CheckCompleteConditions;
        }

        private void Expose()
        {
            firstItemTank.OnItemAccepted -= CheckCompleteConditions;
            secondItemTank.OnItemAccepted -= CheckCompleteConditions;
        }

        [Serializable]
        private class TankAnswerGroup
        {
            [field: SerializeField] public ItemSO Item { get; private set; }
            [field: SerializeField] public List<string> Tags { get; private set; }
        }
    }
}