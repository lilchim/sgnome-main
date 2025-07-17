using System.Text.Json.Serialization;

namespace Sgnome.Models.Graph;

// Represents an expandable relationship or inline data
public class Pin
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "recently-played", "publisher", "release-date", etc.
    public PinState State { get; set; } = PinState.Unexpanded;
    public PinBehavior Behavior { get; set; } = PinBehavior.Expandable;
    
    // Summary data to display inline (e.g., "Recently Played (5 games)" or "Released: 2020-12-10")
    public PinSummary Summary { get; set; } = new();
    
    // Metadata for expansion (only used if Behavior is Expandable)
    public PinMetadata? Metadata { get; set; }
}

public class PinSummary
{
    public string DisplayText { get; set; } = string.Empty;
    public int? Count { get; set; }
    public string? Icon { get; set; }
    public Dictionary<string, object> Preview { get; set; } = new();
}

public class PinMetadata
{
    public string TargetNodeType { get; set; } = string.Empty; // "game", "publisher", etc.
    public string? TargetNodeId { get; set; } // If we know the specific node
    public string? OriginNodeId { get; set; } // The source node that owns this pin
    public string? ApiEndpoint { get; set; } // "/api/player/{id}/recently-played" (null for informational pins)
    public Dictionary<string, object> Parameters { get; set; } = new();
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PinState
{
    Unexpanded,
    Loading,
    Expanded
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PinBehavior
{
    Expandable,    // Can be clicked to create new nodes/edges
    Informational  // Just displays data inline, no expansion
} 