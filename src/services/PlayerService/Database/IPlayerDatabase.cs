using Sgnome.Models.Nodes;

namespace PlayerService.Database;

/// <summary>
/// Handles persistent storage and retrieval of PlayerNode data in Redis
/// </summary>
public interface IPlayerDatabase
{
    /// <summary>
    /// Attempts to resolve a PlayerNode using any of the provided identifiers
    /// </summary>
    /// <param name="identifiers">Dictionary of identifier type -> value pairs</param>
    /// <returns>Resolved PlayerNode if found, null otherwise</returns>
    Task<PlayerNode?> ResolvePlayerAsync(Dictionary<string, object> identifiers);
    
    /// <summary>
    /// Creates a new PlayerNode and establishes identifier mappings
    /// </summary>
    /// <param name="player">The player node to store</param>
    /// <param name="identifiers">Dictionary of identifier type -> value pairs</param>
    /// <returns>The created PlayerNode with internal ID assigned</returns>
    Task<PlayerNode> CreatePlayerAsync(PlayerNode player, Dictionary<string, object> identifiers);
    
    /// <summary>
    /// Updates an existing PlayerNode
    /// </summary>
    /// <param name="player">The player node to update</param>
    /// <returns>The updated PlayerNode</returns>
    Task<PlayerNode> UpdatePlayerAsync(PlayerNode player);
    
    /// <summary>
    /// Adds additional identifiers to an existing player
    /// </summary>
    /// <param name="internalId">The internal player ID</param>
    /// <param name="identifiers">Additional identifier mappings to add</param>
    Task AddIdentifiersAsync(string internalId, Dictionary<string, object> identifiers);
} 