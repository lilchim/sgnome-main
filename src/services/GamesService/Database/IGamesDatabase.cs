using Sgnome.Models.Nodes;

namespace GamesService.Database;

public interface IGamesDatabase
{
    /// <summary>
    /// Resolves a game by its identifiers, creating or updating as needed
    /// </summary>
    /// <param name="identifiers">Dictionary of storefront identifiers</param>
    /// <returns>Resolved GameNode with internal ID</returns>
    Task<GameNode> ResolveGameAsync(Dictionary<string, object> identifiers);

    /// <summary>
    /// Gets a game by its internal ID
    /// </summary>
    /// <param name="internalId">Internal game ID</param>
    /// <returns>GameNode if found, null otherwise</returns>
    Task<GameNode?> GetGameAsync(string internalId);

    /// <summary>
    /// Updates game data
    /// </summary>
    /// <param name="game">Game to update</param>
    Task UpdateGameAsync(GameNode game);
} 