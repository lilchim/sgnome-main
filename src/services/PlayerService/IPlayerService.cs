using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;

namespace PlayerService;

public interface IPlayerService
{
    /// <summary>
    /// Consumes a PlayerNode (self-domain) - resolves the node and returns enrichment pins
    /// </summary>
    /// <param name="partial">Partial player data to resolve</param>
    /// <returns>Tuple of enrichment pins and resolved PlayerNode</returns>
    Task<(IEnumerable<Pin> Pins, PlayerNode ResolvedNode)> Consume(PlayerNode partial);

    /// <summary>
    /// Consumes a LibraryNode (foreign-domain) - returns pins linking to player data
    /// </summary>
    /// <param name="library">Library node to analyze for player connections</param>
    /// <returns>Collection of pins representing player-related information</returns>
    Task<IEnumerable<Pin>> Consume(LibraryNode library);

    /// <summary>
    /// Consumes a LibraryListNode (foreign-domain) - returns pins linking to player data
    /// </summary>
    /// <param name="libraryList">Library list node to analyze for player connections</param>
    /// <returns>Collection of pins representing player-related information</returns>
    Task<IEnumerable<Pin>> Consume(LibraryListNode libraryList);
} 