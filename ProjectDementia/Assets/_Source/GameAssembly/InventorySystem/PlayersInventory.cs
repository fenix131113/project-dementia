using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

namespace InventorySystem
{
    public class PlayersInventory
    {
        private readonly Dictionary<Photon.Realtime.Player, Inventory> _playersInventory = new();
        
        public PlayersInventory()
        {
            foreach (var player in PhotonNetwork.PlayerList)
                _playersInventory.Add(player, new Inventory());
        }

        public Inventory GetPlayerInventory(Photon.Realtime.Player player) => _playersInventory[player];
    }
}