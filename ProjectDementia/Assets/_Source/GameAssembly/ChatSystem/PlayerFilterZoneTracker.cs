using UnityEngine;
using System.Collections.Generic;

namespace ChatSystem
{
    public class PlayerFilterZoneTracker : MonoBehaviour
    {
        public List<string> currentFilters = new();
        public Dictionary<string, HashSet<string>> wordSets = new();

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out WordFilterZone zone))
            {
                currentFilters.AddRange(zone.GetActiveCategories());
                var sets = zone.GetWordSets();
                foreach (var kvp in sets)
                    wordSets[kvp.Key] = kvp.Value;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out WordFilterZone zone))
            {
                foreach (var cat in zone.GetActiveCategories())
                {
                    currentFilters.Remove(cat);
                    wordSets.Remove(cat);
                }
            }
        }
    }
}
