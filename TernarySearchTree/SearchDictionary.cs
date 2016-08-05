using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;

namespace TernarySearchTree
{
    /// <summary>
    /// SearchDictionary, implements a dictionary with a key of type String where a partial key can be used to find values.
    /// Implemented as a ternary search tree.
    /// 
    /// Based on: 
    /// http://www.codeproject.com/KB/recipes/tst.aspx
    /// http://www.cs.otago.ac.nz/cosc463/2005/ternary.c
    /// http://www.cs.otago.ac.nz/cosc463/2005/ternary.pdf
    /// </summary>
    /// <typeparam name="TValue">Type of value.</typeparam>
    public class SearchDictionary<TValue> : IDictionary<string, TValue>
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
        /// Gets if the dictionary is read-only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Get/set value by key.
        /// </summary>
        /// <param name="key">Key to get or set.</param>
        /// <returns>Gets or sets the value associated with the specified key.</returns>
        public TValue this[string key]
        {
            get
            {
                Argument.IsNotNullAndNotEmpty(key, nameof(key));

                // Find node.
                var node = Traverse.GetNode(root, key);

                // If no node was found, throw exception.
                if (node == null)
                {
                    throw new KeyNotFoundException("The given key was not present in the dictionary.");
                }

                // Return value of node.
                return node.Value;
            }

            set
            {
                Argument.IsNotNullAndNotEmpty(key, nameof(key));

                // Call insert.
                Insert(key, value, false);
            }
        }

        /// <summary>
        /// Returns all keys in the dictionary.
        /// </summary>
        public ICollection<string> Keys => new Collection<string>(Traverse.GetAllKeys(root, "").ToArray());

        /// <summary>
        /// Returns all values in the dictionary.
        /// </summary>
        public ICollection<TValue> Values => new Collection<TValue>(Traverse.GetAllValues(root).ToArray());

        /// <summary>
        /// Add an item to the dictionary.
        /// </summary>
        /// <param name="key">Key of the item. May not be null or an empty string.</param>
        /// <param name="value">Value to associate with the key.</param>
        public void Add(string key, TValue value)
        {
            Argument.IsNotNullAndNotEmpty(key, nameof(key));

            Insert(key, value, true);
        }

        /// <summary>
        /// Add a key value pair to the dictionary.
        /// </summary>
        /// <param name="item">Key value pair to add.</param>
        public void Add(KeyValuePair<string, TValue> item)
        {
            Argument.IsNotNull(item, nameof(item));
            Argument.IsNotNullAndNotEmpty(item.Key, nameof(item));

            Add(item.Key, item.Value);
        }

        /// <summary>
        /// Get enumerator.
        /// </summary>
        /// <returns>Enumerator.</returns>
        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            return Traverse.GetAllKeyValuePairs(root, "").GetEnumerator();
        }

        /// <summary>
        /// Contains key.
        /// </summary>
        /// <param name="key">Key to get.</param>
        /// <returns>True if key was found and false if the key could not be found.</returns>
        public bool ContainsKey(string key)
        {
            Argument.IsNotNullAndNotEmpty(key, nameof(key));

            var treeWildcardDictionaryNode = Traverse.GetNode(root, key);
            return treeWildcardDictionaryNode != null && treeWildcardDictionaryNode.HasValue;
        }

        /// <summary>
        /// Determines if the dictionary contains the given item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<string, TValue> item)
        {
            Argument.IsNotNull(item, nameof(item));
            Argument.IsNotNullAndNotEmpty(item.Key, nameof(item));

            var node = Traverse.GetNode(root, item.Key);

            return node != null && node.HasValue && EqualityComparer<TValue>.Default.Equals(node.Value, item.Value);
        }

        /// <summary>
        /// Try get value.
        /// </summary>
        /// <param name="key">Key to get.</param>
        /// <param name="value">Value if the value can be found.</param>
        /// <returns>True if key was found and false if the key could not be found.</returns>
        public bool TryGetValue(string key, out TValue value)
        {
            Argument.IsNotNullAndNotEmpty(key, nameof(key));

            var node = Traverse.GetNode(root, key);
            if (node == null || node.HasValue == false)
            {
                value = default(TValue);
                return false;
            }

            value = node.Value;
            return true;
        }

        /// <summary>
        /// Gets all items starting with the specified key.
        /// </summary>
        /// <param name="startOfKey">Start of keys to match.</param>
        /// <returns>List of values matching key.</returns>
        public IEnumerable<TValue> StartsWith(string startOfKey)
        {
            Argument.IsNotNullAndNotEmpty(startOfKey, nameof(startOfKey));

            var node = Traverse.GetNode(root, startOfKey);

            if (node == null)
            {
                yield break;
            }

            if (node.HasValue)
            {
                yield return node.Value;
            }
            
            foreach (var value in Traverse.GetAllValues(node.EqualNode))
            {
                yield return value;
            }
        }

        /// <summary>
        /// Copy the dictionary to an array.
        /// </summary>
        /// <param name="array">Array to copy to.</param>
        /// <param name="index">Index in array to copy to.</param>
        public void CopyTo(KeyValuePair<string, TValue>[] array, int index)
        {
            Argument.IsNotNull(array, nameof(array));
            Argument.IsWithinRange(index >= 0 && array.Length - index >= Count, nameof(index));

            foreach (var pair in Traverse.GetAllKeyValuePairs(root, ""))
            {
                array[index++] = pair;
            }
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
        /// Remove a key from the dictionary (Not implemented).
        /// </summary>
        /// <param name="key">Key to get.</param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            throw new NotImplementedException("Remove is not yet implemented.");
        }

        /// <summary>
        /// Remove a key from the dictionary (Not implemented).
        /// </summary>
        /// <param name="item">Item to remove.</param>
        public bool Remove(KeyValuePair<string, TValue> item)
        {
            throw new NotImplementedException("Remove is not yet implemented.");
        }

        private void Insert(string key, TValue value, bool checkIfKeyExists)
        {
            // Setup current key index and character.
            var currentKeyCharacterIndex = 0;
            var currentKeyCharacter = key[currentKeyCharacterIndex];

            // Create a root node if it does not exist.
            if (root == null)
            {
                root = new Node<TValue>(currentKeyCharacter);
            }

            // Get current node.
            var node = root;

            // Loop until we have added the key and value.
            for (;;)
            {
                if (currentKeyCharacter < node.SplitCharacter)
                {
                    // If no lower node exists, create a new lower node.
                    if (node.LowerNode == null)
                    {
                        node.LowerNode = new Node<TValue>(currentKeyCharacter);
                    }

                    // Set current node to lower node.
                    node = node.LowerNode;
                }                
                else if (currentKeyCharacter > node.SplitCharacter)
                {
                    // If no higher node exists, create a new higher node.
                    if (node.HigherNode == null)
                    {
                        node.HigherNode = new Node<TValue>(currentKeyCharacter);
                    }

                    // Set current node to higher node.
                    node = node.HigherNode;
                }
                else
                {
                    // If we havent processed all split characters.
                    if (++currentKeyCharacterIndex < key.Length)
                    {
                        // Advance to next split character.
                        currentKeyCharacter = key[currentKeyCharacterIndex];

                        // Create new equal node if it does not exist.
                        if (node.EqualNode == null)
                        {
                            node.EqualNode = new Node<TValue>(currentKeyCharacter);
                        }

                        // Set current node to equal node.
                        node = node.EqualNode;
                    }
                    else
                    {
                        // Make sure current node does not already have a value.
                        if (checkIfKeyExists && node.HasValue)
                        {
                            throw new ArgumentException("An item with the same key has already been added to the dictionary.", nameof(key));
                        }

                        // Increase count.
                        if (node.HasValue == false)
                        {
                            Count++;
                        }

                        // Set the value of the current node.
                        node.Value = value;
                        return;
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}