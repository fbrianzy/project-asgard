using MegaNotes.Api.Domain;
using MegaNotes.Api.Interfaces;
using MegaNotes.Api.Utilities;

namespace MegaNotes.Api.Repositories;

public class InMemoryTagRepository : ITagRepository
{
    private readonly List<Tag> _db = new();

    public IEnumerable<Tag> GetAll() => _db;
    public Tag? GetById(Guid id) => _db.FirstOrDefault(x => x.Id == id);

    public Tag Add(Tag entity)
    {
        entity.Id = Guid.NewGuid();
        entity.Slug = Slugify.ToSlug(entity.Name);
        entity.CreatedAt = DateTime.UtcNow;
        _db.Add(entity);
        return entity;
    }

    public Tag? Update(Guid id, Tag entity)
    {
        var e = GetById(id); if (e is null) return null;
        e.Name = entity.Name;
        e.Slug = Slugify.ToSlug(entity.Name);
        e.UpdatedAt = DateTime.UtcNow;
        return e;
    }

    public bool Delete(Guid id)
    {
        var e = GetById(id); if (e is null) return false;
        return _db.Remove(e);
    }

    public Tag? GetBySlug(string slug) => _db.FirstOrDefault(x => x.Slug == slug);
}
