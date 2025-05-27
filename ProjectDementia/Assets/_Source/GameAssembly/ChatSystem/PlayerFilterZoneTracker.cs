using UnityEngine;
using System.Collections.Generic;

namespace ChatSystem
{
    public class PlayerFilterZoneTracker : MonoBehaviour
    {
        public List<string> currentFilters = new();

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out WordFilterZone zone))
                currentFilters.AddRange(zone.activeWordLists);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out WordFilterZone zone))
                foreach (var item in zone.activeWordLists)
                    currentFilters.Remove(item);
        }
    }
}