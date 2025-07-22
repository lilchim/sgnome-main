using Microsoft.Extensions.DependencyInjection;

namespace Sgnome.Clients.Steam;

/// <summary>
/// Extension methods for registering Steam client services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Steam client services to the service collection.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddSteamClient(this IServiceCollection services)
    {
        services.AddScoped<ISteamClient, SteamClient>();
        return services;
    }
} 