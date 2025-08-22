using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace TernarySearchTree.Benchmark;

public class DictionaryBenchmark
{
    private static readonly string[] Keys = Keygenerator.GenerateKeys(42, 20, '0', 'z').Distinct().Take(10000).ToArray();
    private static readonly string[] KeysInRandomOrder = Keys.ToArray();

    private readonly Dictionary<string, int> dictionary = new Dictionary<string, int>();
    private readonly SearchDictionary<int> searchDictionary = new SearchDictionary<int>();
    private readonly SearchDictionary<int> searchDictionaryOptimized = new SearchDictionary<int>();

    [GlobalSetup]
    public virtual void Setup()
    {
        new Random(42).Shuffle(KeysInRandomOrder);

        foreach (var key in Keys)
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

        foreach (var key in Keys)
        {
            insertDictionary.Add(key, 0);
        }

        return insertDictionary.Count;
    }

    [Benchmark]
    public int SearchDictionary_Add()
    {
        var insertSearchDictionary = new SearchDictionary<int>();

        foreach (var key in Keys)
        {
            insertSearchDictionary.Add(key, 0);
        }

        return insertSearchDictionary.Count;
    }

    [Benchmark]
    public int Dictionary_Lookup()
    {
        var sum = 0;

        foreach (var key in KeysInRandomOrder)
        {
            sum += dictionary[key];
        }

        return sum;
    }

    [Benchmark]
    public int SearchDictionary_Lookup()
    {
        var sum = 0;

        foreach (var key in KeysInRandomOrder)
        {
            sum += searchDictionary[key];
        }

        return sum;
    }

    [Benchmark]
    public int SearchDictionaryOptimized_Lookup()
    {
        var sum = 0;

        foreach (var key in KeysInRandomOrder)
        {
            sum += searchDictionaryOptimized[key];
        }

        return sum;
    }

    [Benchmark]
    public int SearchDictionary_StartsWith()
    {
        var sum = 0;
        foreach (var key in KeysInRandomOrder)
        {
            sum += searchDictionary.StartsWith(key).Sum();
        }

        return sum;
    }


    [Benchmark]
    public int SearchDictionaryOptimized_StartsWith()
    {
        var sum = 0;
        foreach (var key in KeysInRandomOrder)
        {
            sum += searchDictionaryOptimized.StartsWith(key).Sum();
        }

        return sum;
    }
}