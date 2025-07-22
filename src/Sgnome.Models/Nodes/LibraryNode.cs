namespace Sgnome.Models.Nodes;

/// <summary>
/// Represents a single library source (e.g., Steam, Epic, GOG) for a player.
/// 
/// Role: Manages a single library source and provides access to different views
/// of the games within that library (recently played, favorites, all games, etc.).
/// 
/// Characteristics:
/// - Belongs to a specific library source (Steam, Epic, etc.)
/// - Contains pins for different game categories/views
/// - Acts as an intermediary between Libraries and GamesList nodes
/// 
/// Workflow: Libraries -> Library (shows organized views of that library)
/// 
/// Use Cases:
/// - Player expands a library source pin from LibrariesNode
/// - Provides organized access to games within that library
/// - Enables different views of the same game collection
/// </summary>
public class LibraryNode
{
    /// <summary>
    /// Internal identifier for Redis storage and resolution
    /// </summary>
    public string InternalId { get; set; } = string.Empty;
    
    /// <summary>
    /// The library source this organized view belongs to (Steam, Epic, GOG, etc.)
    /// </summary>
    public string LibrarySource { get; set; } = string.Empty;
    
    /// <summary>
    /// Display name for this organized library view
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
    
    /// <summary>
    /// External service identifiers and relationships
    /// Example: { ["player"] = "player-internal-id", ["steam"] = "76561198012345678" }
    /// </summary>
    public Dictionary<string, string> Identifiers { get; set; } = new();
    
    /// <summary>
    /// Total number of games in this library
    /// </summary>
    public int TotalGameCount { get; set; }
    
    /// <summary>
    /// Available organization categories (recently-played, favorites, all-games, etc.)
    /// </summary>
    public List<string> AvailableCategories { get; set; } = new();
    
    /// <summary>
    /// Last time this library was updated
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
} 