namespace MegaNotes.Api.Features.Notes.Queries;

public record SearchNotesQuery(string? Q, int Page = 1, int PageSize = 10);
