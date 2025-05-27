using System;
using System.Collections.Generic;
using Interactable.Custom.ClickInteractions;
using Interactable.Custom.Screens;
using Interactable.Custom.TransmissionBox;
using ItemsSystem.Data;
using UnityEngine;
using UnityEngine.Events;

namespace Levels._3
{
    public class FigureGameManager : MonoBehaviour
    {
        [SerializeField] private BlockableTransitionBox firstPlayerBox;
        [SerializeField] private BlockableTransitionBox secondPlayerBox;
        [SerializeField] private MaterialScreen firstPlayerScreen;
        [SerializeField] private MaterialScreen secondPlayerScreen;
        [SerializeField] private List<PhaseGroup> phases;

        private int _currentPhaseIndex;
        private bool _isFirstPlaced;
        private bool _isSecondPlaced;

        private void Start()
        {
            SetCurrentPhase();
            Bind();
        }

        private void OnDestroy() => Expose();

        private void SetCurrentPhase()
        {
            if (phases[_currentPhaseIndex].showColorToFirstPlayer)
                firstPlayerScreen.SetMaterial(phases[_currentPhaseIndex].screenMaterial);
            else
                secondPlayerScreen.SetMaterial(phases[_currentPhaseIndex].screenMaterial);
        }

        private void CheckForNextPhase() // Called on both clients (network method)
        {
            if (!_isFirstPlaced || !_isSecondPlaced || _currentPhaseIndex >= phases.Count)
                return;

            var phaseCompleted = firstPlayerBox.CurrentItem &&
                                 firstPlayerBox.CurrentItem.Item == phases[_currentPhaseIndex].firstAnswer;

            if (!secondPlayerBox.CurrentItem ||
                secondPlayerBox.CurrentItem.Item != phases[_currentPhaseIndex].secondAnswer)
                phaseCompleted = false;

            if (phaseCompleted)
            {
                firstPlayerBox.ToggleFirst(false, true);
                secondPlayerBox.ToggleSecond(false, true);
                firstPlayerBox.CurrentItem.OnInteract += OnFirstItemTaken; // Expose in PickableItem
                secondPlayerBox.CurrentItem.OnInteract += OnSecondItemTaken; // Expose in PickableItem
                _currentPhaseIndex++;
                firstPlayerScreen.ResetScreen();
                secondPlayerScreen.ResetScreen();
                
                if (_currentPhaseIndex >= phases.Count)
                {
                    firstPlayerBox.ToggleBoxActivation(false);
                    secondPlayerBox.ToggleBoxActivation(false);
                    return;
                }
            }
            else
            {
                firstPlayerBox.ToggleFirst(true, false);
                secondPlayerBox.ToggleSecond(true, false);
            }
            
            _isFirstPlaced = false;
            _isSecondPlaced = false;

            if (_currentPhaseIndex == phases.Count)
                return;

            SetCurrentPhase();
        }

        private void SetFirstPlaced()
        {
            _isFirstPlaced = true;
            CheckForNextPhase();
        }

        private void SetSecondPlaced()
        {
            _isSecondPlaced = true;
            CheckForNextPhase();
        }

        private void OnFirstItemTaken() => firstPlayerBox.ToggleSecond(false, true);

        private void OnSecondItemTaken() => secondPlayerBox.ToggleFirst(false, true);

        private void Bind()
        {
            firstPlayerBox.OnItemPlacedEvent += SetFirstPlaced;
            secondPlayerBox.OnItemPlacedEvent += SetSecondPlaced;
        }

        private void Expose()
        {
            firstPlayerBox.OnItemPlacedEvent -= SetFirstPlaced;
            secondPlayerBox.OnItemPlacedEvent -= SetSecondPlaced;
        }

        [Serializable]
        private class PhaseGroup
        {
            [field: SerializeField] public bool showColorToFirstPlayer;
            [field: SerializeField] public Material screenMaterial;
            [field: SerializeField] public ItemSO firstAnswer;
            [field: SerializeField] public ItemSO secondAnswer;
        }
    }
}