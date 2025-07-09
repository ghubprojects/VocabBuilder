namespace VocabBuilder.Models;

public class PaginationResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public int TotalCount { get; }

    public PaginationResult(IEnumerable<T> items, int totalCount)
    {
        Items = items.ToList().AsReadOnly();
        TotalCount = totalCount;
    }
}