using ItemsSystem;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace InventorySystem.View
{
    public class InventoryView : MonoBehaviour
    {
        [SerializeField] private Image icon;

        private Inventory _inventory;
        
        [Inject]
        private void Construct(PlayersInventory playersInventory)
        {
            _inventory = playersInventory.GetPlayerInventory(PhotonNetwork.LocalPlayer);
        }

        private void Start() => Bind();

        private void OnDestroy() => Expose();

        private void DrawInventory(Item item)
        {
            icon.sprite = item.Icon;
            Debug.Log("DrawInventory");
        }

        private void Bind() => _inventory.OnItemAdded += DrawInventory;

        private void Expose() => _inventory.OnItemAdded -= DrawInventory;
    }
}