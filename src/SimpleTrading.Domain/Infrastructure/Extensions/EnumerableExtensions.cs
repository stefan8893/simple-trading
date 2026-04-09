namespace SimpleTrading.Domain.Infrastructure.Extensions;

public static class EnumerableExtensions
{
    // based on: https://github.com/DapperLib/Dapper/blob/9ed3525598494dddc1fbeb4e95e018239fffed13/Dapper/SqlMapper.cs#L518
    /// <param name="source">The enumerable to return as a list.</param>
    /// <typeparam name="T">The type of the element in the list.</typeparam>
    extension<T>(IEnumerable<T>? source)
    {
        /// <summary>
        ///     Collects the data as a list; if it is *already* a list, the original object is returned without
        ///     any duplication; otherwise, ToList() is invoked.
        /// </summary>
        public List<T> AsList()
        {
            return source switch
            {
                null => [],
                List<T> list => list,
                T[] array => [..array],
                _ => source.ToList()
            };
        }
    }
}