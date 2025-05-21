using System;
using Interactable.Base;
using UnityEngine;

namespace Interactable.Custom.Lobby
{
    public class LevelSelectorButton : AInteractableObject
    {
        [field: SerializeField] public int LevelToLoadIndex { get; private set; }
        [SerializeField] private GameObject selectedSign;
        
        public override event Action OnInteract;
        public event Action<LevelSelectorButton> OnButtonClicked;

        public override void Interact()
        {
            base.Interact();
            OnInteract?.Invoke();
            OnButtonClicked?.Invoke(this);
        }
        
        public void ActivateSelector()
        {
            selectedSign.SetActive(true);
        }

        public void DeactivateSelector()
        {
            selectedSign.SetActive(false);
        }
    }
}