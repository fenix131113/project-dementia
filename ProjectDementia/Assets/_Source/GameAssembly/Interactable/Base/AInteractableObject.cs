using System;
using Photon.Pun;
using UnityEngine;

namespace Interactable.Base
{
    [RequireComponent(typeof(PhotonView))]
    public abstract class AInteractableObject : MonoBehaviour
    {
        [field: SerializeField] public PhotonView PhotonView { get; private set; }
        
        public abstract event Action OnInteract;
        
        [PunRPC]
        public abstract void Interact();
    }
}