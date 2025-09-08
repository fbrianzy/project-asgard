using MegaNotes.Api.Domain;

namespace MegaNotes.Api.Interfaces;

public interface INoteRepository : IRepository<Note, Guid>
{
    IEnumerable<Note> Search(string? query);
}
