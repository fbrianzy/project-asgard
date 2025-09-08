namespace MegaNotes.Api.Infrastructure.Clock;

public interface IClock { DateTime UtcNow { get; } }
public class SystemClock : IClock { public DateTime UtcNow => DateTime.UtcNow; }
