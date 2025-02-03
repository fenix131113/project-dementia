using UnityEngine;

namespace ItemsSystem.Data
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Configs/New Item")]
    public class ItemSO : ScriptableObject
    {
        [field: SerializeField] public string ItemName { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
    }
}