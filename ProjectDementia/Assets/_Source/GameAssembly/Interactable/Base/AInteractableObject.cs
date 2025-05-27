using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace Interactable.Base
{
    [RequireComponent(typeof(PhotonView))]
    public abstract class AInteractableObject : MonoBehaviour
    {
        [field: SerializeField] public string InteractText { get; private set; }
        [field: SerializeField] public PhotonView PhotonView { get; private set; }
        [SerializeField] protected UnityEvent onInteractEvent;

        public bool CanInteract { get; private set; } = true;

        public abstract event Action OnInteract;

        [PunRPC]
        public virtual void Interact()
        {
            if (!CanInteract)
                return;
        }

        public void SetInteractable_RPC(bool canInteract) =>
            PhotonView.RPC(nameof(SetInteractable_RPC), RpcTarget.All, canInteract);

        [PunRPC]
        public void SetInteractable(bool canInteract) => CanInteract = canInteract;
    }
}