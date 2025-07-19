namespace Sgnome.Models.Nodes;

/// <summary>
/// Represents a collection of games that can be displayed and interacted with.
/// 
/// Role: Universal container for any list of games, regardless of source or purpose.
/// This node can represent games from a library, custom lists, recommendations,
/// search results, or any other game collection.
/// 
/// Characteristics:
/// - Universal - works for any source of games
/// - Composable - can be combined with other GamesLists
/// - Interactive - supports sorting, filtering, and individual game expansion
/// - Reusable - same structure for libraries, custom lists, recommendations
/// 
/// Use Cases:
/// - Displaying games from a library category (recently played, favorites)
/// - Custom game lists created by users
/// - Recommendation results
/// - Search results
/// - Composite lists created by combining other lists
/// </summary>
public class GamesListNode
{
    /// <summary>
    /// Unique identifier for this games list
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Display name for this games list
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;
    
    /// <summary>
    /// Source of this games list (library, user-created, system-generated, etc.)
    /// </summary>
    public string Source { get; set; } = string.Empty;
    
    /// <summary>
    /// Type of list (recently-played, favorites, all-games, custom, etc.)
    /// </summary>
    public string ListType { get; set; } = string.Empty;
    
    /// <summary>
    /// Number of games in this list
    /// </summary>
    public int GameCount { get; set; }
    
    /// <summary>
    /// Game IDs in this list (for reference, actual games come from pins)
    /// </summary>
    public List<string> GameIds { get; set; } = new();
    
    /// <summary>
    /// Metadata about this list (filters, sorting, etc.)
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
    
    /// <summary>
    /// When this list was created or last updated
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
} 