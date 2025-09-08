namespace MegaNotes.Api.Dtos;

public record TagDto(Guid Id, string Name, string Slug);
public record TagCreateDto(string Name);
