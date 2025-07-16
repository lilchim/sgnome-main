using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;

namespace UserLibraryService;

public interface IUserLibraryService
{
    /// <summary>
    /// Gets pins representing the user's library data
    /// </summary>
    /// <param name="player">The player node to get library pins for</param>
    /// <returns>Collection of pins representing library data</returns>
    Task<IEnumerable<Pin>> GetUserLibraryPinsAsync(PlayerNode player);
} 