using System.Threading.Channels;
using ImageResizer.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton(Channel.CreateUnbounded<ImageJob>());
builder.Services.AddHostedService<WatcherService>();
builder.Services.AddHostedService<ResizeWorker>();

var host = builder.Build();
await host.RunAsync();
