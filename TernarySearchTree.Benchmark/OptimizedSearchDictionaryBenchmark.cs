using System.Linq;
using BenchmarkDotNet.Attributes;

namespace TernarySearchTree.Benchmark
{
    public class OptimizedSearchDictionaryBenchmark : DictionaryBenchmark
    {
        private SearchDictionary<int> searchDictionary;

        public OptimizedSearchDictionaryBenchmark() : base(() => new SearchDictionary<int>())
        {            
        }

        [Setup]
        public override void Setup()
        {
            base.Setup();
            searchDictionary = (SearchDictionary<int>)Dictionary;
            searchDictionary.Optimize();
        }

        [Benchmark]
        public override int StartsWith()
        {
            return searchDictionary.StartsWith(RepeatedKeys[KeyIndex++]).Sum();
        }
    }
}
