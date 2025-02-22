using System;
using Interactable.Base;

namespace Interactable.Custom.ClickInteractions
{
    public class ClickableButton : AInteractableObject
    {
        public override event Action OnInteract;

        public bool IsPressed { get; private set; }

        public override void Interact()
        {
            IsPressed = !IsPressed;
            
            OnInteract?.Invoke();
            onInteractEvent?.Invoke();
        }
    }
}