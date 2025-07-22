using Sgnome.Models.Nodes;

namespace LibraryService.Database;

/// <summary>
/// Handles persistent storage and retrieval of LibraryListNode data in Redis
/// </summary>
public interface ILibraryListDatabase
{
    /// <summary>
    /// Resolves a LibraryListNode using the provided player ID, creating or updating as needed
    /// </summary>
    /// <param name="playerId">The player's internal ID</param>
    /// <param name="displayName">Optional display name for new library lists</param>
    /// <returns>Resolved LibraryListNode with all mappings up to date</returns>
    Task<LibraryListNode> ResolveLibraryListAsync(string playerId, string? displayName = null);
    
    /// <summary>
    /// Adds additional library source mappings to an existing library list
    /// </summary>
    /// <param name="internalId">The internal library list ID</param>
    /// <param name="librarySourceMappings">Additional library source -> library ID mappings to add</param>
    Task AddLibrarySourceMappingsAsync(string internalId, Dictionary<string, string> librarySourceMappings);
} 