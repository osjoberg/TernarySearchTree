namespace TernarySearchTree;

public readonly struct SearchResult<TValue>(string key, TValue value, int editDistance)
{
    public TValue Value { get; } = value;

    public int EditDistance { get; } = editDistance;
    
    public string Key { get; } = key;
}
