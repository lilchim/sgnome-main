namespace Sgnome.Models.Graph;

/// <summary>
/// Provides context to providers about how to create pins
/// </summary>
public class PinContext
{
    /// <summary>
    /// The ID of the input node (becomes OriginNodeId in the pin)
    /// </summary>
    public string InputNodeId { get; set; } = string.Empty;
    
    /// <summary>
    /// The type of the input node (used to determine Pin.Behavior)
    /// </summary>
    public string InputNodeType { get; set; } = string.Empty;
    
    /// <summary>
    /// The target node type for the pin (always the provider's domain)
    /// </summary>
    public string TargetNodeType { get; set; } = string.Empty;
    
    /// <summary>
    /// The API endpoint for expanding the pin (domain-specific constant)
    /// </summary>
    public string? ApiEndpoint { get; set; }
    
    /// <summary>
    /// The API parameters required to reach the target node
    /// </summary>
    public Dictionary<string, object> ApiParameters { get; set; } = new();
} 