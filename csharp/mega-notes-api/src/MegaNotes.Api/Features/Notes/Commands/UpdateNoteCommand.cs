namespace MegaNotes.Api.Features.Notes.Commands;

public record UpdateNoteCommand(System.Guid Id, string? Title, string? Content);
