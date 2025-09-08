using MegaNotes.Api.Domain;
using MegaNotes.Api.Interfaces;
using MegaNotes.Api.Utilities;

namespace MegaNotes.Api.Repositories;

public class InMemoryNoteRepository : INoteRepository
{
    private readonly List<Note> _db = new();

    public IEnumerable<Note> GetAll() => _db;

    public Note? GetById(Guid id) => _db.FirstOrDefault(x => x.Id == id);

    public Note Add(Note entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        _db.Add(entity);
        return entity;
    }

    public Note? Update(Guid id, Note entity)
    {
        var existing = GetById(id);
        if (existing is null) return null;
        existing.Title = entity.Title;
        existing.Content = entity.Content;
        existing.AuthorId = entity.AuthorId;
        existing.TagIds = entity.TagIds;
        existing.UpdatedAt = DateTime.UtcNow;
        return existing;
    }

    public bool Delete(Guid id)
    {
        var e = GetById(id);
        if (e is null) return false;
        return _db.Remove(e);
    }

    public IEnumerable<Note> Search(string? query)
    {
        if (string.IsNullOrWhiteSpace(query)) return _db;
        query = query.ToLowerInvariant();
        return _db.Where(n => (n.Title ?? "").ToLowerInvariant().Contains(query) 
                           || (n.Content ?? "").ToLowerInvariant().Contains(query));
    }
}
