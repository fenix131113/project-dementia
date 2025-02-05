using System;
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
        private RoomListItem roomsItemPrefab;

        [SerializeField] private Transform roomsListContent;

        private readonly List<RoomListItem> _spawnedFreeRooms = new();
        private readonly List<RoomListItem> _spawnedUsedRooms = new();
        private List<RoomInfo> _rooms;
        private TypedLobby customLobby = new("customLobby", LobbyType.Default);

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
            PhotonNetwork.SerializationRate = 40;
            PhotonNetwork.SendRate = 80;
        }

        public override void OnJoinedLobby()
        {
        }

        public void JoinLobby()
        {
            PhotonNetwork.JoinLobby(customLobby);
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
            roomPlayerCountLabel.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";

            foreach (var playerPair in room.Players)
                roomPlayersText.text += playerPair.Value.NickName + "\n";
        }

        private void CheckRoomConditionsForPlayer()
        {
            if(!PhotonNetwork.IsMasterClient || PhotonNetwork.CurrentRoom.PlayerCount != PhotonNetwork.CurrentRoom.MaxPlayers)
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
                new RoomOptions { MaxPlayers = 2, IsVisible = publicRoomToggle.isOn, IsOpen = true },
                customLobby);
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

        #endregion"

        private void Bind()
        {
            leftRoomButton.onClick.AddListener(OnLeftRoomButtonClick);
            loadLevelButton.onClick.AddListener(OnLoadLevelButtonClick);
            createRoomButton.onClick.AddListener(OnCreateRoomButtonClick);
            acceptRoomCreationButton.onClick.AddListener(OnAcceptRoomCreateButtonClick);
            returnFromRoomCreationButton.onClick.AddListener(OnReturnFromRoomCreationButtonClick);
        }

        private void Expose()
        {
            leftRoomButton.onClick.RemoveAllListeners();
            loadLevelButton.onClick.RemoveAllListeners();
            createRoomButton.onClick.RemoveAllListeners();
            acceptRoomCreationButton.onClick.RemoveAllListeners();
            returnFromRoomCreationButton.onClick.RemoveAllListeners();
        }
    }
}