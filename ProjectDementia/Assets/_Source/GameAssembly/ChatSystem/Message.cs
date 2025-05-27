using TMPro;
using UnityEngine;
using UnityEngine.UI;

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