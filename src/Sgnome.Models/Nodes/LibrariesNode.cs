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
public class LibrariesNode
{
    /// <summary>
    /// The player who owns this library collection
    /// </summary>
    public string PlayerId { get; set; } = string.Empty;
    
    /// <summary>
    /// Display name for the library collection
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
    
    /// <summary>
    /// Available library sources (Steam, Epic, GOG, etc.)
    /// </summary>
    public List<string> AvailableSources { get; set; } = new();
    
    /// <summary>
    /// Internal IDs of the LibraryNode instances that belong to this collection
    /// </summary>
    public List<string> LibraryIds { get; set; } = new();
    
    /// <summary>
    /// Total number of games across all libraries
    /// </summary>
    public int TotalGameCount { get; set; }
    
    /// <summary>
    /// Last time the library data was updated
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
} 