using Sgnome.Models.Graph;

namespace OrganizedLibraryService.Providers;

public interface ISteamOrganizedLibraryProvider
{
    /// <summary>
    /// Gets organized library pins for a Steam library
    /// </summary>
    /// <param name="steamId">The Steam ID of the player</param>
    /// <param name="context">Context about how to create the pins</param>
    /// <returns>Collection of pins representing organized Steam library data</returns>
    Task<IEnumerable<Pin>> GetOrganizedLibraryPinsAsync(string steamId, PinContext context);
} 