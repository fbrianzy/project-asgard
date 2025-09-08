namespace MegaNotes.Api.Utilities;

public enum SortDirection { Asc, Desc }

public static class Sorting
{
    public static IEnumerable<T> Apply<T, TKey>(IEnumerable<T> src, Func<T, TKey> key, SortDirection dir)
        => dir == SortDirection.Asc ? src.OrderBy(key) : src.OrderByDescending(key);
}
