namespace TernarySearchTree;

public struct SearchResult<TValue>(TValue value, int editDistance, string key)
{
    public TValue Value { get; } = value;

    public int EditDistance { get; } = editDistance;
    
    public string Key { get; } = key;
}
