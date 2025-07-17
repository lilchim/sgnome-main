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
    /// Gets library pins for a player (cross-domain)
    /// </summary>
    /// <param name="player">The player node</param>
    /// <returns>Collection of pins representing library data</returns>
    Task<IEnumerable<Pin>> GetLibraryPinsAsync(PlayerNode player);

    /// <summary>
    /// Gets library pins for a library node itself (self-reference)
    /// </summary>
    /// <param name="library">The library node</param>
    /// <returns>Collection of informational pins about the library</returns>
    Task<IEnumerable<Pin>> GetLibraryPinsAsync(LibraryNode library);
} 