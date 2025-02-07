using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Initializing;
using Photon.Pun;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace ChatSystem
{
    public class ChatManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private PhotonView netView;
        [SerializeField] private ScrollRect scrollRect;

        public TMP_InputField inputField;
        public Message Message;
        public VerticalLayoutGroup Content;
        public GameObject chatPanel;
        public GameObject additionalPanel;

        private PlayerController playerController;
        private float hideDelay = 5f;
        private Coroutine hideCoroutine;
        private PlayerObjects _playersObjects;
        private List<string> exceptWords;
        private List<string> brainrotWords;

        [Inject]
        private void Construct(PlayerObjects playersObjects)
        {
            _playersObjects = playersObjects;
        }

        private void Start()
        {
            LoadTexts();
            Bind();
            playerController = _playersObjects.PlayerObject.GetComponent<PlayerController>();
            UpdateChatPanelVisibility();
        }

        private void OnDestroy() => Expose();

        private void Update()
        {
            scrollRect.verticalNormalizedPosition = 0f;
            if (Input.GetKeyDown(KeyCode.Tab))
                ToggleChat();
        }

        private void LoadTexts()
        {
            var exceptPath = Path.Combine(Application.streamingAssetsPath, "Except.txt");
            var brainrotPath = Path.Combine(Application.streamingAssetsPath, "Brainrot.txt");

            var exceptContent = File.ReadAllText(exceptPath);
            var brainrotContent = File.ReadAllText(brainrotPath);

            exceptWords = exceptContent.Split('\n').ToList();
            brainrotWords = brainrotContent.Split('\n').ToList();

            exceptWords.RemoveAt(exceptWords.Count - 1);
            brainrotWords.RemoveAt(brainrotWords.Count - 1);
            
            for (var i = 0; i < exceptWords.Count; i++)
            {
                var trim = exceptWords[i].Trim();
                exceptWords[i] = trim;
            }
            
            for (var i = 0; i < brainrotWords.Count; i++)
            {
                var trim = brainrotWords[i].Trim();
                brainrotWords[i] = trim;
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
                playerController.Controll = false;
                PlayerController.ShowCursor();

                if (hideCoroutine != null) StopCoroutine(hideCoroutine);
                hideCoroutine = StartCoroutine(HideChatPanelAfterDelay(hideDelay));
            }
            else
            {
                playerController.Controll = true;
                PlayerController.HideCursor();
            }
        }

        public void SendChatMessage(string message = "")
        {
            if (!string.IsNullOrEmpty(inputField.text.Trim()))
            {
                netView.RPC(nameof(GetMessage), RpcTarget.All, PhotonNetwork.NickName + ": " + inputField.text.Trim());
                inputField.text = string.Empty;
                inputField.ActivateInputField();
                UpdateChatPanelVisibility();
            }
        }

        [PunRPC]
        public void GetMessage(string ReceiveMessage)
        {
            var prefix = ReceiveMessage.Split(":")[0];
            var message = ReceiveMessage.Split(":")[1];
            foreach (var word in exceptWords.Where(word => message.Trim().Replace("_", "").Replace("|", "").Replace("!", "").Replace(".", "")
                         .Replace(",", "").Replace("+", "").Replace("-", "").ToLower().Contains(word.ToLower())))
            {
                ReceiveMessage = prefix + ": " + brainrotWords[Random.Range(0, brainrotWords.Count)];
            }

            var M = Instantiate(Message, Vector3.zero, Quaternion.identity, Content.transform);
            M.MyMessage.text = ReceiveMessage;
            UpdateChatPanelVisibility();
        }

        private void UpdateChatPanelVisibility()
        {
            var hasMessages = Content.transform.childCount > 0;
            additionalPanel.SetActive(hasMessages || additionalPanel.activeSelf);
            scrollRect.verticalNormalizedPosition = 0f;
        }

        private void Bind() => inputField.onSubmit.AddListener(SendChatMessage);

        private void Expose() => inputField.onSubmit.RemoveAllListeners();

        private IEnumerator HideChatPanelAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (Content.transform.childCount != 0 || inputField.isFocused)
                yield break;

            chatPanel.SetActive(false);
            additionalPanel.SetActive(false);
        }
    }
}