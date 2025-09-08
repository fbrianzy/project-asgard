using MegaNotes.Api.Domain;
using MegaNotes.Api.Dtos;
using MegaNotes.Api.Interfaces;
using MegaNotes.Api.Mapping;
using MegaNotes.Api.Utilities;

namespace MegaNotes.Api.Services;

public class NoteService
{
    private readonly INoteRepository _notes;
    private readonly IAuthorRepository _authors;
    private readonly ITagRepository _tags;

    public NoteService(INoteRepository notes, IAuthorRepository authors, ITagRepository tags)
    {
        _notes = notes; _authors = authors; _tags = tags;
    }

    public PagedResult<NoteDto> Search(int page, int pageSize, string? q)
    {
        var items = _notes.Search(q).ToList();
        var total = items.Count;
        var slice = items.Skip((page-1)*pageSize).Take(pageSize).ToList();
        var mapped = slice.Select(n => Mapper.ToDto(n, _authors, _tags)).ToList();
        return new PagedResult<NoteDto>(mapped, total, page, pageSize);
    }

    public NoteDto? Get(Guid id)
    {
        var n = _notes.GetById(id);
        return n is null ? null : Mapper.ToDto(n, _authors, _tags);
    }

    public NoteDto Create(NoteCreateDto dto)
    {
        var entity = new Note { Title = dto.Title, Content = dto.Content, AuthorId = dto.AuthorId, TagIds = dto.TagIds?.ToList() ?? new() };
        var saved = _notes.Add(entity);
        return Mapper.ToDto(saved, _authors, _tags);
    }

    public NoteDto? Update(Guid id, NoteUpdateDto dto)
    {
        var current = _notes.GetById(id);
        if (current is null) return null;
        if (!string.IsNullOrWhiteSpace(dto.Title)) current.Title = dto.Title.Trim();
        if (!string.IsNullOrWhiteSpace(dto.Content)) current.Content = dto.Content.Trim();
        if (dto.AuthorId.HasValue) current.AuthorId = dto.AuthorId;
        if (dto.TagIds is not null) current.TagIds = dto.TagIds.ToList();
        var updated = _notes.Update(id, current)!;
        return Mapper.ToDto(updated, _authors, _tags);
    }

    public bool Delete(Guid id) => _notes.Delete(id);
}
