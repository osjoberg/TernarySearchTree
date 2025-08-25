namespace TernarySearchTree;

public struct SearchResult<TValue>(TValue value, int editDistance)
{
    public TValue Value { get; } = value;

    public int EditDistance { get; } = editDistance;
}
