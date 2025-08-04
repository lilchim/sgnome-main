using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;

namespace GamesService;

public interface IGamesService
{
    /// <summary>
    /// Consumes a GameNode (self-domain) - resolves the node and returns enrichment pins
    /// </summary>
    /// <param name="partial">Partial game data to resolve</param>
    /// <returns>Tuple of enrichment pins and resolved GameNode</returns>
    Task<(IEnumerable<Pin> Pins, GameNode ResolvedNode)> Consume(GameNode partial);

    /// <summary>
    /// Consumes a LibraryNode (foreign-domain) - returns pins linking to game data
    /// </summary>
    /// <param name="library">Library node to analyze for game connections</param>
    /// <returns>Collection of pins representing game-related information</returns>
    Task<IEnumerable<Pin>> Consume(LibraryNode library);

    /// <summary>
    /// Consumes a PlayerNode (foreign-domain) - returns pins linking to game data
    /// </summary>
    /// <param name="player">Player node to analyze for game connections</param>
    /// <returns>Collection of pins representing game-related information</returns>
    Task<IEnumerable<Pin>> Consume(PlayerNode player);

    /// <summary>
    /// Consumes a GamesListNode (foreign-domain) - returns pins linking to game data
    /// </summary>
    /// <param name="gamesList">Games list node to analyze for game connections</param>
    /// <returns>Collection of pins representing game-related information</returns>
    Task<IEnumerable<Pin>> Consume(GamesListNode gamesList);
} 