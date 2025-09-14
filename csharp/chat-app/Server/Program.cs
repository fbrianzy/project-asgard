using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Concurrent;

namespace ChatServerApp;

class Program
{
    static async Task Main(string[] args)
    {
        int port = args.Length > 0 ? int.Parse(args[0]) : 5000;
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine($"[Server] Listening on port {port}");

        var clients = new ConcurrentDictionary<TcpClient, string>();

        _ = Task.Run(async () =>
        {
            while (true)
            {
                try
                {
                    var client = await listener.AcceptTcpClientAsync();
                    _ = HandleClient(client, clients);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Accept error: {ex.Message}");
                }
            }
        });

        Console.WriteLine("Press ENTER to stop server...");
        Console.ReadLine();
        listener.Stop();
    }

    static async Task HandleClient(TcpClient client, ConcurrentDictionary<TcpClient, string> clients)
    {
        Console.WriteLine("[Server] Client connected");
        using var stream = client.GetStream();
        var reader = new StreamReader(stream, Encoding.UTF8);
        var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

        // First line as username
        string? username = await reader.ReadLineAsync();
        if (string.IsNullOrWhiteSpace(username)) username = "Anon";
        clients[client] = username;
        await Broadcast(clients, $"[+] {username} joined");

        try
        {
            while (true)
            {
                var line = await reader.ReadLineAsync();
                if (line == null) break;
                await Broadcast(clients, $"{username}: {line}");
            }
        }
        catch { /* ignore */ }
        finally
        {
            clients.TryRemove(client, out _);
            await Broadcast(clients, $"[-] {username} left");
            client.Close();
            Console.WriteLine("[Server] Client disconnected");
        }
    }

    static async Task Broadcast(ConcurrentDictionary<TcpClient, string> clients, string message)
    {
        Console.WriteLine(message);
        var dead = new List<TcpClient>();
        foreach (var kv in clients.Keys)
        {
            try
            {
                var w = new StreamWriter(kv.GetStream(), Encoding.UTF8) { AutoFlush = true };
                await w.WriteLineAsync(message);
            }
            catch
            {
                dead.Add(kv);
            }
        }
        foreach (var d in dead) clients.TryRemove(d, out _);
    }
}
