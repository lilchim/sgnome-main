using Sgnome.Models.Graph;

namespace Sgnome.Models.Nodes;

// Minimal game identity - relational data goes in pins
public class GameNode : Node
{
    public string? InternalId { get; set; }
    public string? Name { get; set; }
    public string? IconUrl { get; set; }
    public string? LogoUrl { get; set; }
    
    // Core identity properties only
    public Dictionary<string, object> Identifiers { get; set; } = new();

    public GameNode()
    {
        Type = "gameNode";
        Data.NodeType = "game";
    }
} 