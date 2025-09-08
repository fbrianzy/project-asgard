namespace MegaNotes.Api.Utilities;

public static class Guard
{
    public static void NotNull(object? value, string name)
    {
        if (value is null) throw new ArgumentNullException(name);
    }

    public static string NotEmpty(string? value, string name)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException($"{name} is required.");
        return value;
    }
}
