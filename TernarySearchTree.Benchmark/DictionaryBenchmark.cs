using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace TernarySearchTree.Benchmark
{
    public class DictionaryBenchmark
    {
        protected static readonly HashSet<string> Keys = new HashSet<string>(Keygenerator.GenerateKeys(42, 20, '0', 'z').Distinct().Take(10000));
        private readonly Func<IDictionary<string, int>> construct;

        public DictionaryBenchmark() : this(() => new Dictionary<string, int>())
        {            
        }

        protected DictionaryBenchmark(Func<IDictionary<string, int>> construct)
        {
            this.construct = construct;
        }

        [Params(/*5, 10, */20)]
        public int KeyLength { get; set; }

        protected int KeyIndex { get; set; }

        protected string[] RepeatedKeys { get; set; }

        protected virtual IDictionary<string, int> Dictionary { get; set; }

        protected virtual IDictionary<string, int> InsertDictionary { get; set; }

        [IterationSetup]
        public virtual void Setup()
        {
            Dictionary = construct();
            InsertDictionary = construct();
            foreach (var key2 in Keys)
            {
                Dictionary.Add(key2, 0);
            }

            RepeatedKeys = Sequence.Repeat(Dictionary.Keys.Where(key => key.Length == KeyLength && Keys.Contains(key)), 100).ToArray();
            KeyIndex = 0;
        }

        [Benchmark]
        public void Insert()
        {
            InsertDictionary.Add(RepeatedKeys[KeyIndex++], 0);
        }

        [Benchmark]
        public int Query()
        {
            return Dictionary[RepeatedKeys[KeyIndex++]];
        }

        [Benchmark]
        public virtual int StartsWith()
        {
            int sum = 0;
            foreach (var dictionaryKey in Dictionary.Keys)
            {
                if (dictionaryKey.StartsWith(RepeatedKeys[KeyIndex++]))
                {
                    sum += Dictionary[dictionaryKey];
                }
            }

            return sum;
        }
    }
}
