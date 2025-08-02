using Sgnome.Models.Graph;

namespace Sgnome.Models.Nodes;

// Minimal game identity - relational data goes in pins
public class GameNode : Node
{
    public string? InternalId { get; set; }
    
    // Core identity properties only
    public Dictionary<string, object> Identifiers { get; set; } = [];

    public GameNode()
    {
        Type = NodeConstants.NodeTypes.Game;
        Data.NodeType = NodeConstants.NodeTypes.Game;
    }
} 