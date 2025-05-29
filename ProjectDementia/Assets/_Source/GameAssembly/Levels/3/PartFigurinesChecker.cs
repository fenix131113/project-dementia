using System;
using System.Collections.Generic;
using Interactable.Custom.ClickInteractions;
using ItemsSystem.Data;
using UnityEngine;
using UnityEngine.Events;

namespace Levels._3
{
    public class PartFigurinesChecker : MonoBehaviour
    {
        [SerializeField] private List<ItemObjectPlaceZone> firstPlayerZones;
        [SerializeField] private List<ItemObjectPlaceZone> secondPlayerZones;
        [SerializeField] private List<PartFigureAnswerGroup> firstAnswer;
        [SerializeField] private List<PartFigureAnswerGroup> secondAnswer;
        [SerializeField] private UnityEvent onPuzzleCompleted;

        private bool _firstCompleted;
        private bool _secondCompleted;
        private bool _puzzleCompleted;

        private void Start() => Bind();

        private void OnDestroy() => Expose();

        private void CheckCompletedConditions()
        {
            if (!_firstCompleted || !_secondCompleted)
                return;

            _puzzleCompleted = true;
            onPuzzleCompleted?.Invoke();
        }

        private void OnFirstRotationChanged() => OnFirstObjectItemChanged(null);
        private void OnSecondRotationChanged() => OnSecondObjectItemChanged(null);

        private void OnFirstObjectItemChanged(PickableItem item) // Call on both clients via Bind()
        {
            if (_puzzleCompleted)
                return;

            if (item)
                item.GetComponent<PartFigurines>().OnRotateChanged += OnFirstRotationChanged;

            var completed = true;

            for (var i = 0; i < 3; i++)
                if (!firstPlayerZones[i].SpawnedItem)
                {
                    completed = false;
                    break;
                }
                else if (firstAnswer[i].Item != firstPlayerZones[i].SpawnedItem.Item)
                {
                    completed = false;
                    break;
                }
                else if (firstAnswer[i].RotateIndex !=
                         firstPlayerZones[i].SpawnedItem.GetComponent<PartFigurines>().CurrentRotateIndex)
                {
                    completed = false;
                    break;
                }

            if (!completed)
                return;

            firstPlayerZones.ForEach(x => x.SpawnedItem.SetInteractable(false));
            _firstCompleted = true;
            CheckCompletedConditions();
        }

        private void OnSecondObjectItemChanged(PickableItem item) // Call on both clients via Bind()
        {
            if (_puzzleCompleted)
                return;

            if (item)
                item.GetComponent<PartFigurines>().OnRotateChanged += OnSecondRotationChanged;

            var completed = true;

            for (var i = 0; i < 3; i++)
                if (!secondPlayerZones[i].SpawnedItem)
                {
                    completed = false;
                    break;
                }
                else if (secondAnswer[i].Item != secondPlayerZones[i].SpawnedItem.Item)
                {
                    completed = false;
                    break;
                }
                else if (secondAnswer[i].RotateIndex !=
                         secondPlayerZones[i].SpawnedItem.GetComponent<PartFigurines>().CurrentRotateIndex)
                {
                    completed = false;
                    break;
                }

            if (!completed)
                return;

            secondPlayerZones.ForEach(x => x.SpawnedItem.SetInteractable(false));
            _secondCompleted = true;
            CheckCompletedConditions();
        }

        private void Bind()
        {
            firstPlayerZones.ForEach(x => x.OnObjectPlaced += OnFirstObjectItemChanged);
            secondPlayerZones.ForEach(x => x.OnObjectPlaced += OnSecondObjectItemChanged);
        }

        private void Expose()
        {
            firstPlayerZones.ForEach(x => x.OnObjectPlaced -= OnFirstObjectItemChanged);
            secondPlayerZones.ForEach(x => x.OnObjectPlaced -= OnSecondObjectItemChanged);
        }

        [Serializable]
        private class PartFigureAnswerGroup
        {
            [field: SerializeField] public ItemSO Item { get; private set; }
            [field: SerializeField] public int RotateIndex { get; private set; }
        }
    }
}