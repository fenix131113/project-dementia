using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace Levels
{
    public class PlayerCheckCollider : MonoBehaviourPun
    {
        [SerializeField] private LayerMask playerLayer;
        [SerializeField] private bool rpcEvent = true;
        [SerializeField] private UnityEvent onPlayerEnter;

        private void OnTriggerEnter(Collider other)
        {
            if (!LayerService.CheckLayersEquality(other.gameObject.layer, playerLayer))
                return;

            if (rpcEvent)
                photonView.RPC(nameof(InvokeEvent), RpcTarget.All);
            else
                InvokeEvent();
        }

        [PunRPC]
        private void InvokeEvent()
        {
            onPlayerEnter.Invoke();
        }
    }
}