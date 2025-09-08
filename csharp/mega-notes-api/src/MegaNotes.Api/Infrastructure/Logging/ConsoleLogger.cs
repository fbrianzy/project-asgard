namespace MegaNotes.Api.Infrastructure.Logging;

public interface ISimpleLogger { void Info(string msg); void Error(string msg); }
public class ConsoleLogger : ISimpleLogger
{
    public void Info(string msg) => Console.WriteLine($"[INFO] {msg}");
    public void Error(string msg) => Console.WriteLine($"[ERROR] {msg}");
}
