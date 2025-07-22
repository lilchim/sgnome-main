using Microsoft.Extensions.DependencyInjection;
using PlayerService.Database;
using PlayerService.PinGenerators;
using PlayerService.Providers;

namespace PlayerService;

public static class PlayerServiceExtensions
{
    public static IServiceCollection AddPlayerService(this IServiceCollection services)
    {
        services.AddScoped<IPlayerDatabase, RedisPlayerDatabase>();
        services.AddScoped<PlayerInfoPinGenerator>();
        services.AddScoped<PlayerFriendsPinGenerator>();
        services.AddScoped<PlayerActivityPinGenerator>();
        services.AddScoped<IPlayerService, PlayerService>();
        return services;
    }
} 