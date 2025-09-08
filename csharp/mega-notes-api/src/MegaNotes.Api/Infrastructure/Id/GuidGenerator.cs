namespace MegaNotes.Api.Infrastructure.Id;

public interface IGuidGenerator { Guid NewGuid(); }
public class GuidGenerator : IGuidGenerator { public Guid NewGuid() => Guid.NewGuid(); }
