using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;

namespace LibraryService;

public interface ILibraryService
{
    /// <summary>
    /// Resolves a LibraryNode from partial data
    /// </summary>
    /// <param name="partialLibrary">Partial library data to resolve</param>
    /// <returns>Resolved LibraryNode instance</returns>
    Task<LibraryNode> ResolveNodeAsync(LibraryNode partialLibrary);

    /// <summary>
    /// Gets library pins for a player
    /// </summary>
    /// <param name="player">The player node</param>
    /// <returns>Collection of pins representing library data</returns>
    Task<IEnumerable<Pin>> GetLibraryPinsAsync(PlayerNode player);
} 