using System.Collections.Generic;
using ItemsSystem;

namespace InventorySystem
{
    public class InventoryItem
    {
        public Item Item { get; private set; }
        public List<string> CustomData { get; private set; }

        public InventoryItem(Item item, List<string> customData)
        {
            Item = item;
            CustomData = customData;
        }
        
        public bool TryRemoveDataTag(string dataTag) // TODO: Replace with single logic (interface, class or something)
        {
            if (!CustomData.Contains(dataTag))
                return false;

            CustomData.Remove(dataTag);
            return true;
        }

        public void AddCustomDataTag(string dataTag)
        {
            if(CustomData.Contains(dataTag))
                return;
            
            CustomData.Add(dataTag);
        }
    }
}