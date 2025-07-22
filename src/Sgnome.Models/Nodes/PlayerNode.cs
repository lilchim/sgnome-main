namespace Sgnome.Models.Nodes;

// Minimal player identity - relational data goes in pins
public class PlayerNode
{
    public string? InternalId { get; set; }
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
    
    // Core identity properties - all external IDs stored here
    public Dictionary<string, string> Identifiers { get; set; } = new();
} 