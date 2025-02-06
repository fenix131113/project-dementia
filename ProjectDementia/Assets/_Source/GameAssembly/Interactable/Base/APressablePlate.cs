using Photon.Pun;
using UnityEngine;
using Utils;

namespace Interactable.Base
{
    [RequireComponent(typeof(PhotonView))]
    public abstract class APressablePlate : MonoBehaviour
    {
        [SerializeField] protected LayerMask interactableLayer;
        [SerializeField] protected PhotonView netView;

        protected bool IsBlocked;

        private void OnTriggerEnter(Collider other)
        {
            if (!LayerService.CheckLayersEquality(other.gameObject.layer, interactableLayer))
                return;
            
            Press();
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (!LayerService.CheckLayersEquality(other.gameObject.layer, interactableLayer))
                return;
            
            Release();
        }

        public void BlockPlate() => IsBlocked = true;

        public virtual void Press()
        {
            if(IsBlocked)
                return;
        }

        public virtual void Release()
        {
            if(IsBlocked)
                return;
        }
    }
}