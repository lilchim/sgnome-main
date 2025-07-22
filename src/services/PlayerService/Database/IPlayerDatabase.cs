using Sgnome.Models.Nodes;

namespace PlayerService.Database;

/// <summary>
/// Handles persistent storage and retrieval of PlayerNode data in Redis
/// </summary>
public interface IPlayerDatabase
{
    /// <summary>
    /// Resolves a PlayerNode using any of the provided identifiers, creating or updating as needed
    /// </summary>
    /// <param name="identifiers">Dictionary of identifier type -> value pairs</param>
    /// <returns>Resolved PlayerNode with all identifiers up to date</returns>
    Task<PlayerNode> ResolvePlayerAsync(Dictionary<string, string> identifiers);
    
    /// <summary>
    /// Adds additional identifiers to an existing player
    /// </summary>
    /// <param name="internalId">The internal player ID</param>
    /// <param name="identifiers">Additional identifier mappings to add</param>
    Task AddIdentifiersAsync(string internalId, Dictionary<string, string> identifiers);
} 