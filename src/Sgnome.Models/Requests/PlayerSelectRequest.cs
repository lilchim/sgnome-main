namespace Sgnome.Models.Requests;

/// <summary>
/// Request model for selecting a player by identifiers
/// </summary>
public class PlayerSelectRequest
{
    /// <summary>
    /// Internal ID of the player (if known)
    /// </summary>
    public string? InternalId { get; set; }

    /// <summary>
    /// Dictionary of identifiers to resolve the player
    /// 
    /// Valid identifier keys:
    /// - "steam:steamId" - Steam 64-bit ID (e.g., 76561197995791208)
    /// - "steam:vanityUrl" - Steam vanity URL name
    /// - "epic:epicId" - Epic Games Store ID
    /// - "internalId" - Internal system ID (if known)
    /// </summary>
    public Dictionary<string, string> Identifiers { get; set; } = new();

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