namespace PlayerService.Providers;

public interface ISteamPlayerProvider
{
    /// <summary>
    /// Gets Steam player data for a player
    /// </summary>
    /// <param name="steamId">The Steam ID of the player</param>
    /// <returns>Steam player data or null if not found</returns>
    Task<SteamPlayerData?> GetPlayerDataAsync(string steamId);
    
    /// <summary>
    /// Gets Steam friends data for a player
    /// </summary>
    /// <param name="steamId">The Steam ID of the player</param>
    /// <returns>Steam friends data or null if not found</returns>
    Task<SteamFriendsData?> GetFriendsDataAsync(string steamId);
    
    /// <summary>
    /// Gets Steam activity data for a player
    /// </summary>
    /// <param name="steamId">The Steam ID of the player</param>
    /// <returns>Steam activity data or null if not found</returns>
    Task<SteamActivityData?> GetActivityDataAsync(string steamId);
} 