namespace Sgnome.Models.Nodes;

// Minimal publisher identity - relational data goes in pins
public class PublisherNode
{
    public string? Name { get; set; }
    public string? LogoUrl { get; set; }
    public string? Website { get; set; }
    
    // Core identity properties only
    public Dictionary<string, object> Identifiers { get; set; } = new();
} 