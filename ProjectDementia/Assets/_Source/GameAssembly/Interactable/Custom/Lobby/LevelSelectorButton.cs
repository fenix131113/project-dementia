using System;
using Interactable.Base;
using UnityEngine;

namespace Interactable.Custom.Lobby
{
    public class LevelSelectorButton : AInteractableObject
    {
        [field: SerializeField] public int LevelToLoadIndex { get; private set; }
        [SerializeField] private MeshRenderer buttonRenderer;
        [SerializeField] private Material defaultButtonMaterial;
        [SerializeField] private Material activatedButtonMaterial;
        
        public override event Action OnInteract;
        public event Action<LevelSelectorButton> OnButtonClicked;

        public override void Interact()
        {
            base.Interact();
            OnInteract?.Invoke();
            OnButtonClicked?.Invoke(this);
        }
        
        public void ActivateSelector() => buttonRenderer.material = activatedButtonMaterial;

        public void DeactivateSelector() => buttonRenderer.material = defaultButtonMaterial;
    }
}