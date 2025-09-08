namespace MegaNotes.Api.Utilities;

public record PagedResult<T>(IEnumerable<T> Items, int Total, int Page, int PageSize);
