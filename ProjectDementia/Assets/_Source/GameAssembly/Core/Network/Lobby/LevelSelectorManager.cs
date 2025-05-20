using System.Collections.Generic;
using Interactable.Custom.Lobby;
using Photon.Pun;
using UnityEngine;
using VContainer;

namespace Core.Network.Lobby
{
    public class LevelSelectorManager : MonoBehaviourPun
    {
        [SerializeField] private List<LevelSelectorButton> levelSelectedButtons;
        [SerializeField] private LevelSelectorButton startSelectedButton;

        [Inject] private ReadyManager _readyManager;

        private LevelSelectorButton _selectedButton;

        private void Start()
        {
            SelectLevel(levelSelectedButtons.IndexOf(startSelectedButton)); // Call client method on both clients ;)
            Bind();
        }

        private void OnDestroy() => Expose();

        private void OnButtonClicked(LevelSelectorButton button) => photonView.RPC(nameof(SelectLevel), RpcTarget.All,
            levelSelectedButtons.IndexOf(button));

        [PunRPC]
        private void SelectLevel(int buttonIndex)
        {
            levelSelectedButtons.ForEach(x => x.DeactivateSelector());
            levelSelectedButtons[buttonIndex].ActivateSelector();
            _readyManager.SetSelectedSceneToLoad(levelSelectedButtons[buttonIndex].LevelToLoadIndex);
        }

        private void Bind() => levelSelectedButtons.ForEach(x => x.OnButtonClicked += OnButtonClicked);

        private void Expose() => levelSelectedButtons.ForEach(x => x.OnButtonClicked -= OnButtonClicked);
    }
}