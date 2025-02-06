using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Interactable.Custom
{
    [RequireComponent(typeof(PhotonView))]
    public class InfoScreen : MonoBehaviour
    {
        [SerializeField] private TMP_Text infoText;
        [SerializeField] private string defaultText;

        private void Start() => ResetScreen();

        public void ResetScreen() => infoText.text = defaultText;

        public void DrawText(string text) => infoText.text = text;
    }
}