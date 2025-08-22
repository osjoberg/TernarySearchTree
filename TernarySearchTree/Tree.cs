using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace TernarySearchTree;

internal static class Tree
{
    internal static void GetSiblings<TValue>(Node<TValue> node, List<Node<TValue>> siblings)
    {
        siblings.Add(node);

        if (node.LowerNode != null)
        {
            GetSiblings(node.LowerNode, siblings);
        }

        if (node.HigherNode != null)
        {
            GetSiblings(node.HigherNode, siblings);
        }
    }

    internal static Node<TValue>? OptimizeEqualNode<TValue>(Node<TValue>? equalNode)
    {
        if (equalNode == null)
        {
            return null;
        }

        var siblings = new List<Node<TValue>>();
        GetSiblings(equalNode, siblings);
        siblings.Sort(new SplitCharacterComparer<TValue>());

        return OptimizeSiblings(siblings, 0, siblings.Count);
    }

    internal static Node<TValue>? OptimizeSiblings<TValue>(List<Node<TValue>> nodes, int index, int count)
    {
        if (count == 0)
        {
            return null;
        }

        if (count == 1)
        {
            var leafNode = nodes[index];
            leafNode.LowerNode = null;
            leafNode.HigherNode = null;
            return leafNode;
        }

        var middleNode = nodes[index + count / 2];
        middleNode.LowerNode = OptimizeSiblings(nodes, index, count / 2);
        middleNode.HigherNode = OptimizeSiblings(nodes, index + count / 2 + 1, (count - 1) / 2);
        return middleNode;
    }

    internal static Node<TValue>? GetNodeWithValue<TValue>(Node<TValue>? node, string key)
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


        // If we get here there were no nodes to traverse, the key could not be found.
        return null;
    }

    internal static Node<TValue>? GetNode<TValue>(Node<TValue>? node, string key)
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

        // If we get here there were no nodes to traverse, the key could not be found.
        return null;
    }

    internal static Node<TValue> CreateNodes<TValue>(ref Node<TValue>? root, string key)
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
                node.LowerNode ??= new Node<TValue>(currentKeyCharacter);

                // Set current node to lower node.
                node = node.LowerNode;
            }
            else if (currentKeyCharacter > node.SplitCharacter)
            {
                // If no higher node exists, create a new higher node.
                node.HigherNode ??= new Node<TValue>(currentKeyCharacter);

                // Set current node to higher node.
                node = node.HigherNode;
            }
            else if (++currentKeyCharacterIndex < key.Length)
            {
                // Advance to next split character.
                currentKeyCharacter = key[currentKeyCharacterIndex];

                // Create new equal node if it does not exist.
                node.EqualNode ??= new Node<TValue>(currentKeyCharacter);

                // Set current node to equal node.
                node = node.EqualNode;
            }
            else
            {
                return node;
            }
        }
    }

    internal static IEnumerable<string> GetAllKeys<TValue>(Node<TValue>? root)
    {
        if (root == null)
        {
            yield break;
        }

        var stack = new Stack<(Node<TValue>, string)>();
        stack.Push((root, ""));

        while (stack.TryPop(out var pair))
        {
            var (node, key) = pair;

            if (node.HasValue)
            {
                yield return key + node.SplitCharacter;
            }

            if (node.LowerNode != null)
            {
                stack.Push((node.LowerNode, key));
            }

            if (node.EqualNode != null)
            {
                stack.Push((node.EqualNode, key + node.SplitCharacter));
            }

            if (node.HigherNode != null)
            {
                stack.Push((node.HigherNode, key));
            }
        }
    }

    internal static IEnumerable<TValue> GetAllValues<TValue>(Node<TValue>? root)
    {
        if (root == null)
        {
            yield break;
        }

        var stack = new Stack<Node<TValue>>();
        stack.Push(root);

        while (stack.TryPop(out var node))
        {
            if (node.HasValue)
            {
                var node = stack.Pop();
                if (node == null)
                    continue;

                if (node.HasValue)
                    yield return node.Value;

            if (node.LowerNode != null)
            {
                stack.Push(node.LowerNode);
            }

            if (node.EqualNode != null)
            {
                stack.Push(node.EqualNode);
            }

            if (node.HigherNode != null)
            {
                stack.Push(node.HigherNode);
            }
        }
    }

    internal static IEnumerable<Node<TValue>> GetAllNodes<TValue>(Node<TValue>? root)
    {
        if (root == null)
        {
            yield break;
        }

        var stack = new Stack<Node<TValue>>();
        stack.Push(root);

        while (stack.TryPop(out var node))
        {
            yield return node;

            if (node.LowerNode != null)
            {
                stack.Push(node.LowerNode);
            }

            if (node.EqualNode != null)
            {
                stack.Push(node.EqualNode);
            }

            if (node.HigherNode != null)
            {
                stack.Push(node.HigherNode);
            }
        }
    }

    internal static IEnumerable<KeyValuePair<string, TValue>> GetAllKeyValuePairs<TValue>(Node<TValue>? root)
    {
        if (root == null)
        {
            yield break;
        }

        var stack = new Stack<(Node<TValue>, string)>();

        stack.Push((root, ""));

        while (stack.TryPop(out var pair))
        {
            var (node, key) = pair;

            if (node.HasValue)
            {
                yield return new KeyValuePair<string, TValue>(key + node.SplitCharacter, node.Value);
            }

            if (node.LowerNode != null)
            {
                stack.Push((node.LowerNode, key));
            }

            if (node.EqualNode != null)
            {
                stack.Push((node.EqualNode, key + node.SplitCharacter));
            }

            if (node.HigherNode != null)
            {
                stack.Push((node.HigherNode, key));
            }
        }
    }

    internal static bool RemoveNode<TValue>(Node<TValue>? node, string key, int keyIndex)
    {
        if (node == null)
        {
            return false;
        }

        var currentKeyCharacter = key[keyIndex];

        if (currentKeyCharacter < node.SplitCharacter && node.LowerNode != null)
        {
            if (RemoveNode(node.LowerNode, key, keyIndex))
            {
                node.LowerNode = null;
                return node.CanBeRemoved;
            }

            if (node.LowerNode.CanBeSimplified)
            {
                node.LowerNode = node.LowerNode.LowerNode ?? node.LowerNode.HigherNode;
            }
        }

        if (currentKeyCharacter > node.SplitCharacter && node.HigherNode != null)
        {
            if (RemoveNode(node.HigherNode, key, keyIndex))
            {
                node.HigherNode = null;
                return node.CanBeRemoved;
            }

            if (node.HigherNode.CanBeSimplified)
            {
                node.HigherNode = node.HigherNode.HigherNode ?? node.HigherNode.LowerNode;
            }
        }

        if (keyIndex < key.Length - 1 && currentKeyCharacter == node.SplitCharacter && node.EqualNode != null && RemoveNode(node.EqualNode, key, keyIndex + 1))
        {
            node.EqualNode = null;
            return node.CanBeRemoved;
        }

        if (keyIndex == key.Length - 1 && currentKeyCharacter == node.SplitCharacter)
        {
            node.ClearValue();
        }

        return node.CanBeRemoved;
    }
}