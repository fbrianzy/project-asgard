using MegaNotes.Api.Domain;
using MegaNotes.Api.Dtos;
using MegaNotes.Api.Interfaces;

namespace MegaNotes.Api.Mapping;

public static class Mapper
{
    public static NoteDto ToDto(Note n, IAuthorRepository authors, ITagRepository tags)
    {
        var author = n.AuthorId.HasValue ? authors.GetById(n.AuthorId.Value)?.Name : null;
        var tagNames = n.TagIds.Select(id => tags.GetById(id)?.Name ?? "").Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        return new NoteDto(n.Id, n.Title, n.Content, author, tagNames);
    }
}
