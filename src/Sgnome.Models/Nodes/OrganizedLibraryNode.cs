namespace Sgnome.Models.Nodes;

/// <summary>
/// Represents an organized view of a specific game library (e.g., Steam, Epic, GOG).
/// 
/// Role: Provides categorized access to games within a specific library source.
/// This node contains pins for different ways to view and organize the games
/// (recently played, favorites, all games, etc.).
/// 
/// Characteristics:
/// - Belongs to a specific library source (Steam, Epic, etc.)
/// - Contains pins for different game categories/views
/// - Acts as an intermediary between Library and GamesList nodes
/// 
/// Use Cases:
/// - Player expands a library source pin from LibraryNode
/// - Provides organized access to games within that library
/// - Enables different views of the same game collection
/// </summary>
public class OrganizedLibraryNode
{
    /// <summary>
    /// The library source this organized view belongs to (Steam, Epic, GOG, etc.)
    /// </summary>
    public string LibrarySource { get; set; } = string.Empty;
    
    /// <summary>
    /// The player who owns this library
    /// </summary>
    public string PlayerId { get; set; } = string.Empty;
    
    /// <summary>
    /// Display name for this organized library view
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
    
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