using Microsoft.Extensions.DependencyInjection;
using GamesService.Database;
using Sgnome.Services.GamesService.Consumers;

namespace GamesService;

public static class GamesServiceExtensions
{
    public static IServiceCollection AddGamesService(this IServiceCollection services)
    {
        services.AddScoped<IGamesService, GamesService>();
        services.AddScoped<IGamesDatabase, RedisGamesDatabase>();
        services.AddScoped<GameNodeConsumer>();
        services.AddScoped<PlayerNodeConsumer>();
        return services;
    }
} 