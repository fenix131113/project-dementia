using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace Core.Network.Menu
{
    public class MainMenu : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject connectionLoaderPanel;
        [SerializeField] private TMP_InputField roomNameField;
        [SerializeField] private GameObject roomPanel;
        [SerializeField] private TMP_Text roomPlayersText;
        [SerializeField] private TMP_InputField nicknameInputField;

        [Space(15)] [SerializeField] private RoomListItem roomsItemPrefab;
        [SerializeField] private Transform roomsListContent;

        private readonly List<RoomListItem> _spawnedFreeRooms = new();
        private readonly List<RoomListItem> _spawnedUsedRooms = new();
        private List<RoomInfo> _rooms;
        private TypedLobby customLobby = new("customLobby", LobbyType.Default);

        private void Start() => ConnectToPhoton();

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
            Debug.Log("Joined Lobby");
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
            RedrawLabel();
            roomPanel.SetActive(true);
        }

        public override void OnLeftRoom()
        {
            roomPanel.SetActive(false);
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            RedrawLabel();
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            RedrawLabel();
        }

        private void RedrawLabel()
        {
            var room = PhotonNetwork.CurrentRoom;
            if (room == null)
            {
                Debug.Log("No current room");
                return;
            }

            roomPlayersText.text = "";

            roomPlayersText.text += room.Name + "\n";

            foreach (var playerPair in room.Players)
                roomPlayersText.text += playerPair.Value.NickName + " - " + playerPair.Value.ActorNumber + "\n";
        }

        public void LoadScene()
        {
            PhotonNetwork.LoadLevel(1);
        }

        public void CreateRoom()
        {
            if (string.IsNullOrEmpty(roomNameField.text))
                return;

            PhotonNetwork.CreateRoom(roomNameField.text, new RoomOptions { MaxPlayers = 3, IsVisible = true, IsOpen = true },
                customLobby);
            Debug.Log("Room created");
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
    }
}