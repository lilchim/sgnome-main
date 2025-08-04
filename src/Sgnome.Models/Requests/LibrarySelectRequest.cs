namespace Sgnome.Models.Requests;

/// <summary>
/// Request model for selecting a library by player and source
/// </summary>
public class LibrarySelectRequest
{
    /// <summary>
    /// Player ID that owns the library
    /// </summary>
    public string PlayerId { get; set; } = string.Empty;

    /// <summary>
    /// Library source (steam, epic, etc.)
    /// </summary>
    public string LibrarySource { get; set; } = string.Empty;

    /// <summary>
    /// ID of the node that originated this request (for context and edge generation)
    /// </summary>
    public string? OriginNodeId { get; set; }
    
    /// <summary>
    /// X position of the game node
    /// Default to 0
    /// </summary>
    public int X { get; set; } = 0;

    /// <summary>
    /// Y position of the game node
    /// Default to 0
    /// </summary>
    public int Y { get; set; } = 0;
}