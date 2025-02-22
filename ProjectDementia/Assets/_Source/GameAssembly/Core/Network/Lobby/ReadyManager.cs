using DG.Tweening;
using Interactable.Custom;
using Interactable.Custom.ClickInteractions;
using Photon.Pun;
using UnityEngine;

namespace Core.Network.Lobby
{
    [RequireComponent(typeof(PhotonView))]
    public class ReadyManager : MonoBehaviour
    {
        [SerializeField] private PhotonView photonView;
        [SerializeField] private Lamp inactiveLamp;
        [SerializeField] private Lamp activeLamp;
        [SerializeField] private InfoScreen firstPlayerInfoScreen;
        [SerializeField] private InfoScreen secondPlayerInfoScreen;
        [SerializeField] private Color readyColor = Color.green;
        [SerializeField] private Color unreadyColor = Color.red;
        [SerializeField] private ClickableButton readyButton;

        private int _selectedSceneToLoad = 3;
        private bool _firstPlayerReady;
        private bool _secondPlayerReady;

        private void Start() => Bind();

        private void OnDestroy() => Expose();

        private void OnReadyButtonClicked()
        {
            if (readyButton.IsPressed)
            {
                activeLamp.Activate();
                inactiveLamp.Deactivate();
            }
            else
            {
                inactiveLamp.Activate();
                activeLamp.Deactivate();
            }
            
            photonView.RPC(nameof(RPC_SetPlayerReady), RpcTarget.All, readyButton.IsPressed, PhotonNetwork.LocalPlayer.ActorNumber);
            photonView.RPC(nameof(CheckLoadLevelConditions), RpcTarget.MasterClient);
        }

        public void SetSelectedSceneToLoad(int sceneToLoad) => _selectedSceneToLoad = sceneToLoad;

        [PunRPC]
        private void RPC_SetPlayerReady(bool state, int playerID)
        {
            if (playerID == 1)
            {
                _firstPlayerReady = state;
                firstPlayerInfoScreen.SetColor(state ? readyColor : unreadyColor);
                firstPlayerInfoScreen.DrawText(state ? "\\/" : "X");
            }
            else
            {
                _secondPlayerReady = state;
                secondPlayerInfoScreen.SetColor(state ? readyColor : unreadyColor);
                secondPlayerInfoScreen.DrawText(state ? "\\/" : "X");
            }

            if (_firstPlayerReady && _secondPlayerReady)
                DOTween.KillAll();
        }

        [PunRPC]
        private void CheckLoadLevelConditions()
        {
            if (_firstPlayerReady && _secondPlayerReady)
                PhotonNetwork.LoadLevel(_selectedSceneToLoad);
        }

        private void Bind()
        {
            readyButton.OnInteract += OnReadyButtonClicked;
        }

        private void Expose()
        {
            readyButton.OnInteract -= OnReadyButtonClicked;
        }
    }
}