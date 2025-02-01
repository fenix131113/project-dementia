using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Network.Menu
{
    public class RoomListItem : MonoBehaviour
    {
        [field: SerializeField] public Button ItemButton { get; private set; }
        
        [SerializeField] private TMP_Text roomNameLabel;
        [SerializeField] private TMP_Text roomPlayerCountLabel;

        public void Setup(RoomInfo roomInfo)
        {
            roomNameLabel.text = roomInfo.Name;
            roomPlayerCountLabel.text = roomInfo.PlayerCount.ToString() + "/" + roomInfo.MaxPlayers.ToString();
        }
    }
}