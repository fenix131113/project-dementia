using System;
using Interactable.Base;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace Interactable.Custom
{
    public class ObjectPressablePlate : APressablePlate
    {
        [SerializeField] private UnityEvent onPressedEvent;
        [SerializeField] private UnityEvent onReleasedEvent;
        [SerializeField] private bool canRelease;

        public event Action<ObjectPressablePlate> OnPressed;
        public event Action<ObjectPressablePlate, GameObject> OnPressedOwner;
        public event Action<ObjectPressablePlate> OnReleased;
        public event Action<ObjectPressablePlate, GameObject> OnReleasedOwner;
        
        private Action _customPressAction;
        
        public void RPC_ResetPlate() => netView.RPC(nameof(ResetPlate), RpcTarget.AllBuffered);

        public void SetCustomPressAction(Action pressAction) => _customPressAction = pressAction;

        public override void Press(GameObject initiator)
        {
            base.Press(initiator);
            
            if(Pressed)
                return;
            
            _customPressAction?.Invoke();
            Pressed = true;
            
            onPressedEvent?.Invoke();
            OnPressed?.Invoke(this);
            OnPressedOwner?.Invoke(this, initiator);
        }

        public override void Release(GameObject initiator)
        {
            base.Release(initiator);
            
            if(!Pressed || !canRelease)
                return;
            
            ResetPlate();
            UnblockPlate();
            
            onReleasedEvent?.Invoke();
            OnReleased?.Invoke(this);
            OnReleasedOwner?.Invoke(this, initiator);
        }
    }
}