using Microsoft.Extensions.DependencyInjection;
using GamesService.Database;

namespace GamesService;

public static class GamesServiceExtensions
{
    public static IServiceCollection AddGamesService(this IServiceCollection services)
    {
        services.AddScoped<IGamesService, GamesService>();
        services.AddScoped<IGamesDatabase, RedisGamesDatabase>();
        services.AddScoped<GameNodeConsumer>();
        return services;
    }
} 