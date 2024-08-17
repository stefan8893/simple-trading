using SimpleTrading.Domain.Infrastructure;

namespace SimpleTrading.Domain.Extensions;

public static class PagedListExtensions
{
    public static PagedList<TResult> Select<TSource, TResult>(this PagedList<TSource> source,
        Func<TSource, TResult> selector)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(selector);

        return ExecuteDeferred(source, selector);

        static PagedList<TResult> ExecuteDeferred(PagedList<TSource> source, Func<TSource, TResult> selector)
        {
            var transformed = ((IEnumerable<TSource>) source).Select(selector);
            return new PagedList<TResult>(transformed, source.TotalCount, source.Page, source.PageSize);
        }
    }
}