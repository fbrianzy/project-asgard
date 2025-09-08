using MegaNotes.Api.Infrastructure.Extensions;
using MegaNotes.Api.Infrastructure.Middleware;
using MegaNotes.Api.Repositories;
using MegaNotes.Api.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // no Swagger, but keep for future
builder.Services.AddAppServices();
builder.Services.AddRepositories();

var app = builder.Build();

app.UseMiddleware<RequestTimingMiddleware>();
app.MapControllers();
app.MapGet("/", () => new { name = "MegaNotes.Api", status = "ok" });

app.Run();
