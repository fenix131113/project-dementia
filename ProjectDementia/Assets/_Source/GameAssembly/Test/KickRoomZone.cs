using System;
using Photon.Pun;
using UnityEngine;
using Utils;

namespace Test
{
    public class KickRoomZone : MonoBehaviour
    {
        [SerializeField] private LayerMask interactableLayer;

        public void Kick()
        {
            PhotonNetwork.LeaveRoom();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!LayerService.CheckLayersEquality(other.gameObject.layer, interactableLayer))
                return;
            
            Kick();
        }
    }
}