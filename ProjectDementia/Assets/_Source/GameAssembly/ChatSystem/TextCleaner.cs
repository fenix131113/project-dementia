using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace ChatSystem
{
    public static class TextCleaner
    {
        private static Dictionary<string, List<string>> categoryWords = new();
        private static HashSet<string> dictionary = new();

        public static void Load(string dictPath, string categoryDir)
        {
            dictionary = File.ReadAllLines(dictPath)
                .Select(w => WordFixer.Normalize(w.Trim())).ToHashSet();

            categoryWords.Clear();
            foreach (var file in Directory.GetFiles(categoryDir, "*.txt"))
            {
                var categoryName = Path.GetFileNameWithoutExtension(file);
                categoryWords[categoryName] = File.ReadAllLines(file)
                    .Select(w => WordFixer.Normalize(w.Trim())).ToList();
            }
        }

        public static string ApplyFilter(string input, List<string> activeCategories)
        {
            var words = input.Split(' ');
            var result = new List<string>();

            foreach (var word in words)
            {
                var normalized = WordFixer.Normalize(word);
                if (activeCategories.Any(cat => categoryWords.ContainsKey(cat) && categoryWords[cat].Contains(normalized)))
                    continue; // запрещённое слово
                result.Add(word);
            }

            return string.Join(" ", result);
        }

        public static string AutoCorrect(string input)
        {
            var words = input.Split(' ');
            var result = new List<string>();

            foreach (var word in words)
            {
                var normalized = WordFixer.Normalize(word);
                if (IsGarbage(normalized)) continue;

                if (dictionary.Contains(normalized))
                {
                    result.Add(word);
                }
                else
                {
                    var match = FindClosest(normalized);
                    if (match != null) result.Add(match);
                }
            }

            return string.Join(" ", result);
        }

        private static bool IsGarbage(string word)
        {
            return word.Length < 3 || word.Count(char.IsLetter) < word.Length / 2;
        }

        private static string? FindClosest(string word)
        {
            string? best = null;
            int bestDistance = int.MaxValue;
            foreach (var dictWord in dictionary)
            {
                if (Math.Abs(dictWord.Length - word.Length) > 2) continue;
                int dist = WordFixer.LevenshteinDistance(word, dictWord);
                if (dist < bestDistance)
                {
                    best = dictWord;
                    bestDistance = dist;
                    if (dist == 0) break;
                }
            }
            return best;
        }
    }
}