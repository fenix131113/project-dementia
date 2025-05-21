using System;
using System.Collections.Generic;
using System.Linq;
using ItemsSystem.Data;
using UnityEngine;

namespace InventorySystem.Data
{
    [Serializable]
    public class ItemTagsPair
    {
        [field: SerializeField] public ItemSO Item { get; private set; }
        [field: SerializeField] public List<string> Tags { get; private set; }
    }

    public static class ItemTagsPairExtension
    {
        public static bool CheckItemTags(this ItemTagsPair pair, ItemSO item, List<string> tags,
            ItemTagsCompare compareType)
        {
            if (pair.Item != item)
                return false;

            return compareType switch
            {
                ItemTagsCompare.EQUAL => tags.Count == pair.Tags.Count && pair.Tags.Equals(tags),
                ItemTagsCompare.ANY => tags.Any(tag => pair.Tags.Contains(tag)),
                ItemTagsCompare.MINIMUM_ALL => tags.All(tag => pair.Tags.Contains(tag)),
                _ => throw new ArgumentOutOfRangeException(nameof(compareType), compareType, null)
            };
        }
    }

    public enum ItemTagsCompare
    {
        EQUAL = 0,
        ANY = 1,
        MINIMUM_ALL = 2
    }
}