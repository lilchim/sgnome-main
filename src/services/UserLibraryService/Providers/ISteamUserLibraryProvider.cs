using SteamApi.Client;
using SteamApi.Models.Steam.Responses;

namespace UserLibraryService.Providers;

public interface ISteamUserLibraryProvider
{
    /// <summary>
    /// Gets the user's Steam library (owned games)
    /// </summary>
    /// <param name="steamId">Steam ID of the user</param>
    /// <returns>User library data or null if not found</returns>
    Task<SteamResponse<OwnedGamesResponse>?> GetUserLibraryAsync(string steamId);
    
    /// <summary>
    /// Gets the user's recently played games
    /// </summary>
    /// <param name="steamId">Steam ID of the user</param>
    /// <returns>Recently played games data or null if not found</returns>
    Task<SteamResponse<RecentlyPlayedGamesResponse>?> GetRecentlyPlayedGamesAsync(string steamId);
} 