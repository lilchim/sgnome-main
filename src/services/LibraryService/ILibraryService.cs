using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;

namespace LibraryService;

/// <summary>
/// Service for consuming library-related nodes and generating pins about related data
/// </summary>
public interface ILibraryService
{
    /// <summary>
    /// Consumes a LibraryNode and returns pins + resolved node
    /// </summary>
    /// <param name="partial">Partial library node with identifiers</param>
    /// <returns>Tuple of pins and resolved library node (self domain)</returns>
    Task<(IEnumerable<Pin> Pins, LibraryNode ResolvedNode)> Consume(LibraryNode partial);

    /// <summary>
    /// Consumes a LibraryListNode and returns pins + resolved node
    /// </summary>
    /// <param name="partial">Partial library list node with player ID</param>
    /// <returns>Tuple of pins and resolved library list node (self domain)</returns>
    Task<(IEnumerable<Pin> Pins, LibraryListNode ResolvedNode)> Consume(LibraryListNode partial);

    /// <summary>
    /// Consumes a PlayerNode and returns pins about their libraries
    /// </summary>
    /// <param name="player">Player node (foreign domain)</param>
    /// <returns>Pins for all library-related data (foreign domain)</returns>
    Task<IEnumerable<Pin>> Consume(PlayerNode player);
}
