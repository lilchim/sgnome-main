using Sgnome.Models.Graph;

namespace LibraryService.Providers;

public interface ISteamLibraryProvider
{
    /// <summary>
    /// Gets library pins for a Steam library
    /// </summary>
    /// <param name="steamId">The Steam ID of the player</param>
    /// <param name="context">Context about how to create the pins</param>
    /// <returns>Collection of pins representing Steam library data</returns>
    Task<IEnumerable<Pin>> GetLibraryPinsAsync(string steamId, PinContext context);
} 