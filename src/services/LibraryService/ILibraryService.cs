using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;

namespace LibraryService;

public interface ILibraryService
{
    /// <summary>
    /// Gets pins representing the player's library overview
    /// </summary>
    /// <param name="player">The player to get library pins for</param>
    /// <returns>Collection of pins representing library data</returns>
    Task<IEnumerable<Pin>> GetLibraryPinsAsync(PlayerNode player);
    
    /// <summary>
    /// Gets pins representing organized views of a specific library source
    /// </summary>
    /// <param name="librarySource">The library source (Steam, Epic, etc.)</param>
    /// <param name="playerId">The player ID</param>
    /// <returns>Collection of pins representing organized library views</returns>
    Task<IEnumerable<Pin>> GetOrganizedLibraryPinsAsync(string librarySource, string playerId);
} 