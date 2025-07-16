namespace Sgnome.Models.Nodes;

/// <summary>
/// Represents a collection of game libraries for a player.
/// 
/// Role: Intermediary node that serves as an entry point to a player's various game libraries.
/// This node doesn't contain games directly, but provides access to organized library sections
/// (Steam, Epic, GOG, etc.) through expandable pins.
/// 
/// Characteristics:
/// - Always has a single player as its source
/// - Contains pins for each library source (Steam, Epic, etc.)
/// - Acts as a gateway to more detailed library organization
/// 
/// Use Cases:
/// - Player expands "Library" pin from PlayerNode
/// - Provides overview of all available game libraries
/// - Enables access to organized library sections
/// </summary>
public class LibraryNode
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
    /// Total number of games across all libraries
    /// </summary>
    public int TotalGameCount { get; set; }
    
    /// <summary>
    /// Last time the library data was updated
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
} 