using Microsoft.Extensions.DependencyInjection;
using LibraryService.Database;
using LibraryService.PinGenerators;
using Sgnome.Clients.Steam;

namespace LibraryService;

/// <summary>
/// Extension methods for configuring LibraryService dependencies
/// </summary>
public static class LibraryServiceExtensions
{
    /// <summary>
    /// Adds LibraryService and all its dependencies to the service collection
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddLibraryService(this IServiceCollection services)
    {
        // Register database implementations
        services.AddScoped<ILibraryDatabase, RedisLibraryDatabase>();
        services.AddScoped<ILibraryListDatabase, RedisLibraryListDatabase>();

        // Register Pin Generators
        services.AddScoped<LibraryPinGenerator>();
        services.AddScoped<LibraryListPinGenerator>();

        // Register the main service
        services.AddScoped<ILibraryService, LibraryService>();

        return services;
    }
} 