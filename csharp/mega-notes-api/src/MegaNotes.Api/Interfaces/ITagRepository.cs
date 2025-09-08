using MegaNotes.Api.Domain;

namespace MegaNotes.Api.Interfaces;

public interface ITagRepository : IRepository<Tag, Guid>
{
    Tag? GetBySlug(string slug);
}
