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

        private bool _pressed;
        
        public void RPC_ResetPlate() => netView.RPC(nameof(ResetPlate), RpcTarget.AllBuffered);
        
        public override void Press()
        {
            base.Press();
            
            if(_pressed)
                return;
            
            _pressed = true;
            
            OnPressed?.Invoke(this);
        }

        public void ResetPlate()
        {
            _pressed = false;
            InfoScreen.ResetScreen();
        }
    }
}