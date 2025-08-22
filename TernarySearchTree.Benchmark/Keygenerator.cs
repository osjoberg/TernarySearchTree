using System;
using System.Collections.Generic;
using System.Linq;

namespace TernarySearchTree.Benchmark;

internal class Keygenerator
{
    public static IEnumerable<string> GenerateKeys(int seed, int maxKeyLength, char startChar, char endChar)
    {
        var random = new Random(seed);

        for (; ; )
        {
            var length = random.Next(1, maxKeyLength + 1);
            yield return new string(Enumerable.Range(0, length).Select(i => (char)random.Next(startChar, endChar + 1)).ToArray());
        }
    }
}