using MegaNotes.Api.Domain;

namespace MegaNotes.Api.Specifications;

public static class NoteSpecifications
{
    public static bool HasTag(Note n, Guid tagId) => n.TagIds.Contains(tagId);
    public static bool AuthoredBy(Note n, Guid authorId) => n.AuthorId == authorId;
}
