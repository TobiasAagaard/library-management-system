using Microsoft.Extensions.DependencyInjection;
using Lms.Application.Repositories;
using Lms.Application.Services;

namespace Lms.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<ILibraryRepository, LibraryRepository>();
        services.AddSingleton<IBookRepository, BookRepository>();
        services.AddSingleton<ILibraryService, LibraryService>();

        return services;
    }
}