using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TernarySearchTree.Test
{
    [TestClass]
    public class FuzzySearchDictionaryTest
    {
        [TestMethod]
        public void Adding10000RanomKeysWork()
        {
            var randomKeys = Keygenerator.GenerateKeys(42, 20, '0', 'z', 10000);

            var searchDictionary = new SearchDictionary<int>();

            foreach (var ranomKey in randomKeys)
            {
                searchDictionary.Add(ranomKey, 0);
            }

            Assert.AreEqual(randomKeys.Count, searchDictionary.Count);
            foreach (var ranomKey in randomKeys)
            {
                Assert.IsTrue(searchDictionary.ContainsKey(ranomKey));
            }
        }

        [TestMethod]
        public void Adding10000RanomKeysAndRemovingHalfWorks()
        {
            var randomKeys = Keygenerator.GenerateKeys(49, 20, '0', 'z', 100000);

            var searchDictionary = new SearchDictionary<int>();

            foreach (var ranomKey in randomKeys)
            {
                searchDictionary.Add(ranomKey, 0);
            }

            int i = 0;
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

            searchDictionary.Optimize();

            foreach (var randomKey in randomKeys)
            {
                Assert.IsTrue(searchDictionary.ContainsKey(randomKey), $"ContainsKey(\"{randomKey}\"");
            }
        }
    }
}
