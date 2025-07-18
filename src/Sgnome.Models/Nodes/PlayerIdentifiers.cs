namespace Sgnome.Models.Nodes;

/// <summary>
/// Standard identifier types for PlayerNode
/// </summary>
public static class PlayerIdentifiers
{
    // Platform identifiers
    public const string Steam = "steam";
    public const string Epic = "epic";
    public const string Gog = "gog";
    public const string Xbox = "xbox";
    public const string PlayStation = "playstation";
    public const string Nintendo = "nintendo";
    
    // Social identifiers
    public const string Discord = "discord";
    public const string Twitch = "twitch";
    public const string Twitter = "twitter";
    
    // Internal identifiers
    public const string Internal = "internal";
    
    /// <summary>
    /// Gets all standard identifier types
    /// </summary>
    public static IEnumerable<string> All => new[]
    {
        Steam, Epic, Gog, Xbox, PlayStation, Nintendo,
        Discord, Twitch, Twitter, Internal
    };
    
    /// <summary>
    /// Gets platform identifier types only
    /// </summary>
    public static IEnumerable<string> Platforms => new[]
    {
        Steam, Epic, Gog, Xbox, PlayStation, Nintendo
    };
    
    /// <summary>
    /// Gets social identifier types only
    /// </summary>
    public static IEnumerable<string> Social => new[]
    {
        Discord, Twitch, Twitter
    };
} 