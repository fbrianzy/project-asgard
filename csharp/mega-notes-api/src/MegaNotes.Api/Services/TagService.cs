using MegaNotes.Api.Domain;
using MegaNotes.Api.Dtos;
using MegaNotes.Api.Interfaces;

namespace MegaNotes.Api.Services;

public class TagService
{
    private readonly ITagRepository _repo;
    public TagService(ITagRepository repo) => _repo = repo;

    public IEnumerable<TagDto> GetAll() => _repo.GetAll().Select(t => new TagDto(t.Id, t.Name, t.Slug));

    public TagDto Create(TagCreateDto dto)
    {
        var t = _repo.Add(new Tag { Name = dto.Name.Trim() });
        return new TagDto(t.Id, t.Name, t.Slug);
    }
}
