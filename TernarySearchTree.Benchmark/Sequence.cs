using System.Collections.Generic;

namespace TernarySearchTree.Benchmark
{
    internal static class Sequence
    {
        public static IEnumerable<T> Repeat<T>(IEnumerable<T> sequence, int count)
        {
            for (var i = 0; i < count; i++)
            {
                foreach (var item in sequence)
                {
                    yield return item;
                }
            }
        }
    }
}
