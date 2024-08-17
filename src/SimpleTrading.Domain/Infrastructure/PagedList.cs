using System.Collections;

namespace SimpleTrading.Domain.Infrastructure;

public class PagedList<T>(IEnumerable<T> items, int totalCount, int page, int pageSize)
    : IReadOnlyList<T>
{
    private readonly IList<T> _subset = items as IList<T> ?? new List<T>(items);

    public int Page { get; } = page;
    public int PageSize { get; } = pageSize;

    public int TotalPages { get; } = (int) Math.Ceiling(totalCount / (double) pageSize);

    public bool IsFirstPage => Page == 1;

    public bool IsLastPage => Page == TotalPages;

    public int TotalCount { get; } = totalCount;

    /// <summary>
    ///     Count of current page
    /// </summary>
    public int Count => _subset.Count;

    public T this[int index] => _subset[index];

    public IEnumerator<T> GetEnumerator()
    {
        return _subset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}