using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace TernarySearchTree.Benchmark
{
    [MinIterationCount(7)]
    [MaxIterationCount(10)]
    public class DictionaryBenchmark
    {
        private static readonly string[] keys = Keygenerator.GenerateKeys(42, 20, '0', 'z').Distinct().Take(10000).ToArray();
        private string[] keysInRandomOrder;

        private Dictionary<string, int> dictionary = new Dictionary<string, int>();
        private SearchDictionary<int> searchDictionary = new SearchDictionary<int>();
        private SearchDictionary<int> searchDictionaryOptimized = new SearchDictionary<int>();

        [GlobalSetup]
        public virtual void Setup()
        {
            keysInRandomOrder = keys.ToArray();
            new Random(42).Shuffle(keysInRandomOrder);

            foreach (var key in keys)
            {
                dictionary.Add(key, 0);
                searchDictionary.Add(key, 0);
                searchDictionaryOptimized.Add(key, 0);
            }

            searchDictionaryOptimized.Optimize();
        }

        [Benchmark]
        public int Dictionary_Add()
        {
            var insertDictionary = new Dictionary<string, int>();

            foreach (var key in keys)
            {
                insertDictionary.Add(key, 0);
            }

            return insertDictionary.Count;
        }

        [Benchmark]
        public int SearchDictionary_Add()
        {
            var insertSearchDictionary = new SearchDictionary<int>();

            foreach (var key in keys)
            {
                insertSearchDictionary.Add(key, 0);
            }

            return insertSearchDictionary.Count;
        }

        [Benchmark]
        public int Dictionary_Lookup()
        {
            var sum = 0;

            foreach (var key in keysInRandomOrder)
            {
                sum += dictionary[key];
            }

            return sum;
        }

        [Benchmark]
        public int SearchDictionary_Lookup()
        {
            var sum = 0;

            foreach (var key in keysInRandomOrder)
            {
                sum += searchDictionary[key];
            }

            return sum;
        }

        [Benchmark]
        public int SearchDictionaryOptimized_Lookup()
        {
            var sum = 0;

            foreach (var key in keysInRandomOrder)
            {
                sum += searchDictionaryOptimized[key];
            }

            return sum;
        }

        [Benchmark]
        public int Dictionary_StartsWith()
        {
            var sum = 0;
            return sum;
        }

        [Benchmark]
        public int SearchDictionary_StartsWith()
        {
            var sum = 0;
            foreach (var key in keysInRandomOrder)
            {
                sum += searchDictionary.StartsWith(key).Sum();
            }

            return sum;
        }


        [Benchmark]
        public int SearchDictionaryOptimized_StartsWith()
        {
            var sum = 0;
            foreach (var key in keysInRandomOrder)
            {
                sum += searchDictionaryOptimized.StartsWith(key).Sum();
            }

            return sum;
        }
        
        [Benchmark]
        public int SearchDictionary_NearSearch()
        {
            var sum = 0;
            foreach (var key in keysInRandomOrder.ToArray())
            {
                sum += searchDictionary.NearSearch(key, 4).Select(v => v.Value).Sum();
            }

            return sum;
        }
    }
}
