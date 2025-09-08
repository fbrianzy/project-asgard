using MegaNotes.Api.Domain;
using MegaNotes.Api.Dtos;
using MegaNotes.Api.Interfaces;

namespace MegaNotes.Api.Services;

public class AuthorService
{
    private readonly IAuthorRepository _repo;
    public AuthorService(IAuthorRepository repo) => _repo = repo;

    public IEnumerable<AuthorDto> GetAll() => _repo.GetAll().Select(a => new AuthorDto(a.Id, a.Name));

    public AuthorDto Create(AuthorCreateDto dto)
    {
        var a = _repo.Add(new Author { Name = dto.Name.Trim() });
        return new AuthorDto(a.Id, a.Name);
    }
}
