using System;
using System.Collections.Generic;

namespace TernarySearchTree
{
    /// <summary>
    /// WildcardDictionary, implements a dictionary with a key of type String. 
    /// The Key to search for may contain wildcards. Implemented as a ternary search tree.
    /// 
    /// Based on: 
    /// http://www.codeproject.com/KB/recipes/tst.aspx
    /// http://www.cs.otago.ac.nz/cosc463/2005/ternary.c
    /// http://www.cs.otago.ac.nz/cosc463/2005/ternary.pdf
    /// </summary>
    /// <typeparam name="TValue">Type of value.</typeparam>
    public class SearchDictionary<TValue>
    {
        /// <summary>
        /// Root node.
        /// </summary>
        private Node<TValue> root;

        /// <summary>
        /// Gets the number of items in the dictionary.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Get/set value by key.
        /// </summary>
        /// <param name="key">Key to get or set.</param>
        /// <returns>Gets or sets the value associated with the specified key.</returns>
        public TValue this[string key]
        {
            get
            {
                // Find node.
                var wildcardDictionaryNode = FindNodeByKey(root, key);

                // If no node was found, throw exception.
                if (wildcardDictionaryNode == null)
                {
                    throw new KeyNotFoundException("The given key was not present in the dictionary.");
                }

                // Return value of node.
                return wildcardDictionaryNode.Value;
            }

            set
            {
                // Call insert.
                Insert(key, value, false);
            }
        }

        /// <summary>
        /// Add an item to the dictionary.
        /// </summary>
        /// <param name="key">Key of the item. May not be null or an empty string.</param>
        /// <param name="value">Value to associate with the key.</param>
        public void Add(string key, TValue value)
        {
            Insert(key, value, true);
        }

        /// <summary>
        /// Contains key.
        /// </summary>
        /// <param name="key">Key to get.</param>
        /// <returns>True if key was found and false if the key could not be found.</returns>
        public bool ContainsKey(string key)
        {
            var treeWildcardDictionaryNode = FindNodeByKey(root, key);
            return treeWildcardDictionaryNode != null && treeWildcardDictionaryNode.HasValue;
        }

        /// <summary>
        /// Try get value.
        /// </summary>
        /// <param name="key">Key to get.</param>
        /// <param name="value">Value if the value can be found.</param>
        /// <returns>True if key was found and false if the key could not be found.</returns>
        public bool TryGetValue(string key, out TValue value)
        {
            var node = FindNodeByKey(root, key);
            if (node == null)
            {
                value = default(TValue);
                return false;
            }

            value = node.Value;
            return true;
        }

        /// <summary>
        /// Removes all items in the dictionary.
        /// </summary>
        public void Clear()
        {
            root = null;
            Count = 0;
        }

        /// <summary>
        /// Gets all items starting with the specified key.
        /// </summary>
        /// <param name="startOfKey">Start of keys to match.</param>
        /// <returns>List of values matching key.</returns>
        public IEnumerable<TValue> StartsWith(string startOfKey)
        {
            var node = FindNodeByKey(root, startOfKey);

            if (node == null)
            {
                yield break;
            }

            yield return node.Value;

            foreach (var value in GetAllValues(node.EqualNode))
            {
                yield return value;
            }
        }

        private static IEnumerable<TValue> GetAllValues(Node<TValue> node)
        {
            if (node == null)
            {
                yield break;
            }

            if (node.HasValue)
            {
                yield return node.Value;
            }

            foreach (var lowerValue in GetAllValues(node.LowerNode))
            {
                yield return lowerValue;
            }

            foreach (var equalValue in GetAllValues(node.EqualNode))
            {
                yield return equalValue;
            }

            foreach (var higherValue in GetAllValues(node.HigherNode))
            {
                yield return higherValue;
            }
        }

        private static Node<TValue> FindNodeByKey(Node<TValue> node, string key)
        {
            // Setup current key index and character.
            var currentKeyCharacterIndex = 0;
            var currentKeyCharacter = key[currentKeyCharacterIndex];

            // Loop while we have a node reference.
            while (node != null)
            {
                if (currentKeyCharacter < node.SplitCharacter)
                {
                    // Set current node to lower node.
                    node = node.LowerNode;
                }
                else if (currentKeyCharacter > node.SplitCharacter)
                {
                    // Set current node to higher node.
                    node = node.HigherNode;
                }
                else
                {
                    // If we have processed the whole key, return node information if it has a value.
                    if (++currentKeyCharacterIndex == key.Length)
                    {
                        return node;
                    }

                    // Advance to next split character.
                    currentKeyCharacter = key[currentKeyCharacterIndex];

                    // Set current node to equal node.
                    node = node.EqualNode;
                }
            }

            // If we get here there was no nodes to traverse, the key could not be found.
            return null;
        }

        private void Insert(string key, TValue value, bool checkIfKeyExists)
        {
            // Validate input parameters.
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (key.Length == 0)
            {
                throw new ArgumentException("Key can not be an empty string.", nameof(key));
            }

            // Setup current key index and character.
            var currentKeyCharacterIndex = 0;
            var currentKeyCharacter = key[currentKeyCharacterIndex];

            // Create a root node if it does not exist.
            if (root == null)
            {
                root = new Node<TValue>(currentKeyCharacter);
            }

            // Pre-calculate key length that is one character less than the actual key length.
            var keyLengthMinusOne = key.Length - 1;

            // Get current node.
            var currentWildcardDictionaryNode = root;

            // Loop until we have added the key and value.
            for (;;)
            {
                if (currentKeyCharacter < currentWildcardDictionaryNode.SplitCharacter)
                {
                    // If no lower node exists, create a new lower node.
                    if (currentWildcardDictionaryNode.LowerNode == null)
                    {
                        currentWildcardDictionaryNode.LowerNode = new Node<TValue>(currentKeyCharacter);
                    }

                    // Set current node to lower node.
                    currentWildcardDictionaryNode = currentWildcardDictionaryNode.LowerNode;
                }                
                else if (currentKeyCharacter > currentWildcardDictionaryNode.SplitCharacter)
                {
                    // If no higher node exists, create a new higher node.
                    if (currentWildcardDictionaryNode.HigherNode == null)
                    {
                        currentWildcardDictionaryNode.HigherNode = new Node<TValue>(currentKeyCharacter);
                    }

                    // Set current node to higher node.
                    currentWildcardDictionaryNode = currentWildcardDictionaryNode.HigherNode;
                }
                else
                {
                    // If we havent processed all split characters.
                    if (currentKeyCharacterIndex < keyLengthMinusOne)
                    {
                        // Advance to next split character.
                        currentKeyCharacter = key[++currentKeyCharacterIndex];

                        // Create new equal node if it does not exist.
                        if (currentWildcardDictionaryNode.EqualNode == null)
                        {
                            currentWildcardDictionaryNode.EqualNode = new Node<TValue>(currentKeyCharacter);
                        }

                        // Set current node to equal node.
                        currentWildcardDictionaryNode = currentWildcardDictionaryNode.EqualNode;
                    }
                    else
                    {
                        // Make sure current node does not already have a value.
                        if (checkIfKeyExists && currentWildcardDictionaryNode.HasValue)
                        {
                            throw new ArgumentException("An item with the same key has already been added to the dictionary.", nameof(key));
                        }

                        // Increase count.
                        if (!currentWildcardDictionaryNode.HasValue)
                        {
                            Count++;
                        }

                        // Set the value of the current node.
                        currentWildcardDictionaryNode.Value = value;
                        return;
                    }
                }
            }
        }
    }
}