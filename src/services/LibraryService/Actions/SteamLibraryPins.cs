using Sgnome.Models.Graph;
using SteamApi.Models.Steam.Responses;

namespace LibraryService.Actions;

/// <summary>
/// Actions for creating Library pins from Steam API responses.
/// Single responsibility: Transform Steam library data to Library pins.
/// </summary>
public static class SteamLibraryPins
{
    /// <summary>
    /// Creates library pins from Steam owned games response.
    /// </summary>
    /// <param name="response">Steam API response containing owned games</param>
    /// <param name="context">Context for pin creation</param>
    /// <returns>Collection of library pins</returns>
    public static IEnumerable<Pin> CreateLibraryPins(OwnedGamesResponse response, PinContext context)
    {
        if (response == null)
        {
            return Enumerable.Empty<Pin>();
        }

        var pins = new List<Pin>();

        // Steam library pin
        var steamLibraryPin = new Pin
        {
            Id = $"steam-library-{context.ApiParameters["playerId"]}",
            Label = $"Steam Library ({response.GameCount} games)",
            Type = "library",
            Behavior = PinBehavior.Expandable,
            Summary = new PinSummary
            {
                DisplayText = $"{response.GameCount} games on Steam",
                Count = response.GameCount,
                Icon = "steam"
            },
            Metadata = new PinMetadata
            {
                TargetNodeType = context.TargetNodeType,
                OriginNodeId = context.InputNodeId,
                ApiEndpoint = context.ApiEndpoint,
                Parameters = context.ApiParameters
            }
        };
        pins.Add(steamLibraryPin);

        return pins;
    }

    /// <summary>
    /// Creates informational library pins from Steam owned games response.
    /// Used when displaying library information (self-reference).
    /// </summary>
    /// <param name="response">Steam API response containing owned games</param>
    /// <param name="context">Context for pin creation</param>
    /// <returns>Collection of informational library pins</returns>
    public static IEnumerable<Pin> CreateLibraryInfoPins(OwnedGamesResponse response, PinContext context)
    {
        if (response == null)
        {
            return Enumerable.Empty<Pin>();
        }

        var pins = new List<Pin>();

        // Steam library informational pin
        var steamLibraryInfoPin = new Pin
        {
            Id = $"steam-library-info-{context.InputNodeId}",
            Label = $"Steam Library Info ({response.GameCount} games)",
            Type = "library",
            Behavior = PinBehavior.Informational,
            Summary = new PinSummary
            {
                DisplayText = $"{response.GameCount} games on Steam",
                Count = response.GameCount,
                Icon = "steam"
            },
            Metadata = new PinMetadata
            {
                TargetNodeType = context.TargetNodeType,
                OriginNodeId = context.InputNodeId,
                ApiEndpoint = context.ApiEndpoint,
                Parameters = context.ApiParameters
            }
        };
        pins.Add(steamLibraryInfoPin);

        return pins;
    }

    /// <summary>
    /// Gets the game count from Steam owned games response.
    /// </summary>
    /// <param name="response">Steam API response containing owned games</param>
    /// <returns>Number of games in the library</returns>
    public static int GetGameCount(OwnedGamesResponse response)
    {
        return response?.GameCount ?? 0;
    }
} 