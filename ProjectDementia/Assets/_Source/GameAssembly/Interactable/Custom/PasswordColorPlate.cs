using System;
using Interactable.Base;
using Photon.Pun;
using UnityEngine;

namespace Interactable.Custom
{
    public class PasswordColorPlate : APressablePlate
    {
        [field: SerializeField] public InfoScreen InfoScreen { get; private set; }

        public event Action<PasswordColorPlate> OnPressed;
        public event Action<PasswordColorPlate, GameObject> OnPressedOwner;

        private bool _pressed;
        private Action _customPressAction;
        
        public void RPC_ResetPlate() => netView.RPC(nameof(ResetPlate), RpcTarget.AllBuffered);

        public void SetCustomPressAction(Action pressAction) => _customPressAction = pressAction;

        public override void Press(GameObject initiator)
        {
            base.Press(initiator);
            
            if(_pressed)
                return;
            
            _customPressAction?.Invoke();
            _pressed = true;
            
            OnPressed?.Invoke(this);
            OnPressedOwner?.Invoke(this, initiator);
        }

        public void ResetPlate()
        {
            _pressed = false;
            InfoScreen?.ResetScreen();
        }
    }
}