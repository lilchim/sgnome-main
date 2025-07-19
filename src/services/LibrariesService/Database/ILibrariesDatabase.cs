using Sgnome.Models.Nodes;

namespace LibrariesService.Database;

/// <summary>
/// Handles persistent storage and retrieval of LibrariesNode data in Redis
/// </summary>
public interface ILibrariesDatabase
{
    /// <summary>
    /// Attempts to resolve a LibrariesNode using the player ID
    /// </summary>
    /// <param name="playerId">The player ID</param>
    /// <returns>Resolved LibrariesNode if found, null otherwise</returns>
    Task<LibrariesNode?> ResolveLibrariesAsync(string playerId);
    
    /// <summary>
    /// Creates a new LibrariesNode for a player
    /// </summary>
    /// <param name="libraries">The libraries node to store</param>
    /// <returns>The created LibrariesNode with internal ID assigned</returns>
    Task<LibrariesNode> CreateLibrariesAsync(LibrariesNode libraries);
    
    /// <summary>
    /// Updates an existing LibrariesNode
    /// </summary>
    /// <param name="libraries">The libraries node to update</param>
    /// <returns>The updated LibrariesNode</returns>
    Task<LibrariesNode> UpdateLibrariesAsync(LibrariesNode libraries);
    
    /// <summary>
    /// Adds a LibraryNode to the LibrariesNode collection
    /// </summary>
    /// <param name="librariesId">The internal libraries ID</param>
    /// <param name="libraryId">The internal library ID to add</param>
    Task AddLibraryAsync(string librariesId, string libraryId);
    
    /// <summary>
    /// Gets all LibraryNode IDs that belong to a LibrariesNode
    /// </summary>
    /// <param name="librariesId">The internal libraries ID</param>
    /// <returns>List of library IDs</returns>
    Task<List<string>> GetLibraryIdsAsync(string librariesId);
} 