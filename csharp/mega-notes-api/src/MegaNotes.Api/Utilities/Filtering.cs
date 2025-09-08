namespace MegaNotes.Api.Utilities;

public static class Filtering
{
    public static IEnumerable<T> WhereIf<T>(IEnumerable<T> src, bool condition, Func<T, bool> predicate)
        => condition ? src.Where(predicate) : src;
}
