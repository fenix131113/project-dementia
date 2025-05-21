using Interactable.Custom.ClickInteractions;
using UnityEngine;

namespace ItemsSystem.Data
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Configs/New Item")]
    public class ItemSO : ScriptableObject
    {
        [field: SerializeField] public string ItemName { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public PickableItem Prefab { get; private set; }
        [field: SerializeField] public string AdditionalFolderPath { get; private set; }
    }
}