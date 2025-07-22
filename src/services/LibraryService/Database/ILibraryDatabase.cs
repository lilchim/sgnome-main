using Sgnome.Models.Nodes;

namespace LibraryService.Database;

/// <summary>
/// Handles persistent storage and retrieval of LibraryNode data in Redis
/// </summary>
public interface ILibraryDatabase
{
    /// <summary>
    /// Resolves a LibraryNode using any of the provided identifiers, creating or updating as needed
    /// </summary>
    /// <param name="identifiers">Dictionary of identifier type -> value pairs</param>
    /// <param name="librarySource">The library source (steam, epic, etc.)</param>
    /// <param name="displayName">Optional display name for new libraries</param>
    /// <returns>Resolved LibraryNode with all identifiers up to date</returns>
    Task<LibraryNode> ResolveLibraryAsync(Dictionary<string, string> identifiers, string librarySource, string? displayName = null);
    
    /// <summary>
    /// Adds additional identifiers to an existing library
    /// </summary>
    /// <param name="internalId">The internal library ID</param>
    /// <param name="identifiers">Additional identifier mappings to add</param>
    Task AddIdentifiersAsync(string internalId, Dictionary<string, string> identifiers);
    
    /// <summary>
    /// Gets a library by its internal ID
    /// </summary>
    /// <param name="internalId">The internal library ID</param>
    /// <returns>The library node if found, null otherwise</returns>
    Task<LibraryNode?> GetByInternalIdAsync(string internalId);
} 