namespace Sgnome.Models.Nodes;

/// <summary>
/// Standard identifier types for GamesList nodes
/// </summary>
public static class GamesListIdentifiers
{
    // List types
    public const string RecentlyPlayed = "recently-played";
    public const string Favorites = "favorites";
    public const string AllGames = "all-games";
    public const string Custom = "custom";
    public const string Recommendations = "recommendations";
    public const string SearchResults = "search-results";
    
    // Sources
    public const string Library = "library";
    public const string UserCreated = "user-created";
    public const string SystemGenerated = "system-generated";
    
    // Internal identifiers
    public const string Internal = "internal";
    
    /// <summary>
    /// Gets all list types
    /// </summary>
    public static IEnumerable<string> AllListTypes => new[]
    {
        RecentlyPlayed, Favorites, AllGames, Custom, Recommendations, SearchResults
    };
    
    /// <summary>
    /// Gets all sources
    /// </summary>
    public static IEnumerable<string> AllSources => new[]
    {
        Library, UserCreated, SystemGenerated
    };
} 