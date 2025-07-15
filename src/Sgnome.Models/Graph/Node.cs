namespace Sgnome.Models.Graph;

// Represents the xyflow Node structure
public class Node
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public double X { get; set; }
    public double Y { get; set; }
    public NodeData Data { get; set; } = new();
}

// Our custom data that goes in the xyflow Node.data field
public class NodeData
{
    public string Label { get; set; } = string.Empty;
    public string NodeType { get; set; } = string.Empty; // "player", "game", "publisher", etc.
    public Dictionary<string, object> Properties { get; set; } = new();
    public List<Pin> Pins { get; set; } = new();
    public NodeState State { get; set; } = NodeState.Loading;
}

public enum NodeState
{
    Loading,
    Loaded,
    Error
} 