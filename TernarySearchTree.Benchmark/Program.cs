using BenchmarkDotNet.Running;

namespace TernarySearchTree.Benchmark
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<DictionaryBenchmark>();
        }
    }
}
