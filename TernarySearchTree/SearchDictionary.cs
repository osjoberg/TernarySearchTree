using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TernarySearchTree;

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
    private Node<TValue>? root;

    /// <summary>
    /// Gets the number of items in the dictionary.
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Gets if the dictionary is read-only.
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Returns all keys in the dictionary.
    /// </summary>
    public ICollection<string> Keys => new Collection<string>(Tree.GetAllKeys(root).ToArray());

    /// <summary>
    /// Returns all values in the dictionary.
    /// </summary>
    public ICollection<TValue> Values => new Collection<TValue>(Tree.GetAllValues(root).ToArray());

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
            var node = Tree.GetNodeWithValue(root, key);

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

            var leafNode = Tree.CreateNodes(ref root, key);
            if (leafNode.HasValue == false)
            {
                Count++;
            }

            leafNode.Value = value;
        }
    }

    /// <summary>
    /// Add an item to the dictionary.
    /// </summary>
    /// <param name="key">Key of the item. May not be null or an empty string.</param>
    /// <param name="value">Value to associate with the key.</param>
    public void Add(string key, TValue value)
    {
        Argument.IsNotNullAndNotEmpty(key, nameof(key));

        var node = Tree.CreateNodes(ref root, key);
        if (node.HasValue)
        {
            throw new ArgumentException("An item with the same key has already been added to the dictionary.", nameof(key));
        }

        Count++;
        node.Value = value;
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
        return Tree.GetAllKeyValuePairs(root).GetEnumerator();
    }

    /// <summary>
    /// Contains key.
    /// </summary>
    /// <param name="key">Key to get.</param>
    /// <returns>True if key was found and false if the key could not be found.</returns>
    public bool ContainsKey(string key)
    {
        Argument.IsNotNullAndNotEmpty(key, nameof(key));

        var node = Tree.GetNodeWithValue(root, key);
        return node != null && node.HasValue;
    }

    /// <summary>
    /// Determines if the dictionary contains the given item.
    /// </summary>
    /// <param name="item">Key value pair to locate in the collection.</param>
    /// <returns>True if item is found and false if the item could not be found.</returns>
    public bool Contains(KeyValuePair<string, TValue> item)
    {
        Argument.IsNotNull(item, nameof(item));
        Argument.IsNotNullAndNotEmpty(item.Key, nameof(item));

        var node = Tree.GetNodeWithValue(root, item.Key);

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

        var node = Tree.GetNodeWithValue(root, key);
        if (node == null || node.HasValue == false)
        {
            value = default!;
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

        var node = Tree.GetNode(root, startOfKey);
        if (node == null)
        {
            yield break;
        }

        if (node.HasValue)
        {
            yield return node.Value;
        }

        foreach (var value in Tree.GetAllValues(node.EqualNode))
        {
            yield return value;
        }
    }

    /// <summary>
    /// Gets all items starting with the specified key, allowing a specified edit distance to be matched.
    /// A distance is noted as either an insert, edit or delete.
    /// Note that items whose keys have a large distance will be returned if they are long and starts with the specified key.
    /// </summary>
    /// <param name="startOfKey">Start of keys to match.</param>
    /// <param name="maxEditDistance">Maximum edit distance to match.</param>
    /// <returns>List of values matching key, with their edit distance towards the search key.</returns>
    public IEnumerable<SearchResult<TValue>> StartsWith(string startOfKey, int maxEditDistance)
    {
        Argument.IsNotNullAndNotEmpty(startOfKey, nameof(startOfKey));
        Argument.IsWithinRange(maxEditDistance >= 0, nameof(maxEditDistance));

        if (root == null)
        {
            yield break;
        }

        var keyLength = startOfKey.Length;
        var arrayLength = startOfKey.Length + 1;

        // Pool of arrays so that arrays for the Levenshtein algorithm can be resued.
        var arrayPool = new Stack<int[]>();
        int[] RentArray() => arrayPool.TryPop(out var array) ? array : new int[arrayLength];
        void ReturnArray(int[] array) => arrayPool.Push(array);
        
        // Initialize a Levenshtein table with no consumption (i.e.only 1 row).
        var init = RentArray();
        for (var index = 0; index < arrayLength; index++)
        {
            init[index] = index;
        }

        var stack = new Stack<(Node<TValue> node, int[] row)>();
        stack.Push((root, init));

        var valueStack = new Stack<Node<TValue>>();

        while (stack.TryPop(out var state))
        {
            var (node, previousRow) = state;

            // Traverse side branches without advancing along the path.
            // They are on a step earlier from us (consuming them will cost), so it is free to check them, result in reuse of levenshtein row.
            if (node.LowerNode != null)
            {
                var copy = RentArray();
                Array.Copy(previousRow, copy, arrayLength);
                stack.Push((node.LowerNode, copy));
            }

            if (node.HigherNode != null)
            {
                var copy = RentArray();
                Array.Copy(previousRow, copy, arrayLength);
                stack.Push((node.HigherNode, copy));
            }

            // Build next row for this node's split char.
            var currentRow = RentArray();

            // The first column is increasing from the one above in the table (deletion).
            currentRow[0] = previousRow[0] + 1; 

            var minCurrentRow = currentRow[0];

            for (var keyIndex = 0; keyIndex < startOfKey.Length; keyIndex++)
            {
                var arrayIndex = keyIndex + 1;

                // Delete operations take the value above + 1.
                var delete = previousRow[arrayIndex] + 1;

                // Insert operations take the value to the left + 1.
                var insert = currentRow[keyIndex] + 1;

                // Substitution is free (compared to upper left) if there is a match (substitute a for a), otherwise it costs 1.
                var substitute = previousRow[keyIndex] + (startOfKey[keyIndex] == node.SplitCharacter ? 0 : 1); 
                
                currentRow[arrayIndex] = Math.Min(Math.Min(delete, insert), substitute);
                if (currentRow[arrayIndex] < minCurrentRow)
                {
                    minCurrentRow = currentRow[arrayIndex];
                }
            }

            // Dump the tree if the distance from the search term to the child is within the allowed distance.
            // Also dump the tree if the current node has consumed the entire key and the distance between them is within the allowed distance.
            if (currentRow[keyLength] <= maxEditDistance && (currentRow[0] == startOfKey.Length || node.HasValue))
            {
                if (node.HasValue)
                {
                    yield return new SearchResult<TValue>(node.Value, currentRow[keyLength]);
                }

                if (node.EqualNode != null)
                {
                    valueStack.Push(node.EqualNode);

                    while (valueStack.TryPop(out var valueNode))
                    {
                        if (valueNode.HasValue)
                        {
                            yield return new SearchResult<TValue>(valueNode.Value, currentRow[keyLength]);
                        }

                        if (valueNode.LowerNode != null)
                        {
                            valueStack.Push(valueNode.LowerNode);
                        }

                        if (valueNode.EqualNode != null)
                        {
                            valueStack.Push(valueNode.EqualNode);
                        }

                        if (valueNode.HigherNode != null)
                        {
                            valueStack.Push(valueNode.HigherNode);
                        }
                    }
                }
            }
            else if (minCurrentRow <= maxEditDistance && node.EqualNode != null)
            {
                // Move down the tree. By sending currentRow we are telling the tree that we are consuming it.
                var copy = RentArray();
                Array.Copy(currentRow, copy, arrayLength);
                stack.Push((node.EqualNode, copy));
            }

            ReturnArray(currentRow);
            ReturnArray(previousRow);
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

        foreach (var pair in Tree.GetAllKeyValuePairs(root))
        {
            array[index++] = pair;
        }
    }

    /// <summary>
    /// Remove a key from the dictionary.
    /// </summary>
    /// <param name="key">Key to get.</param>
    /// <returns></returns>
    public bool Remove(string key)
    {
        Argument.IsNotNullAndNotEmpty(key, nameof(key));

        var node = Tree.GetNodeWithValue(root, key);
        if (node == null || node.HasValue == false)
        {
            return false;
        }

        RemoveInternal(key);
        return true;
    }

    /// <summary>
    /// Remove a key from the dictionary.
    /// </summary>
    /// <param name="item">Item to remove.</param>
    public bool Remove(KeyValuePair<string, TValue> item)
    {
        Argument.IsNotNull(item, nameof(item));
        Argument.IsNotNullAndNotEmpty(item.Key, nameof(item));

        var node = Tree.GetNodeWithValue(root, item.Key);
        if (node == null || node.HasValue == false || EqualityComparer<TValue>.Default.Equals(node.Value, item.Value) == false)
        {
            return false;
        }

        RemoveInternal(item.Key);
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

    public void Optimize()
    {
        foreach (var node in Tree.GetAllNodes(root))
        {
            if (node.EqualNode != null)
            {
                var newEqualNode = Tree.OptimizeEqualNode(node.EqualNode);
                node.EqualNode = newEqualNode;
            }
        }

        root = Tree.OptimizeEqualNode(root);
    }

    private void RemoveInternal(string key)
    {
        Tree.RemoveNode(root, key, 0);
        if (root!.CanBeRemoved)
        {
            root = null;
        }

        Count--;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}