namespace Sgnome.Models.Requests;

/// <summary>
/// Request model for selecting a game by identifiers
/// </summary>
public class GameSelectRequest
{
    /// <summary>
    /// Internal ID of the game (if known)
    /// </summary>
    public string? InternalId { get; set; }

    /// <summary>
    /// Dictionary of identifiers to resolve the game
    /// 
    /// Valid identifier keys:
    /// - "storefront:steam" - Steam App ID (e.g., 730 for CS2)
    /// - "storefront:epic" - Epic Games Store ID
    /// - "storefront:gog" - GOG.com ID
    /// - "rawg:id" - RAWG database ID
    /// - "internalId" - Internal system ID (if known)
    /// </summary>
    public Dictionary<string, object> Identifiers { get; set; } = new();

    /// <summary>
    /// ID of the node that originated this request (for context)
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