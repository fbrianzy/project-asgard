using MegaNotes.Api.Dtos;

namespace MegaNotes.Api.Validators;

public static class AuthorValidator
{
    public static void ValidateCreate(AuthorCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Name is required");
    }
}
