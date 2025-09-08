namespace MegaNotes.Api.Dtos;

public record AuthorDto(Guid Id, string Name);
public record AuthorCreateDto(string Name);
