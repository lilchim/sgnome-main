namespace Sgnome.Models.Graph;

// Represents the xyflow Edge structure
public class Edge
{
    public string Id { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public EdgeData Data { get; set; } = new();
}

// Our custom data that goes in the xyflow Edge.data field
public class EdgeData
{
    public string Label { get; set; } = string.Empty;
    public string EdgeType { get; set; } = string.Empty; // "owns", "published", "developed", etc.
    public Dictionary<string, object> Properties { get; set; } = new();
} 