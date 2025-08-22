using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TernarySearchTree.Test;

[TestClass]
public class FuzzySearchDictionaryTest
{
    [TestMethod]
    public void Adding10000RandomKeysWork()
    {
        var randomKeys = Keygenerator.GenerateKeys(42, 20, '0', 'z', 10000);

        var searchDictionary = new SearchDictionary<int>();

        foreach (var randomKey in randomKeys)
        {
            searchDictionary.Add(randomKey, 0);
        }

        Assert.AreEqual(randomKeys.Count, searchDictionary.Count);
        foreach (var randomKey in randomKeys)
        {
            Assert.IsTrue(searchDictionary.ContainsKey(randomKey));
        }
    }

    [TestMethod]
    public void Adding10000RandomKeysAndRemovingHalfWorks()
    {
        var randomKeys = Keygenerator.GenerateKeys(49, 20, '0', 'z', 100000);

        var searchDictionary = new SearchDictionary<int>();

        foreach (var randomKey in randomKeys)
        {
            searchDictionary.Add(randomKey, 0);
        }

        var i = 0;
        foreach (var randomKey in randomKeys)
        {
            if (i % 2 == 0)
            {
                Assert.IsTrue(searchDictionary.Remove(randomKey), $"RemoveKey(\"{randomKey}\"");
            }

            i++;
        }

        i = 0;
        foreach (var randomKey in randomKeys)
        {
            Assert.AreEqual(i % 2 != 0, searchDictionary.ContainsKey(randomKey), $"ContainsKey(\"{randomKey}\"");
            i++;
        }

        Assert.AreEqual(randomKeys.Count / 2, searchDictionary.Count);
    }

    [TestMethod]
    public void OptimizeDoesNotDestroyTheTree()
    {
        var randomKeys = Keygenerator.GenerateKeys(57, 20, '0', 'z', 10000);

        var searchDictionary = new SearchDictionary<int>();

        foreach (var ranomKey in randomKeys)
        {
            searchDictionary.Add(ranomKey, 0);
        }

        foreach (var randomKey in randomKeys)
        {
            Assert.IsTrue(searchDictionary.ContainsKey(randomKey), $"ContainsKey(\"{randomKey}\"");
        }
    }

    [TestMethod]
    public void Adding10000RandomKeysAndStartsWithFindsRandomKeysWork()
    {
        var randomKeys = Keygenerator.GenerateKeys(57, 20, '0', 'z', 10000);
        var random = new Random(57);

        var searchDictionary = new SearchDictionary<string>();

        foreach (var randomKey in randomKeys)
        {
            searchDictionary.Add(randomKey, randomKey);
        }

        foreach (var randomKey in randomKeys)
        {
            var length = random.Next(1, randomKey.Length + 1);
            var startOfKey = randomKey.Substring(0, length);

            var foundValues = searchDictionary.StartsWith(startOfKey).OrderBy(value => value).ToArray();
            var actualValues = randomKeys.Where(key => key.StartsWith(startOfKey)).OrderBy(value => value).ToArray();

            CollectionAssert.AreEqual(foundValues, actualValues);
        }
    }

    [TestMethod]
    public void Adding10000RandomKeysAndRemoveThemWork()
    {
        var randomKeys = Keygenerator.GenerateKeys(42, 20, '0', 'z', 10000);

        var searchDictionary = new SearchDictionary<int>();

        foreach (var randomKey in randomKeys)
        {
            searchDictionary.Add(randomKey, 0);
        }

        var suffledKeys = randomKeys.ToArray();
        new Random(42).Shuffle(suffledKeys);

        foreach (var suffledKey in suffledKeys)
        {
            Assert.IsTrue(searchDictionary.Remove(suffledKey));
        }

        Assert.AreEqual(searchDictionary.Count, 0);
    }
}