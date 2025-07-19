using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;

namespace LibrariesService.Providers;

public interface ISteamLibrariesProvider
{
    /// <summary>
    /// Gets Steam library pins for a player
    /// </summary>
    /// <param name="steamId">The Steam ID of the player</param>
    /// <param name="context">Context about how to create the pins</param>
    /// <returns>Collection of pins representing Steam library data</returns>
    Task<IEnumerable<Pin>> GetLibraryPinsAsync(LibrariesNode libraries, PinContext context);
} 