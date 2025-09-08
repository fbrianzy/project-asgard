using MegaNotes.Api.Dtos;

namespace MegaNotes.Api.Validators;

public static class TagValidator
{
    public static void ValidateCreate(TagCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Name is required");
    }
}
