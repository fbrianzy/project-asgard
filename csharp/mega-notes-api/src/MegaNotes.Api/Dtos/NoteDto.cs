namespace MegaNotes.Api.Dtos;

public record NoteDto(Guid Id, string Title, string Content, string? Author, string[] Tags);
public record NoteCreateDto(string Title, string Content, Guid? AuthorId, Guid[] TagIds);
public record NoteUpdateDto(string? Title, string? Content, Guid? AuthorId, Guid[]? TagIds);
