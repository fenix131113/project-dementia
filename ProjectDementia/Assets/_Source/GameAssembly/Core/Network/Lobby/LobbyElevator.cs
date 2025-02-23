using Photon.Pun;
using R3;
using UnityEngine;
using Utils;

namespace Core.Network.Lobby
{
    public class LobbyElevator : MonoBehaviour
    {
        [SerializeField] protected LayerMask interactableLayer;
        [SerializeField] protected PhotonView netView;

        private bool _firstPlayerInElevator;
        private bool _secondPlayerInElevator;
        
        public readonly ReactiveProperty<bool> BothPlayerInElevator = new(false);

        private void OnTriggerEnter(Collider other)
        {
            if (!LayerService.CheckLayersEquality(other.gameObject.layer, interactableLayer))
                return;

            netView.RPC(nameof(RPC_ElevatorEnterStateUpdate), RpcTarget.MasterClient,
                PhotonNetwork.LocalPlayer.ActorNumber);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!LayerService.CheckLayersEquality(other.gameObject.layer, interactableLayer))
                return;

            netView.RPC(nameof(RPC_ElevatorExitStateUpdate), RpcTarget.MasterClient,
                PhotonNetwork.LocalPlayer.ActorNumber);
        }

        [PunRPC]
        private void RPC_ElevatorEnterStateUpdate(int playerID)
        {
            if (playerID == 1)
                _firstPlayerInElevator = true;
            else
                _secondPlayerInElevator = true;

            BothPlayerInElevator.Value = _firstPlayerInElevator && _secondPlayerInElevator;
        }

        [PunRPC]
        private void RPC_ElevatorExitStateUpdate(int playerID)
        {
            if (playerID == 1)
                _firstPlayerInElevator = false;
            else
                _secondPlayerInElevator = false;
            
            BothPlayerInElevator.Value = _firstPlayerInElevator && _secondPlayerInElevator;
        }
    }
}