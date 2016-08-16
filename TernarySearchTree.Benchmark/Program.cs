using BenchmarkDotNet.Running;

namespace TernarySearchTree.Benchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<SearchDictionaryBenchmark>();
            BenchmarkRunner.Run<OptimizedSearchDictionaryBenchmark>();
            BenchmarkRunner.Run<DictionaryBenchmark>();
        }
    }
}
