using MegaNotes.Api.Interfaces;
using MegaNotes.Api.Repositories;
using MegaNotes.Api.Services;

namespace MegaNotes.Api.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddSingleton<NoteService>();
        services.AddSingleton<AuthorService>();
        services.AddSingleton<TagService>();
        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<INoteRepository, InMemoryNoteRepository>();
        services.AddSingleton<IAuthorRepository, InMemoryAuthorRepository>();
        services.AddSingleton<ITagRepository, InMemoryTagRepository>();
        return services;
    }
}
