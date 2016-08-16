namespace TernarySearchTree.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class Keygenerator
    {
        public static IEnumerable<string> GenerateKeys(int seed, int maxKeyLength, char startChar, char endChar)
        {
            var random = new Random(seed);
            for (;;)
            {
                yield return GenerateKey(random, maxKeyLength, startChar, endChar);
            }
        }

        public static HashSet<string> GenerateKeys(int seed, int maxKeyLength, char startChar, char endChar, int count)
        {
            var keys = new HashSet<string>();
            var random = new Random(seed);
            while (keys.Count != count)
            {
                var key = GenerateKey(random, maxKeyLength, startChar, endChar);

                if (keys.Contains(key) == false)
                {
                    keys.Add(key);
                }
            }

            return keys;
        }

        private static string GenerateKey(Random random, int maxKeyLength, char startChar, char endChar)
        {
            var length = random.Next(1, maxKeyLength + 1);
            return new string(Enumerable.Range(0, length).Select(i => (char)random.Next(startChar, endChar + 1)).ToArray());
        }
    }
}
