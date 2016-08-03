using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TernarySearchTree.Test
{
    [TestClass]
    public class SearchDictionaryTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Add()
        {
            SearchDictionary<string> dictionary = new SearchDictionary<string>();
            const string Key = "Test";

            dictionary.Add(Key, "Data");

            // Test that ArgumentException is thrown.
            dictionary.Add(Key, "Data");
        }

        [TestMethod]
        public void ContainsKey()
        {
            SearchDictionary<string> dictionary = new SearchDictionary<string>();
            const string Key = "Test";

            // Test that the key does exist.
            Assert.IsFalse(dictionary.ContainsKey(Key));

            dictionary.Add(Key, "Data");

            // Test that the key exist.
            Assert.IsTrue(dictionary.ContainsKey(Key));
            Assert.IsFalse(dictionary.ContainsKey(Key.Substring(0, 3)));
            Assert.IsFalse(dictionary.ContainsKey(Key + "t"));
        }

        [TestMethod]
        public void Clear()
        {
            SearchDictionary<string> dictionary = new SearchDictionary<string>();
            const string Key = "Test";

            dictionary.Add(Key, "data");
            dictionary.Clear();
            dictionary.Add(Key, "data");
            dictionary.Clear();

            // Test that the dictionary count is 0.
            Assert.AreEqual(0, dictionary.Count);
        }

        [TestMethod]
        public void GetCount()
        {
            SearchDictionary<string> dictionary = new SearchDictionary<string>();
            const string Key = "Test";
            const int Count = 34;

            // Test that the dictionary count is 0.
            Assert.AreEqual(dictionary.Count, 0);

            for (int i = 1; i <= Count; i++)
            {
                dictionary.Add(Key + i, null);

                // Test that the count is i.
                Assert.AreEqual(i, dictionary.Count);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetThis()
        {
            SearchDictionary<object> dictionary = new SearchDictionary<object>();
            const string Key = "Test";
            object object1 = new object();
            object object2 = new object();

            dictionary.Add(Key + 1, object1);
            dictionary.Add(Key + 2, object2);

            // Test that key1 maps to object1 and key2 maps to object2.
            Assert.AreEqual(object1, dictionary[Key + 1]);
            Assert.AreEqual(object2, dictionary[Key + 2]);

            // Test that KeyNotFoundException is thrown.
            object obj = dictionary[Key + 3];
        }

        [TestMethod]
        public void SetThis()
        {
            SearchDictionary<object> dictionary = new SearchDictionary<object>();
            const string Key = "Test";
            object object1 = new object();
            object object2 = new object();

            dictionary[Key + 1] = object2;
            dictionary[Key + 2] = object1;

            // Test that the count is 2.
            Assert.AreEqual(2, dictionary.Count);

            dictionary[Key + 1] = object1;
            dictionary[Key + 2] = object2;

            // Test that the count is 2.
            Assert.AreEqual(2, dictionary.Count);

            // Test that key1 maps to object1 and key2 maps to object2.
            Assert.AreEqual(object1, dictionary[Key + 1]);
            Assert.AreEqual(object2, dictionary[Key + 2]);
        }

        [TestMethod]
        public void StartsWith()
        {
            var dictionary = new SearchDictionary<string>();

            dictionary.Add("testa", "testa");
            dictionary.Add("testb", "testb");
            dictionary.Add("test", "test");
            dictionary.Add("tesk", "tesk");

            CollectionAssert.AreEqual(new[] { "testa", "testb", "test" }.OrderBy(v => v).ToArray(), dictionary.StartsWith("test").OrderBy(v => v).ToArray());
        }
    }
}
