using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Interactable.Custom
{
    [RequireComponent(typeof(PhotonView))]
    public class InfoScreen : MonoBehaviour
    {
        [field: SerializeField] public Color DefaultColor { get; private set; } = Color.white;

        [SerializeField] private TMP_Text infoText;
        [SerializeField] private string defaultText;

        private void Start() => ResetScreen();

        public void ResetScreen()
        {
            infoText.color = DefaultColor;
            infoText.text = defaultText;
        }

        public void DrawText(string text) => infoText.text = text;

        public void SetColor(Color color) => infoText.color = color;

        public void SetColor(int r, int g, int b, float a = 1) => infoText.color = new Color(r, g, b, a);
    }
}