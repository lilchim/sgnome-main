namespace Sgnome.Models.Nodes;

// Minimal player identity - relational data goes in pins
public class PlayerNode
{
    public string? SteamId { get; set; }
    public string? EpicId { get; set; }
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
    
    // Core identity properties only
    public Dictionary<string, object> Identifiers { get; set; } = new();
} 