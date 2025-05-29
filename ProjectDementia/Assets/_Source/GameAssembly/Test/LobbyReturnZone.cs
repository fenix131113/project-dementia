using Core.Network.Lobby;
using DG.Tweening;
using Levels;
using Photon.Pun;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace Test
{
    public class LobbyReturnZone : MonoBehaviourPun
    {
        [SerializeField] private LobbyElevator elevator;
        [SerializeField] private Image loadFadeScreen;
        [SerializeField] private int levelIndex;
        [SerializeField] private float fadeTime;
        [SerializeField] private Door elevatorDoor;

        private bool _loading;

        private void Start() => Bind();

        private void OnDestroy() => Expose();

        [PunRPC]
        private void CheckLevelLoadConditions(bool isBoth)
        {
            if (_loading || !isBoth)
                return;

            _loading = true;
            loadFadeScreen.DOFade(1f, fadeTime).onComplete += LoadLobby;

            elevatorDoor.CloseDoor();
        }

        private void LoadLobby()
        {
            if (PhotonNetwork.IsMasterClient)
                PhotonNetwork.LoadLevel(levelIndex);
        }

        private void Bind()
        {
            elevator.BothPlayerInElevator.Subscribe(isBoth =>
                photonView.RPC(nameof(CheckLevelLoadConditions), RpcTarget.All, isBoth));
        }

        private void Expose()
        {
            elevator.BothPlayerInElevator.Dispose();
        }
    }
}