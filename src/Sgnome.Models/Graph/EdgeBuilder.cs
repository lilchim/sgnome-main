namespace Sgnome.Models.Graph;

/// <summary>
/// Utility class for building edges between nodes
/// </summary>
public static class EdgeBuilder
{
    /// <summary>
    /// Creates an edge from a source node to a target node
    /// </summary>
    /// <param name="sourceNodeId">The source node ID</param>
    /// <param name="targetNodeId">The target node ID</param>
    /// <param name="edgeType">The type of relationship (e.g., "owns", "contains", "expands_to")</param>
    /// <param name="label">The edge label to display</param>
    /// <returns>A new Edge instance</returns>
    public static Edge CreateEdge(string sourceNodeId, string targetNodeId, string edgeType, string label)
    {
        return new Edge
        {
            Id = $"edge-{sourceNodeId}-{targetNodeId}",
            Source = sourceNodeId,
            Target = targetNodeId,
            Type = "default", // xyflow default edge type
            Data = new EdgeData
            {
                Label = label,
                EdgeType = edgeType,
                Properties = new Dictionary<string, object>
                {
                    ["sourceNodeId"] = sourceNodeId,
                    ["targetNodeId"] = targetNodeId,
                    ["createdAt"] = DateTime.UtcNow
                }
            }
        };
    }

    /// <summary>
    /// Creates an edge from a player to their library
    /// </summary>
    public static Edge CreatePlayerToLibraryEdge(string playerNodeId, string libraryNodeId)
    {
        return CreateEdge(playerNodeId, libraryNodeId, "owns", "Owns Library");
    }

    /// <summary>
    /// Creates an edge from a library to an organized library
    /// </summary>
    public static Edge CreateLibraryToOrganizedLibraryEdge(string libraryNodeId, string organizedLibraryNodeId)
    {
        return CreateEdge(libraryNodeId, organizedLibraryNodeId, "contains", "Contains");
    }

    /// <summary>
    /// Creates an edge from a player to an organized library (direct expansion)
    /// </summary>
    public static Edge CreatePlayerToOrganizedLibraryEdge(string playerNodeId, string organizedLibraryNodeId)
    {
        return CreateEdge(playerNodeId, organizedLibraryNodeId, "expands_to", "Expands To");
    }

    /// <summary>
    /// Creates an edge from a player to their libraries
    /// </summary>
    public static Edge CreatePlayerToLibrariesEdge(string playerNodeId, string librariesNodeId)
    {
        return CreateEdge(playerNodeId, librariesNodeId, "owns", "Owns Libraries");
    }
} 