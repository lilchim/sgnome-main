namespace Sgnome.Models.Requests;

/// <summary>
/// Request model for selecting a library list by player
/// </summary>
public class LibraryListSelectRequest
{
    /// <summary>
    /// Player ID that owns the library list
    /// </summary>
    public string PlayerId { get; set; } = string.Empty;
    
    /// <summary>
    /// ID of the node that originated this request (for context and edge generation)
    /// </summary>
    public string? OriginNodeId { get; set; }
} 