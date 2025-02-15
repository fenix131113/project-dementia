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

        public event Action<ObjectPressablePlate> OnPressed;
        public event Action<ObjectPressablePlate, GameObject> OnPressedOwner;
        
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
    }
}