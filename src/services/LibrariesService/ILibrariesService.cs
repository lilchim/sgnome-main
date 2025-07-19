using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;

namespace LibrariesService;

public interface ILibrariesService
{
    /// <summary>
    /// Resolves a LibrariesNode from partial data
    /// </summary>
    /// <param name="partialLibraries">Partial libraries data to resolve</param>
    /// <returns>Resolved LibrariesNode instance</returns>
    Task<LibrariesNode> ResolveNodeAsync(LibrariesNode partialLibraries);

    /// <summary>
    /// Gets libraries pins for a player (cross-domain)
    /// </summary>
    /// <param name="player">The player node</param>
    /// <returns>Collection of pins representing libraries data</returns>
    Task<IEnumerable<Pin>> GetLibrariesPinsAsync(PlayerNode player);

    /// <summary>
    /// Gets libraries pins for a libraries node itself (self-reference)
    /// </summary>
    /// <param name="libraries">The libraries node</param>
    /// <returns>Collection of informational pins about the libraries</returns>
    Task<IEnumerable<Pin>> GetLibrariesPinsAsync(LibrariesNode libraries);
} 