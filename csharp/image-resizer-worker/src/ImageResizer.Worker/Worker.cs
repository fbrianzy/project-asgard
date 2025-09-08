using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ImageResizer.Worker;

// Service 1: Watcher — masukkan file baru ke antrian
public class WatcherService : BackgroundService
{
    private readonly ILogger<WatcherService> _log;
    private readonly Channel<ImageJob> _channel;
    private readonly string _watch;

    public WatcherService(ILogger<WatcherService> log, Channel<ImageJob> channel, IConfiguration cfg)
    {
        _log = log; _channel = channel;
        _watch = cfg["WatchFolder"] ?? "inbox";
        Directory.CreateDirectory(_watch);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _log.LogInformation("Watching {Folder}", _watch);
        using var fsw = new FileSystemWatcher(_watch) { EnableRaisingEvents = true, Filter = "*.*" };
        fsw.Created += async (_, e) =>
        {
            if (ct.IsCancellationRequested) return;
            await _channel.Writer.WriteAsync(new ImageJob(e.FullPath), ct);
            _log.LogInformation("Queued: {File}", e.Name);
        };

        // idle loop
        while (!ct.IsCancellationRequested) await Task.Delay(1000, ct);
    }
}

// Service 2: Worker — konsumsi antrian dan resize
public class ResizeWorker : BackgroundService
{
    private readonly ILogger<ResizeWorker> _log;
    private readonly Channel<ImageJob> _channel;
    private readonly string _out;
    private readonly int[] _sizes;

    public ResizeWorker(ILogger<ResizeWorker> log, Channel<ImageJob> channel, IConfiguration cfg)
    {
        _log = log; _channel = channel;
        _out = cfg["OutputFolder"] ?? "outbox";
        Directory.CreateDirectory(_out);
        _sizes = cfg.GetSection("Sizes").Get<int[]>() ?? new[] { 256, 1024, 1600 };
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _log.LogInformation("Resizing to: {Sizes}", string.Join(",", _sizes));
        await foreach (var job in _channel.Reader.ReadAllAsync(ct))
        {
            try { await Process(job, ct); }
            catch (Exception ex) { _log.LogError(ex, "Failed processing {File}", job.Path); }
        }
    }

    private async Task Process(ImageJob job, CancellationToken ct)
    {
        var file = job.Path;
        using var img = await Image.LoadAsync(file, ct);

        foreach (var s in _sizes)
        {
            var clone = img.Clone(x => x.Resize(new ResizeOptions {
                Mode = ResizeMode.Max, Size = new Size(s, s)
            }));
            var name = Path.GetFileNameWithoutExtension(file);
            var ext = Path.GetExtension(file);
            var outPath = Path.Combine(_out, $"{name}_{s}{ext}");
            await clone.SaveAsync(outPath, ct);
            _log.LogInformation("Wrote {Out}", outPath);
        }
    }
}
