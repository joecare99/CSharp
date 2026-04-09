namespace RepoMigrator.Tools.PipelinedMigration;

/// <summary>
/// Buffers work items that may arrive out of order and releases them in the expected order.
/// </summary>
/// <typeparam name="TItem">The buffered item type.</typeparam>
public sealed class OrderedWorkItemBuffer<TItem>
{
    private readonly SortedDictionary<int, TItem> _dctBufferedItems = [];

    /// <summary>
    /// Adds an item for the specified index.
    /// </summary>
    /// <param name="iIndex">The zero-based order index.</param>
    /// <param name="item">The buffered item.</param>
    public void Add(int iIndex, TItem item)
    {
        if (_dctBufferedItems.ContainsKey(iIndex))
            throw new InvalidOperationException($"An item for index {iIndex} has already been buffered.");

        _dctBufferedItems[iIndex] = item;
    }

    /// <summary>
    /// Attempts to remove the item for the next expected index.
    /// </summary>
    /// <param name="iExpectedIndex">The expected zero-based index.</param>
    /// <param name="item">The buffered item if available.</param>
    /// <returns><see langword="true" /> when the next item was available; otherwise <see langword="false" />.</returns>
    public bool TryTakeNext(int iExpectedIndex, out TItem? item)
    {
        if (_dctBufferedItems.Remove(iExpectedIndex, out item))
            return true;

        item = default;
        return false;
    }

    /// <summary>
    /// Returns the buffered items.
    /// </summary>
    /// <returns>The currently buffered items.</returns>
    public IReadOnlyCollection<TItem> GetBufferedItems()
        => _dctBufferedItems.Values.ToList();
}
