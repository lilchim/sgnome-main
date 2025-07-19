namespace Sgnome.Models.Nodes;

/// <summary>
/// Standard identifier types for library nodes
/// </summary>
public static class LibraryIdentifiers
{
    // Library sources
    public const string Steam = "steam";
    public const string Epic = "epic";
    public const string Gog = "gog";
    public const string Xbox = "xbox";
    public const string PlayStation = "playstation";
    public const string Nintendo = "nintendo";
    public const string Custom = "custom";
    
    // Internal identifiers
    public const string Internal = "internal";
    
    /// <summary>
    /// Gets all library source types
    /// </summary>
    public static IEnumerable<string> AllSources => new[]
    {
        Steam, Epic, Gog, Xbox, PlayStation, Nintendo, Custom
    };
    
    /// <summary>
    /// Gets platform library sources only
    /// </summary>
    public static IEnumerable<string> PlatformSources => new[]
    {
        Steam, Epic, Gog, Xbox, PlayStation, Nintendo
    };
} 