using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace ChatSystem
{
    public static class WordFixer
    {
        public static int LevenshteinDistance(string firstWord, string secondWord)
        {
            var n = firstWord.Length + 1;
            var m = secondWord.Length + 1;
            var pool = ArrayPool<int>.Shared;
            int[] buffer = pool.Rent(n * m);

            try
            {
                for (int i = 0; i < n; i++)
                    buffer[i * m + 0] = i;
                for (int j = 0; j < m; j++)
                    buffer[0 * m + j] = j;

                for (int i = 1; i < n; i++)
                {
                    for (int j = 1; j < m; j++)
                    {
                        int substitutionCost = firstWord[i - 1] == secondWord[j - 1] ? 0 : 1;

                        int deletion = buffer[(i - 1) * m + j] + 1;
                        int insertion = buffer[i * m + (j - 1)] + 1;
                        int substitution = buffer[(i - 1) * m + (j - 1)] + substitutionCost;

                        buffer[i * m + j] = Math.Min(Math.Min(deletion, insertion), substitution);
                    }
                }

                return buffer[(n - 1) * m + (m - 1)];
            }
            finally
            {
                pool.Return(buffer);
            }
        }

        private static readonly Dictionary<char, char> _replacements = new()
        {
            ['0'] = 'î',
            ['1'] = 'è',
            ['3'] = 'ç',
            ['4'] = '÷',
            ['6'] = 'á',
            ['8'] = 'â',
            ['9'] = 'ä',
            ['@'] = 'à',
            ['c'] = 'ñ',
            ['i'] = 'è',
            ['y'] = 'ó',
            ['x'] = 'õ',
            ['e'] = 'å',
            ['a'] = 'à',
            ['o'] = 'î',
            ['p'] = 'ð',
            ['k'] = 'ê',
            ['b'] = 'â',
            ['t'] = 'ò',
            ['m'] = 'ì',
            ['h'] = 'í'
        };

        public static string Normalize(string input)
        {
            var sb = new StringBuilder(input.Length);
            foreach (char ch in input.ToLowerInvariant())
                sb.Append(_replacements.TryGetValue(ch, out var mapped) ? mapped : ch);
            return sb.ToString();
        }
    }
}