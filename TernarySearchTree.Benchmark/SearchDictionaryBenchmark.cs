using System;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace TernarySearchTree.Benchmark
{
    public class SearchDictionaryBenchmark : DictionaryBenchmark
    {
        private SearchDictionary<int> searchDictionary;

        public SearchDictionaryBenchmark() : base(() => new SearchDictionary<int>())
        {            
        }

       
        public override void Setup()
        {
            base.Setup();
            searchDictionary = (SearchDictionary<int>)Dictionary;
        }

        [Benchmark]
        public override int StartsWith()
        {
            return searchDictionary.StartsWith(RepeatedKeys[KeyIndex]).Sum();
        }
    }
}
