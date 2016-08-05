using System.Collections.Generic;

namespace TernarySearchTree
{
    internal static class Tree
    {
        internal static Node<TValue> GetNode<TValue>(Node<TValue> node, string key)
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
                        return node.HasValue ? node : null;
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

        internal static Node<TValue> CreateNodes<TValue>(ref Node<TValue> root, string key)
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
                else if (++currentKeyCharacterIndex < key.Length)
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
                    return node;
                }
            }
        }

        internal static IEnumerable<string> GetAllKeys<TValue>(Node<TValue> node, string key)
        {
            if (node == null)
            {
                yield break;
            }

            if (node.HasValue)
            {
                yield return key + node.SplitCharacter;
            }

            foreach (var lowerValueKey in GetAllKeys(node.LowerNode, key))
            {
                yield return lowerValueKey;
            }

            foreach (var equalValueKey in GetAllKeys(node.EqualNode, key + node.SplitCharacter))
            {
                yield return equalValueKey;
            }

            foreach (var higherValueKey in GetAllKeys(node.HigherNode, key))
            {
                yield return higherValueKey;
            }
        }

        internal static IEnumerable<TValue> GetAllValues<TValue>(Node<TValue> node)
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

        internal static IEnumerable<KeyValuePair<string, TValue>> GetAllKeyValuePairs<TValue>(Node<TValue> node, string key)
        {
            if (node == null)
            {
                yield break;
            }

            if (node.HasValue)
            {
                yield return new KeyValuePair<string, TValue>(key + node.SplitCharacter, node.Value);
            }

            foreach (var lowerKeyValuePair in GetAllKeyValuePairs(node.LowerNode, key))
            {
                yield return lowerKeyValuePair;
            }

            foreach (var equalKeyValuePair in GetAllKeyValuePairs(node.EqualNode, key + node.SplitCharacter))
            {
                yield return equalKeyValuePair;
            }

            foreach (var higherKeyValuePair in GetAllKeyValuePairs(node.HigherNode, key))
            {
                yield return higherKeyValuePair;
            }
        }

        internal static bool RemoveNode<TValue>(Node<TValue> node, string key, int keyIndex)
        {
            var currentKeyCharacter = key[keyIndex];

            if (currentKeyCharacter < node.SplitCharacter && RemoveNode(node.LowerNode, key, keyIndex))
            {
                node.LowerNode = null;
                return node.CanBeRemoved;
            }

            if (currentKeyCharacter > node.SplitCharacter && RemoveNode(node.HigherNode, key, keyIndex))
            {
                node.HigherNode = null;
                return node.CanBeRemoved;
            }

            if (keyIndex < key.Length - 1 && RemoveNode(node.EqualNode, key, keyIndex + 1))
            {
                node.EqualNode = null;
                return node.CanBeRemoved;
            }

            if (node.HasValue == false)
            {
                return false;
            }

            node.ClearValue();
            return node.CanBeRemoved;
        }
    }
}
