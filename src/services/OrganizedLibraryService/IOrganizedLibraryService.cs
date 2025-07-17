using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;

namespace OrganizedLibraryService;

public interface IOrganizedLibraryService
{
    /// <summary>
    /// Resolves an OrganizedLibraryNode from partial data
    /// </summary>
    /// <param name="partialOrganizedLibrary">Partial organized library data to resolve</param>
    /// <returns>Resolved OrganizedLibraryNode instance</returns>
    Task<OrganizedLibraryNode> ResolveNodeAsync(OrganizedLibraryNode partialOrganizedLibrary);

    /// <summary>
    /// Gets organized library pins for a specific organized library (self-reference)
    /// </summary>
    /// <param name="organizedLibrary">The organized library node</param>
    /// <returns>Collection of pins representing organized library data</returns>
    Task<IEnumerable<Pin>> GetOrganizedLibraryPinsAsync(OrganizedLibraryNode organizedLibrary);

    /// <summary>
    /// Gets organized library pins for a player (cross-domain)
    /// </summary>
    /// <param name="player">The player node</param>
    /// <returns>Collection of pins linking player to organized libraries</returns>
    Task<IEnumerable<Pin>> GetOrganizedLibraryPinsAsync(PlayerNode player);

    /// <summary>
    /// Gets organized library pins for a library (cross-domain)
    /// </summary>
    /// <param name="library">The library node</param>
    /// <returns>Collection of pins linking library to organized libraries</returns>
    Task<IEnumerable<Pin>> GetOrganizedLibraryPinsAsync(LibraryNode library);
} 