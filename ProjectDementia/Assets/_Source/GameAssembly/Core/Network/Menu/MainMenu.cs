using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Core.Network.Menu
{
    public class MainMenu : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject connectionLoaderPanel;
        [SerializeField] private TMP_InputField nicknameInputField;
        [SerializeField] private Button exitGameButton;

        [Space(15)] [Header("Room")] [SerializeField]
        private GameObject roomPanel;

        [SerializeField] private GameObject createRoomPanel;
        [SerializeField] private TMP_Text roomPlayersText;
        [SerializeField] private TMP_Text roomNameLabel;
        [SerializeField] private TMP_Text roomPlayerCountLabel;
        [SerializeField] private TMP_InputField roomNameField;
        [SerializeField] private Toggle publicRoomToggle;
        [SerializeField] private Button createRoomButton;
        [SerializeField] private Button leftRoomButton;
        [SerializeField] private Button acceptRoomCreationButton;
        [SerializeField] private Button returnFromRoomCreationButton;
        [SerializeField] private Button loadLevelButton;

        [Space(15)] [Header("Rooms list")] [SerializeField]

        private float reloadRoomsListCooldown = 1.5f;
        [SerializeField] private RoomListItem roomsItemPrefab;
        [SerializeField] private Transform roomsListContent;
        [SerializeField] private GameObject roomsListPanel;
        [SerializeField] private TMP_InputField roomFieldToConnect;
        [SerializeField] private Button joinByRoomNameButton;
        [SerializeField] private Button roomsListButton;
        //[SerializeField] private Button reloadRoomsListButton;

        private readonly TypedLobby _customLobby = new("defaultLobby", LobbyType.Default);
        private readonly List<RoomListItem> _spawnedFreeRooms = new();
        private readonly List<RoomListItem> _spawnedUsedRooms = new();
        private List<RoomInfo> _rooms;
        private Coroutine _reloadRoomsListCoroutine;

        private void Start()
        {
            ConnectToPhoton();
            Bind();
        }

        private void OnDestroy() => Expose();

        private void ConnectToPhoton()
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.AutomaticallySyncScene = true;
            SetNickname("Player " + Random.Range(0, 100000));
        }


        public override void OnConnectedToMaster()
        {
            connectionLoaderPanel.SetActive(false);
            JoinLobby();
            PhotonNetwork.SerializationRate = 40;
            PhotonNetwork.SendRate = 80;
        }

        public override void OnJoinedLobby()
        {
            connectionLoaderPanel.SetActive(false);
        }

        public void JoinLobby()
        {
            PhotonNetwork.JoinLobby(_customLobby);
            connectionLoaderPanel.SetActive(true);
        }

        public void SetNicknameByField()
        {
            if (string.IsNullOrEmpty(nicknameInputField.text))
                return;

            SetNickname(nicknameInputField.text);
        }

        public void SetNickname(string nickname)
        {
            PhotonNetwork.NickName = nickname;
        }

        #region Room Management

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log(message);
        }

        public override void OnJoinedRoom()
        {
            CheckRoomConditionsForPlayer();
            RedrawLabel();
            createRoomPanel.SetActive(false);
            roomPanel.SetActive(true);
        }

        public override void OnLeftRoom()
        {
            roomPanel.SetActive(false);
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            RedrawLabel();
            CheckRoomConditionsForPlayer();
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            RedrawLabel();
            CheckRoomConditionsForPlayer();
        }

        private void RedrawLabel()
        {
            var room = PhotonNetwork.CurrentRoom;
            if (room == null)
            {
                Debug.LogWarning("No current room");
                return;
            }

            roomPlayersText.text = "";
            roomNameLabel.text = $"Room: {room.Name}";
            roomPlayerCountLabel.text =
                $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";

            foreach (var playerPair in room.Players)
                roomPlayersText.text += " - " + playerPair.Value.NickName + "\n";
        }

        private void CheckRoomConditionsForPlayer()
        {
            if (!PhotonNetwork.IsMasterClient ||
                PhotonNetwork.CurrentRoom.PlayerCount != PhotonNetwork.CurrentRoom.MaxPlayers)
                loadLevelButton.interactable = false;
            else
                loadLevelButton.interactable = true;
        }

        private void LoadScene()
        {
            PhotonNetwork.LoadLevel(1);
        }

        private void CreateRoom()
        {
            if (string.IsNullOrEmpty(roomNameField.text))
                return;

            PhotonNetwork.CreateRoom(roomNameField.text,
                new RoomOptions { MaxPlayers = 1, IsVisible = publicRoomToggle.isOn, IsOpen = true },
                _customLobby);
        }

        public void LeaveRoom() => PhotonNetwork.LeaveRoom();

        #endregion

        #region Rooms List

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            _rooms = roomList;
            ClearRoomsObjects();
            SpawnRoomsList();
        }

        private void ClearRoomsObjects()
        {
            foreach (var roomListItem in _spawnedUsedRooms)
            {
                roomListItem.ItemButton.onClick.RemoveAllListeners();
                roomListItem.gameObject.SetActive(false);
                _spawnedFreeRooms.Add(roomListItem);
            }

            _spawnedUsedRooms.Clear();
        }

        private void SpawnRoomsList()
        {
            foreach (var room in _rooms)
            {
                if (room.MaxPlayers < 2)
                    continue;

                var spawnedItem = _spawnedFreeRooms.Count > 0 ? _spawnedFreeRooms[0] : CreateRoomListItem();

                spawnedItem.Setup(room);
                spawnedItem.ItemButton.onClick.AddListener(() => PhotonNetwork.JoinRoom(room.Name));
                spawnedItem.gameObject.SetActive(true);
                _spawnedUsedRooms.Add(spawnedItem);
                _spawnedFreeRooms.RemoveAt(0);
            }
        }

        private void JoinRoomByName(string roomName)
        {
            if (!PhotonNetwork.JoinRoom(roomName)) 
                Debug.LogWarning("This room doesn't exist or full");
        }

        private RoomListItem CreateRoomListItem()
        {
            var spawned = Instantiate(roomsItemPrefab, roomsListContent);
            _spawnedFreeRooms.Add(spawned);
            return spawned;
        }

        #endregion

        #region Buttons

        private void OnCreateRoomButtonClick()
        {
            createRoomPanel.SetActive(true);
        }

        private void OnAcceptRoomCreateButtonClick() => CreateRoom();

        private void OnReturnFromRoomCreationButtonClick()
        {
            createRoomPanel.SetActive(false);
        }

        private void OnLeftRoomButtonClick() => LeaveRoom();

        private void OnLoadLevelButtonClick()
        {
            if (PhotonNetwork.IsMasterClient)
                LoadScene();
        }

        private void OnJoinRoomByNameButtonClick()
        {
            if (!string.IsNullOrEmpty(roomFieldToConnect.text))
                JoinRoomByName(roomFieldToConnect.text);
        }
        
        private void OnExitGameButtonClick() => Application.Quit();

        private void OnRoomListButtonClick()
        {
            //if(!roomsListPanel.activeSelf && _reloadRoomsListCoroutine == null)
                //JoinLobby();
            
            roomsListPanel.SetActive(!roomsListPanel.activeSelf);
        }

        private void OnReloadRoomsListButtonClick()
        {
            _reloadRoomsListCoroutine = StartCoroutine(ReloadRoomsListCooldown());
            JoinLobby();
        }

        #endregion

        private void Bind()
        {
            exitGameButton.onClick.AddListener(OnExitGameButtonClick);
            leftRoomButton.onClick.AddListener(OnLeftRoomButtonClick);
            roomsListButton.onClick.AddListener(OnRoomListButtonClick);
            loadLevelButton.onClick.AddListener(OnLoadLevelButtonClick);
            createRoomButton.onClick.AddListener(OnCreateRoomButtonClick);
            joinByRoomNameButton.onClick.AddListener(OnJoinRoomByNameButtonClick);
            //reloadRoomsListButton.onClick.AddListener(OnReloadRoomsListButtonClick);
            acceptRoomCreationButton.onClick.AddListener(OnAcceptRoomCreateButtonClick);
            returnFromRoomCreationButton.onClick.AddListener(OnReturnFromRoomCreationButtonClick);
        }

        private void Expose()
        {
            exitGameButton.onClick.RemoveAllListeners();
            leftRoomButton.onClick.RemoveAllListeners();
            roomsListButton.onClick.RemoveAllListeners();
            loadLevelButton.onClick.RemoveAllListeners();
            createRoomButton.onClick.RemoveAllListeners();
            joinByRoomNameButton.onClick.RemoveAllListeners();
            //reloadRoomsListButton.onClick.RemoveAllListeners();
            acceptRoomCreationButton.onClick.RemoveAllListeners();
            returnFromRoomCreationButton.onClick.RemoveAllListeners();
        }

        private IEnumerator ReloadRoomsListCooldown()
        {
            //reloadRoomsListButton.interactable = false;
            
            yield return new WaitForSeconds(reloadRoomsListCooldown);
            
            //reloadRoomsListButton.interactable = true;
            _reloadRoomsListCoroutine = null;
        }
    }
}