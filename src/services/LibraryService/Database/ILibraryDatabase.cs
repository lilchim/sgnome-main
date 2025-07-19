using Sgnome.Models.Nodes;

namespace LibrariesService.Database;

/// <summary>
/// Handles persistent storage and retrieval of LibraryNode data in Redis
/// </summary>
public interface ILibraryDatabase
{
    /// <summary>
    /// Attempts to resolve a LibraryNode using player ID and library source
    /// </summary>
    /// <param name="playerId">The player ID</param>
    /// <param name="librarySource">The library source (steam, epic, etc.)</param>
    /// <returns>Resolved LibraryNode if found, null otherwise</returns>
    Task<LibraryNode?> ResolveLibraryAsync(string playerId, string librarySource);
    
    /// <summary>
    /// Creates a new LibraryNode
    /// </summary>
    /// <param name="library">The library node to store</param>
    /// <returns>The created LibraryNode with internal ID assigned</returns>
    Task<LibraryNode> CreateLibraryAsync(LibraryNode library);
    
    /// <summary>
    /// Updates an existing LibraryNode
    /// </summary>
    /// <param name="library">The library node to update</param>
    /// <returns>The updated LibraryNode</returns>
    Task<LibraryNode> UpdateLibraryAsync(LibraryNode library);
    
    /// <summary>
    /// Gets all LibraryNode instances for a player
    /// </summary>
    /// <param name="playerId">The player ID</param>
    /// <returns>List of library nodes</returns>
    Task<List<LibraryNode>> GetLibrariesForPlayerAsync(string playerId);
} 