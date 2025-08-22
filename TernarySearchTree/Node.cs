namespace TernarySearchTree;

internal class Node<TValue>
{
    private TValue value;

#pragma warning disable CS8618
    public Node(char splitCharacter)
    {
        SplitCharacter = splitCharacter;
    }
#pragma warning restore CS8601

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
#pragma warning disable CS8601
        value = default;
#pragma warning restore CS8601
        HasValue = false;
    }

    public bool CanBeRemoved => HigherNode == null && LowerNode == null && EqualNode == null && HasValue == false;

    public bool CanBeSimplified => EqualNode == null && HasValue == false && (LowerNode == null) != (HigherNode == null);
}