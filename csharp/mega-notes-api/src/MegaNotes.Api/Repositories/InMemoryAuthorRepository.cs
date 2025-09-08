using MegaNotes.Api.Domain;
using MegaNotes.Api.Interfaces;

namespace MegaNotes.Api.Repositories;

public class InMemoryAuthorRepository : IAuthorRepository
{
    private readonly List<Author> _db = new();

    public IEnumerable<Author> GetAll() => _db;
    public Author? GetById(Guid id) => _db.FirstOrDefault(x => x.Id == id);

    public Author Add(Author entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        _db.Add(entity);
        return entity;
    }

    public Author? Update(Guid id, Author entity)
    {
        var e = GetById(id); if (e is null) return null;
        e.Name = entity.Name; e.UpdatedAt = DateTime.UtcNow;
        return e;
    }

    public bool Delete(Guid id)
    {
        var e = GetById(id);
        if (e is null) return false;
        return _db.Remove(e);
    }
}
