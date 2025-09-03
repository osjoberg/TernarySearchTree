using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: Parallelize(Workers = 0, Scope = ExecutionScope.MethodLevel)]

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
                new("a", "a"), 
                new("aa", "aa"), 
                new("c", "c"), 
                new("b", "b"), 
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
            var dictionary = new SearchDictionary<string>();
            const string key = "Test";

            // Test that the key does exist.
            Assert.IsFalse(dictionary.ContainsKey(key));

            dictionary.Add(key, "Data");

            // Test that the key exist.
            Assert.IsTrue(dictionary.ContainsKey(key));
            Assert.IsFalse(dictionary.ContainsKey(key.Substring(0, 3)));
            Assert.IsFalse(dictionary.ContainsKey(key + "t"));
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetThis()
        {
            var dictionary = new SearchDictionary<object>();
            const string ley = "Test";
            var object1 = new object();
            var object2 = new object();

            dictionary.Add(ley + 1, object1);
            dictionary.Add(ley + 2, object2);

            // Test that key1 maps to object1 and key2 maps to object2.
            Assert.AreEqual(object1, dictionary[ley + 1]);
            Assert.AreEqual(object2, dictionary[ley + 2]);

            // Test that KeyNotFoundException is thrown.
            var obj = dictionary[ley + 3];
        }

        [TestMethod]
        public void GetCount()
        {
            var dictionary = new SearchDictionary<string>();
            const string key = "Test";
            const int count = 34;

            // Test that the dictionary count is 0.
            Assert.AreEqual(dictionary.Count, 0);

            for (var i = 1; i <= count; i++)
            {
                dictionary.Add(key + i, "");

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
        public void StartsWithEditDistance()
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

            var result = dictionary.StartsWith("lurar", 3).OrderBy(v => v.Value).ToArray();

            CollectionAssert.AreEqual(new[]
            {
                new SearchResult<string>("hörlurarochannat", 3, "hörlurarochannat"),
                new SearchResult<string>("hörlurar", 3, "hörlurar"),
                new SearchResult<string>("höglurar", 3, "höglurar"),
                new SearchResult<string>("lurarna", 0, "lurarna"),
                new SearchResult<string>("lurer", 1, "lurer"),
                new SearchResult<string>("lurar", 0, "lurar"),
                new SearchResult<string>("lurur", 1, "lurur"),
                new SearchResult<string>("lugercheck", 2, "lugercheck"),
            }.OrderBy(v => v.Value).ToArray(), result);
            
            var resultWithDelete = new SearchDictionary<string>
            {
                { "lurcheck", "lurcheck" },
                { "luarcheck", "luarcheck" }
            }.StartsWith("lurarcheck", 1).OrderBy(v => v.Value).ToArray();
            CollectionAssert.AreEqual(new[] { new SearchResult<string>("luarcheck", 1, "luarcheck") }.OrderBy(v => v.Value).ToArray(), resultWithDelete);
        }

        [TestMethod]
        public void StartsWithEditDistance2()
        {
            var resultsWithShortWords = new SearchDictionary<string>
            {
                { "4", "4" },
                { "44", "44" }
            }.StartsWith("sekrita", 4);

            Assert.AreEqual(0, resultsWithShortWords.Count());
        }

        [TestMethod]
        public void SetThis()
        {
            var dictionary = new SearchDictionary<object>();
            const string key = "Test";
            var object1 = new object();
            var object2 = new object();

            dictionary[key + 1] = object2;
            dictionary[key + 2] = object1;

            // Test that the count is 2.
            Assert.AreEqual(2, dictionary.Count);

            dictionary[key + 1] = object1;
            dictionary[key + 2] = object2;

            // Test that the count is 2.
            Assert.AreEqual(2, dictionary.Count);

            // Test that key1 maps to object1 and key2 maps to object2.
            Assert.AreEqual(object1, dictionary[key + 1]);
            Assert.AreEqual(object2, dictionary[key + 2]);
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

            dictionary.Remove("d");
            dictionary.Remove("a");
            dictionary.Remove("b");
            dictionary.Remove("c");
            dictionary.Remove("aa");
            dictionary.Remove("d");

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

            dictionary.Remove("d");
            dictionary.Remove("a");
            dictionary.Remove("b");
            dictionary.Remove("c");
            dictionary.Remove("d");

            Assert.AreEqual(1, dictionary.Count);
            Assert.IsFalse(dictionary.ContainsKey("a"));
            Assert.IsFalse(dictionary.ContainsKey("b"));
            Assert.IsFalse(dictionary.ContainsKey("c"));
            Assert.IsTrue(dictionary.ContainsKey("aa"));
        }



        [TestMethod]
        public void Clear()
        {
            var dictionary = new SearchDictionary<string>();
            const string key = "Test";

            dictionary.Add(key, "data");
            dictionary.Clear();
            dictionary.Add(key, "data");
            dictionary.Clear();

            // Test that the dictionary count is 0.
            Assert.AreEqual(0, dictionary.Count);
        }
    }
}
