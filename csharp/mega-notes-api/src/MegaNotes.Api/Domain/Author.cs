namespace MegaNotes.Api.Domain;

public class Author : AuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
}
