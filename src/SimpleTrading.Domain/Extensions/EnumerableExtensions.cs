namespace SimpleTrading.Domain.Extensions;

public static class EnumerableExtensions
{
    // copied from: https://github.com/DapperLib/Dapper/blob/9ed3525598494dddc1fbeb4e95e018239fffed13/Dapper/SqlMapper.cs#L518

    /// <summary>
    ///     Obtains the data as a list; if it is *already* a list, the original object is returned without
    ///     any duplication; otherwise, ToList() is invoked.
    /// </summary>
    /// <typeparam name="T">The type of element in the list.</typeparam>
    /// <param name="source">The enumerable to return as a list.</param>
    public static List<T> AsList<T>(this IEnumerable<T>? source)
    {
        return source switch
        {
            null => null!,
            List<T> list => list,
            _ => source.ToList()
        };
    }
}