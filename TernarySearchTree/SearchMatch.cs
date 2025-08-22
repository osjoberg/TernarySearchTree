namespace TernarySearchTree;

public struct SearchMatch<TValue>(TValue value, int editDistance)
{
    public TValue Value { get; } = value;

    public int EditDistance { get; } = editDistance;

}
