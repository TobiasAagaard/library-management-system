using Microsoft.Extensions.DependencyInjection;
using Lms.Application.Repositories;

namespace Lms.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<ILibraryRepository, LibraryRepository>();
        services.AddSingleton<IBookRepository, BookRepository>();
        
        return services;
    }
}