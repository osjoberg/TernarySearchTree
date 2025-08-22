namespace TernarySearchTree;

internal class Node<TValue>
{
    private TValue value;

    public Node(char splitCharacter)
    {
        value = default!;
        SplitCharacter = splitCharacter;
    }

    public char SplitCharacter { get; private set; }

    public Node<TValue>? HigherNode { get; set; }

    public Node<TValue>? EqualNode { get; set; }

    public Node<TValue>? LowerNode { get; set; }

    public bool HasValue { get; private set; }

    public TValue Value
    {
        get => value;
        set
        {
            this.value = value;
            HasValue = true;
        }
    }

    internal void ClearValue()
    {
        value = default!;
        HasValue = false;
    }

    public bool CanBeRemoved => HigherNode == null && LowerNode == null && EqualNode == null && HasValue == false;

    public bool CanBeSimplified => EqualNode == null && HasValue == false && (LowerNode == null) != (HigherNode == null);
}