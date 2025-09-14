using System.Net.Sockets;
using System.Text;

namespace ChatClientApp;

class Program
{
    static async Task Main(string[] args)
    {
        string host = args.Length > 0 ? args[0] : "127.0.0.1";
        int port = args.Length > 1 ? int.Parse(args[1]) : 5000;
        Console.Write("Username: ");
        var username = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(username)) username = "Anon";

        using var client = new TcpClient();
        await client.ConnectAsync(host, port);
        Console.WriteLine($"Connected to {host}:{port}");

        using var stream = client.GetStream();
        var reader = new StreamReader(stream, Encoding.UTF8);
        var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

        // Send username first
        await writer.WriteLineAsync(username);

        // Read incoming messages
        _ = Task.Run(async () =>
        {
            try
            {
                while (true)
                {
                    var line = await reader.ReadLineAsync();
                    if (line == null) break;
                    Console.WriteLine(line);
                }
            }
            catch { }
        });

        // Send messages
        while (true)
        {
            var input = Console.ReadLine();
            if (input == null || input.Equals("/quit", StringComparison.OrdinalIgnoreCase)) break;
            await writer.WriteLineAsync(input);
        }
    }
}
