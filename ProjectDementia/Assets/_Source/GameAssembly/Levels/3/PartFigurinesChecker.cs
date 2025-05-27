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
        [SerializeField] private List<ItemSO> firstAnswer;
        [SerializeField] private List<ItemSO> secondAnswer;
        [SerializeField] private UnityEvent onPuzzleCompleted;

        private bool _firstCompleted;
        private bool _secondCompleted;

        private void Start() => Bind();

        private void OnDestroy() => Expose();

        private void CheckCompletedConditions()
        {
            if (!_firstCompleted || !_secondCompleted)
                return;

            onPuzzleCompleted?.Invoke();
        }

        private void OnFirstObjectItemChanged(PickableItem items) // Call on both clients via Bind()
        {
            var completed = true;

            for (var i = 0; i < 3; i++)
                if (!firstPlayerZones[i].SpawnedItem)
                {
                    completed = false;
                    break;
                }
                else if (firstAnswer[i] != firstPlayerZones[i].SpawnedItem.Item)
                {
                    completed = false;
                    break;
                }

            if (completed)
            {
                firstPlayerZones.ForEach(x => x.SpawnedItem.SetInteractable(false));
                _firstCompleted = true;
                CheckCompletedConditions();
            }
        }

        private void OnSecondObjectItemChanged(PickableItem items) // Call on both clients via Bind()
        {
            var completed = true;

            for (var i = 0; i < 3; i++)
                if (!secondPlayerZones[i].SpawnedItem)
                {
                    completed = false;
                    break;
                }
                else if (secondAnswer[i] != secondPlayerZones[i].SpawnedItem.Item)
                {
                    completed = false;
                    break;
                }

            if (completed)
            {
                secondPlayerZones.ForEach(x => x.SpawnedItem.SetInteractable(false));
                _secondCompleted = true;
                CheckCompletedConditions();
            }
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
    }
}