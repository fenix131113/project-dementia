using UnityEngine;
using System.Collections.Generic;

namespace ChatSystem
{
    public class WordFilterZone : MonoBehaviour
    {
        public List<WordCategoryAsset> categories;

        public List<string> GetActiveCategories()
        {
            var result = new List<string>();
            foreach (var cat in categories)
                if (!result.Contains(cat.categoryName))
                    result.Add(cat.categoryName);
            return result;
        }

        public Dictionary<string, HashSet<string>> GetWordSets()
        {
            var dict = new Dictionary<string, HashSet<string>>();
            foreach (var cat in categories)
            {
                if (!dict.ContainsKey(cat.categoryName))
                    dict[cat.categoryName] = new HashSet<string>();

                foreach (var word in cat.words)
                    dict[cat.categoryName].Add(WordFixer.Normalize(word));
            }
            return dict;
        }
    }
}
