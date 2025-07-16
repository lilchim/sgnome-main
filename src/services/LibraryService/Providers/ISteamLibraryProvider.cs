using Sgnome.Models.Graph;

namespace LibraryService.Providers;

public interface ISteamLibraryProvider
{
    /// <summary>
    /// Gets Steam library pins for a player
    /// </summary>
    /// <param name="steamId">The Steam ID of the player</param>
    /// <returns>Collection of pins representing Steam library data</returns>
    Task<IEnumerable<Pin>> GetLibraryPinsAsync(string steamId);
} 