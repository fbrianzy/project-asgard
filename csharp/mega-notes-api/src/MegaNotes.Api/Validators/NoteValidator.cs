using MegaNotes.Api.Dtos;

namespace MegaNotes.Api.Validators;

public static class NoteValidator
{
    public static void ValidateCreate(NoteCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title)) throw new ArgumentException("Title is required");
        if (string.IsNullOrWhiteSpace(dto.Content)) throw new ArgumentException("Content is required");
    }
}
