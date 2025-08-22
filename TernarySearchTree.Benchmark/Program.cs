using BenchmarkDotNet.Running;

namespace TernarySearchTree.Benchmark;

public static class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<DictionaryBenchmark>();
    }
}
