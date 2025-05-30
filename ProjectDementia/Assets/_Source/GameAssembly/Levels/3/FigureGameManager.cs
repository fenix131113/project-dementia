using System;
using System.Collections.Generic;
using Interactable.Custom.Screens;
using Interactable.Custom.TransmissionBox;
using ItemsSystem.Data;
using UnityEngine;

namespace Levels._3
{
    public class FigureGameManager : MonoBehaviour
    {
        [SerializeField] private OneDoorBox firstPlayerBox;
        [SerializeField] private OneDoorBox secondPlayerBox;
        [SerializeField] private Pipe firstPlayerTrashPipe;
        [SerializeField] private Pipe firstPlayerCorrectPipe;
        [SerializeField] private Pipe secondPlayerTrashPipe;
        [SerializeField] private Pipe secondPlayerCorrectPipe;
        [SerializeField] private MaterialScreen firstPlayerMainScreen;
        [SerializeField] private MaterialScreen firstPlayerPart1Screen;
        [SerializeField] private MaterialScreen firstPlayerPart2Screen;
        [SerializeField] private MaterialScreen secondPlayerMainScreen;
        [SerializeField] private MaterialScreen secondPlayerPart1Screen;
        [SerializeField] private MaterialScreen secondPlayerPart2Screen;
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
                firstPlayerMainScreen.SetMaterial(phases[_currentPhaseIndex].screenMaterial);
            else
                secondPlayerMainScreen.SetMaterial(phases[_currentPhaseIndex].screenMaterial);
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
                _currentPhaseIndex++;
                firstPlayerMainScreen.ResetScreen();
                secondPlayerMainScreen.ResetScreen();
                firstPlayerPart1Screen.ResetScreen();
                secondPlayerPart1Screen.ResetScreen();
                firstPlayerPart2Screen.ResetScreen();
                secondPlayerPart2Screen.ResetScreen();
                
                firstPlayerCorrectPipe.DropItem(secondPlayerBox.CurrentItem.Item);
                firstPlayerBox.ClearBox();
                OnFirstDropped();
                
                secondPlayerCorrectPipe.DropItem(firstPlayerBox.CurrentItem.Item);
                secondPlayerBox.ClearBox();
                OnFirstDropped();

                if (_currentPhaseIndex >= phases.Count)
                {
                    firstPlayerBox.SetPlaceAbility(false);
                    secondPlayerBox.SetPlaceAbility(false);
                    return;
                }
            }
            else
            {
                firstPlayerTrashPipe.DropItem(firstPlayerBox.CurrentItem.Item);
                firstPlayerBox.ClearBox();
                OnFirstDropped();

                secondPlayerTrashPipe.DropItem(secondPlayerBox.CurrentItem.Item);
                secondPlayerBox.ClearBox();
                OnSecondDropped();
            }

            firstPlayerBox.ToggleDoor(true);
            secondPlayerBox.ToggleDoor(true);

            _isFirstPlaced = false;
            _isSecondPlaced = false;

            if (_currentPhaseIndex == phases.Count)
                return;

            SetCurrentPhase();
        }

        private void SetFirstPlaced()
        {
            _isFirstPlaced = true;
            
            var mat = firstPlayerBox.CurrentItem.transform.GetChild(0)
                .GetComponent<MeshRenderer>().material;
            
            firstPlayerPart1Screen.SetMaterial(mat);
            secondPlayerPart1Screen.SetMaterial(mat);
            
            CheckForNextPhase();
        }

        private void SetSecondPlaced()
        {
            _isSecondPlaced = true;
                        
            var mat = secondPlayerBox.CurrentItem.transform.GetChild(0)
                .GetComponent<MeshRenderer>().material;
            
            firstPlayerPart2Screen.SetMaterial(mat);
            secondPlayerPart2Screen.SetMaterial(mat);

            CheckForNextPhase();
        }

        private void OnFirstDropped()
        {
            firstPlayerBox.ToggleDoor(true);
            firstPlayerPart1Screen.ResetScreen();
            secondPlayerPart1Screen.ResetScreen();
        }

        private void OnSecondDropped()
        {
            secondPlayerBox.ToggleDoor(true);
            firstPlayerPart2Screen.ResetScreen();
            secondPlayerPart2Screen.ResetScreen();
        }

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