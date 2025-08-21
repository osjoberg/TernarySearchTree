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
            var dictionary = new SearchDictionary<string>
            {
                { "Test", "Data" }
            };

            // Test that ArgumentException is thrown.
            dictionary.Add("Test", "Data");
        }

        [TestMethod]
        public void ValuesPropertyReturnsAllValues()
        {
            var dictionary = new SearchDictionary<string>
            {
                { "a", "a" }, 
                { "aa", "aa" }, 
                { "c", "c" }, 
                { "b", "b" }
            };

            CollectionAssert.AreEqual(new[] { "a", "aa", "b", "c" }, dictionary.Values.OrderBy(value => value).ToArray());
        }

        [TestMethod]
        public void KeysPropertyReturnsAllKeys()
        {
            var dictionary = new SearchDictionary<string>
            {
                { "a", "a" }, 
                { "aa", "aa" }, 
                { "c", "c" }, 
                { "b", "b" }
            };

            CollectionAssert.AreEqual(new[] { "a", "aa", "b", "c" }, dictionary.Keys.OrderBy(key => key).ToArray());
        }

        [TestMethod]
        public void AllKeyValuePairsCanBeEnumerated()
        {
            var allItems = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("a", "a"), 
                new KeyValuePair<string, string>("aa", "aa"), 
                new KeyValuePair<string, string>("c", "c"), 
                new KeyValuePair<string, string>("b", "b"), 
            };

            var dictionary = new SearchDictionary<string>();
            foreach (var item in allItems)
            {
                dictionary.Add(item);
            }

            CollectionAssert.AreEqual(allItems.OrderBy(item => item.Key).ToArray(), dictionary.OrderBy(item => item.Key).ToArray());
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
            var obj = dictionary[Key + 3];
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
        public void StartsWith()
        {
            var dictionary = new SearchDictionary<string>
            {
                { "testa", "testa" }, 
                { "testb", "testb" }, 
                { "test", "test" }, 
                { "tesk", "tesk" }
            };

            CollectionAssert.AreEqual(new[] { "testa", "testb", "test" }.OrderBy(v => v).ToArray(), dictionary.StartsWith("test").OrderBy(v => v).ToArray());
        }

        [TestMethod]
        public void NearSearch()
        {
            var dictionary = new SearchDictionary<string>
            {
                { "hörlurarochannat", "hörlurarochannat" },
                { "hörlurar", "hörlurar" }, 
                { "högtalare", "högtalare" },
                { "höglurar", "höglurar" },
                { "höst", "höst" },
                { "lurarna", "lurarna" },
                { "lurer", "lurer" },
                { "lurar", "lurar" },
                { "lurur", "lurur" },
                { "lugercheck", "lugercheck" },
            };

            var result = dictionary.NearSearch("lurar", 3).OrderBy(v => v.Value).ToArray();

            CollectionAssert.AreEqual(new[]
            {
                new DistancedValue<string>(3, "hörlurarochannat"),
                new DistancedValue<string>(3, "hörlurar"),
                new DistancedValue<string>(3, "höglurar"),
                new DistancedValue<string>(0, "lurarna"),
                new DistancedValue<string>(1, "lurer"),
                new DistancedValue<string>(0, "lurar"),
                new DistancedValue<string>(1, "lurur"),
                new DistancedValue<string>(2, "lugercheck"),
            }.OrderBy(v => v.Value).ToArray(), result);
            
            var resultWithDelete = new SearchDictionary<string>
            {
                { "lurcheck", "lurcheck" },
                { "luarcheck", "luarcheck" }
            }.NearSearch("lurarcheck", 1).OrderBy(v => v.Value).ToArray();
            CollectionAssert.AreEqual(new[] { new DistancedValue<string>(1, "luarcheck") }.OrderBy(v => v.Value).ToArray(), resultWithDelete);
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
        public void RemoveRemovesAllKeysProperly()
        {
            var dictionary = new SearchDictionary<string>
            {
                { "a", "a" }, 
                { "aa", "aa" }, 
                { "c", "c" }, 
                { "b", "b" }
            };

            dictionary.Remove("a");
            dictionary.Remove("b");
            dictionary.Remove("c");
            dictionary.Remove("aa");

            Assert.AreEqual(0, dictionary.Count);
            Assert.IsFalse(dictionary.ContainsKey("a"));
            Assert.IsFalse(dictionary.ContainsKey("b"));
            Assert.IsFalse(dictionary.ContainsKey("c"));
            Assert.IsFalse(dictionary.ContainsKey("aa"));
        }

        [TestMethod]
        public void RemoveRemovesKeysProperly()
        {
            var dictionary = new SearchDictionary<string>
            {
                { "a", "a" }, 
                { "aa", "aa" }, 
                { "c", "c" }, 
                { "b", "b" }
            };

            dictionary.Remove("a");
            dictionary.Remove("b");
            dictionary.Remove("c");

            Assert.AreEqual(1, dictionary.Count);
            Assert.IsFalse(dictionary.ContainsKey("a"));
            Assert.IsFalse(dictionary.ContainsKey("b"));
            Assert.IsFalse(dictionary.ContainsKey("c"));
            Assert.IsTrue(dictionary.ContainsKey("aa"));
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
    }
}
