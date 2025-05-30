using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace ChatSystem
{
    public static class TextCleaner
    {
        private static Dictionary<string, HashSet<string>> categoryWords = new();
        private static HashSet<string> dictionary = new();
        private static bool _isInitialized = false;

        public static void Load(string dictPath)
        {
            if (_isInitialized)
                return;

            _isInitialized = true;

            if (!File.Exists(dictPath))
            {
                Debug.LogError("TextCleaner: russian.txt not found at " + dictPath);
                return;
            }

            dictionary = File.ReadAllLines(dictPath)
                .Select(w => WordFixer.Normalize(w.Trim()))
                .Where(w => w.Length > 0)
                .ToHashSet();

            //Debug.Log("TextCleaner: Loaded dictionary with " + dictionary.Count + " words.");

            string filtersDir = Path.Combine(Application.streamingAssetsPath, "Filters");
            if (Directory.Exists(filtersDir))
            {
                foreach (var file in Directory.GetFiles(filtersDir, "*.txt"))
                {
                    string category = Path.GetFileNameWithoutExtension(file);
                    var words = File.ReadAllLines(file).Select(WordFixer.Normalize).ToHashSet();
                    categoryWords[category] = words;
                }

                //Debug.Log("TextCleaner: Loaded " + categoryWords.Count + " filter categories from Filters/");
            }
        }

        public static string Clean(string input, PlayerFilterZoneTracker tracker)
        {
            var words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var result = new List<string>();

            foreach (var word in words)
            {
                var normalized = WordFixer.Normalize(word);

                if (IsGarbage(normalized))
                {
                    result.Add("...");
                    continue;
                }

                bool isFiltered = tracker.wordSets.Values.Any(set => set.Contains(normalized));
                result.Add(isFiltered ? "..." : word);
            }

            return string.Join(" ", result);
        }

        public static string AutoCorrect(string input)
        {
            var words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var result = new List<string>();

            foreach (var word in words)
            {
                var normalized = WordFixer.Normalize(word);

                if (IsGarbage(normalized))
                {
                    result.Add("...");
                    continue;
                }

                if (dictionary.Contains(normalized))
                {
                    result.Add(word);
                }
                else
                {
                    var match = FindClosest(normalized);
                    result.Add(match ?? word);
                }
            }

            return string.Join(" ", result);
        }

        private static bool IsGarbage(string word)
        {
            return (word.Length <= 2 && !dictionary.Contains(word)) ||
                   word.Count(char.IsLetter) < word.Length / 2;
        }

        private static string? FindClosest(string word)
        {
            string? best = null;
            int bestDistance = int.MaxValue;
            int limit = 2000;
            int checkedCount = 0;

            foreach (var dictWord in dictionary)
            {
                if (Math.Abs(dictWord.Length - word.Length) > 2) continue;

                int dist = WordFixer.LevenshteinDistance(word, dictWord);
                checkedCount++;

                if (dist < bestDistance)
                {
                    best = dictWord;
                    bestDistance = dist;
                    if (dist == 0) break;
                }

                if (checkedCount >= limit)
                    break;
            }

            return best;
        }
    }
}
