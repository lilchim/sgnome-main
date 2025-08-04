namespace Sgnome.Models.Nodes;

/// <summary>
/// Represents a collection of game libraries for a player.
/// 
/// Role: Aggregates a collection of libraries from various sources (Steam, Epic, GOG, etc.)
/// This node serves as an entry point to a player's various game libraries.
/// 
/// Characteristics:
/// - Always has a single player as its source
/// - Contains pins for each library source (Steam, Epic, etc.)
/// - Acts as a gateway to individual library sources
/// 
/// Workflow: Player -> Libraries (shows all available library sources)
/// 
/// Use Cases:
/// - Player expands "Libraries" pin from PlayerNode
/// - Provides overview of all available game libraries
/// - Enables access to individual library sources
/// </summary>
public class LibraryListNode
{
    /// <summary>
    /// Internal identifier for Redis storage and resolution
    /// </summary>
    public string InternalId { get; set; } = string.Empty;
    
    /// <summary>
    /// The player who owns this library collection
    /// </summary>
    public string PlayerId { get; set; } = string.Empty;
    
    
    /// <summary>
    /// Maps library source to internal ID for efficient resolution
    /// Example: { ["steam"] = "library-123", ["epic"] = "library-456" }
    /// </summary>
    public Dictionary<string, string> LibrarySourceMapping { get; set; } = new();
    
    
    /// <summary>
    /// Last time the library data was updated
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
} 