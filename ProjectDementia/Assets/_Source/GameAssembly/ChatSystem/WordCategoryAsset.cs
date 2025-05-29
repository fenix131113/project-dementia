using System.Collections.Generic;
using UnityEngine;

namespace ChatSystem
{
    [CreateAssetMenu(fileName = "WordCategory", menuName = "Chat/Word Category", order = 0)]
    public class WordCategoryAsset : ScriptableObject
    {
        public string categoryName;
        public List<string> words;
    }
}
