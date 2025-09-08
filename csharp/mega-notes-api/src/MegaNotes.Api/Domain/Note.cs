namespace MegaNotes.Api.Domain;

public class Note : AuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public Guid? AuthorId { get; set; }
    public List<Guid> TagIds { get; set; } = new();
}
