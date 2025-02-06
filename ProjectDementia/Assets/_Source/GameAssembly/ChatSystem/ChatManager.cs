using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Player;

namespace ChatSystem
{
    public class ChatManager : MonoBehaviourPunCallbacks
    {
        public TMP_InputField inputField;
        public GameObject Message;
        public GameObject Content;
        public GameObject chatPanel;
        public GameObject additionalPanel;

        private PlayerController playerController;
        private float hideDelay = 5f;
        private Coroutine hideCoroutine;

        private void Start()
        {
            playerController = FindObjectOfType<PlayerController>();
            UpdateChatPanelVisibility();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ToggleChat();
            }

            if (inputField.isFocused && Input.GetKeyDown(KeyCode.Return))
            {
                SendMessage();
            }
        }

        public void ToggleChat()
        {
            bool isActive = chatPanel.activeSelf;
            chatPanel.SetActive(!isActive);
            inputField.gameObject.SetActive(!isActive);

            if (!isActive)
            {
                inputField.ActivateInputField();
                PlayerController.Instance.Controll = false;
                PlayerController.ShowCursor();

                if (hideCoroutine != null) StopCoroutine(hideCoroutine);
                hideCoroutine = StartCoroutine(HideChatPanelAfterDelay(hideDelay));
            }
            else
            {
                PlayerController.Instance.Controll = true;
                PlayerController.HideCursor();
            }
        }

        public void SendMessage()
        {
            if (!string.IsNullOrEmpty(inputField.text.Trim()))
            {
                GetComponent<PhotonView>().RPC("GetMessage", RpcTarget.All, PhotonNetwork.NickName + ": " + inputField.text.Trim());
                inputField.text = string.Empty;
                inputField.ActivateInputField();
                UpdateChatPanelVisibility();
            }
        }

        [PunRPC]
        public void GetMessage(string ReceiveMessage)
        {
            GameObject M = Instantiate(Message, Vector3.zero, Quaternion.identity, Content.transform);
            M.GetComponent<Message>().MyMessage.text = ReceiveMessage;
            UpdateChatPanelVisibility();
        }

        private void UpdateChatPanelVisibility()
        {
            bool hasMessages = Content.transform.childCount > 0;
            chatPanel.SetActive(hasMessages || chatPanel.activeSelf);
            additionalPanel.SetActive(hasMessages || additionalPanel.activeSelf);
        }

        private IEnumerator HideChatPanelAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (Content.transform.childCount == 0)
            {
                chatPanel.SetActive(false);
                additionalPanel.SetActive(false);
            }
        }
    }
}
