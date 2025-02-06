using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ChatSystem
{
    public class Message : MonoBehaviour
    {
        public TMP_Text MyMessage;

        private void Start()
        {
            GetComponent<RectTransform>().SetAsFirstSibling();
        }
    }
}