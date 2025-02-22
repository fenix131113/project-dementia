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

        public abstract event Action OnInteract;

        public void RPC_Interact(RpcTarget target) => PhotonView.RPC(nameof(Interact), target);

        [PunRPC]
        public abstract void Interact();
    }
}