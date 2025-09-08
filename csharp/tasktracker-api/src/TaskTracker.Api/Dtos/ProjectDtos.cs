namespace TaskTracker.Api.Dtos;

public record ProjectCreateDto(string Name, string? Description);
public record ProjectUpdateDto(string? Name, string? Description);
