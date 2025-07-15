namespace Sgnome.Models.Nodes;

// Minimal game identity - relational data goes in pins
public class GameNode
{
    public int? SteamAppId { get; set; }
    public string? EpicId { get; set; }
    public string? Name { get; set; }
    public string? IconUrl { get; set; }
    public string? LogoUrl { get; set; }
    
    // Core identity properties only
    public Dictionary<string, object> Identifiers { get; set; } = new();
} 