using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

namespace ChatSystem
{
    public class ChatManager : MonoBehaviourPunCallbacks
    {
        public TMP_InputField inputField;
        public GameObject Message;
        public GameObject Content;

        public void SendMessage()
        {
            if (!string.IsNullOrEmpty(inputField.text.Trim()))
            {
                GetComponent<PhotonView>().RPC("GetMessage", RpcTarget.All, inputField.text.Trim());
                inputField.text = string.Empty;
                inputField.ActivateInputField();  // Возвращает фокус на поле ввода
            }
        }

        [PunRPC]
        public void GetMessage(string ReceiveMessage)
        {
            GameObject M = Instantiate(Message, Vector3.zero, Quaternion.identity, Content.transform);
            M.GetComponent<Message>().MyMessage.text = ReceiveMessage;
        }
    }
}
