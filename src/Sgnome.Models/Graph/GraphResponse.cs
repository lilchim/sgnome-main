using Sgnome.Models.Nodes;

namespace Sgnome.Models.Graph;

public class GraphResponse
{
    public List<Node> Nodes { get; set; } = new();
    public List<Edge> Edges { get; set; } = new();
    public GraphMetadata Metadata { get; set; } = new();
}

public class GraphMetadata
{
    public string QueryType { get; set; } = string.Empty;
    public string QueryId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> Context { get; set; } = new();
} 