using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;
using UserLibraryService.Providers;
using Microsoft.Extensions.Logging;
using SteamApi.Models.Steam.Responses;

namespace UserLibraryService;

public class UserLibraryAggregator
{
    private readonly ISteamUserLibraryProvider _steamProvider;
    private readonly ILogger<UserLibraryAggregator> _logger;

    public UserLibraryAggregator(
        ISteamUserLibraryProvider steamProvider,
        ILogger<UserLibraryAggregator> logger)
    {
        _steamProvider = steamProvider;
        _logger = logger;
    }

    public async Task<IEnumerable<Pin>> GetUserLibraryPinsAsync(PlayerNode player)
    {
        var pins = new List<Pin>();

        // Get Steam library data
        if (!string.IsNullOrEmpty(player.SteamId))
        {
            var steamLibrary = await _steamProvider.GetUserLibraryAsync(player.SteamId);
            if (steamLibrary != null)
            {
                var steamPins = ProcessSteamLibraryAsync(player, steamLibrary);
                pins.AddRange(steamPins);
            }
        }

        // Add informational pins
        var infoPins = CreatePlayerInfoPins(player);
        pins.AddRange(infoPins);

        return pins;
    }

    private IEnumerable<Pin> ProcessSteamLibraryAsync(PlayerNode player, SteamResponse<OwnedGamesResponse> steamLibraryResponse)
    {
        if (steamLibraryResponse?.Response == null)
        {
            _logger.LogWarning("No Steam library response data");
            return Enumerable.Empty<Pin>();
        }

        var ownedGames = steamLibraryResponse.Response;
        var gameCount = ownedGames.GameCount;
        
        _logger.LogInformation("Creating library pin with GameCount={GameCount}", gameCount);
            
        // Create library pin
        var libraryPin = new Pin
        {
            Id = "steam-library",
            Label = $"Steam Library ({gameCount} games)",
            Type = "library",
            Behavior = PinBehavior.Expandable,
            Summary = new PinSummary
            {
                DisplayText = $"Owns {gameCount} games on Steam",
                Count = gameCount
            },
            Metadata = new PinMetadata
            {
                TargetNodeType = "library",
                OriginNodeId = $"player-{player.SteamId}",
                ApiEndpoint = "/api/player/library",
                Parameters = new Dictionary<string, object>
                {
                    ["steamId"] = player.SteamId!,
                    ["gameCount"] = gameCount,
                    ["lastUpdated"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm")
                }
            }
        };

        return new[] { libraryPin };
    }

    private IEnumerable<Pin> CreatePlayerInfoPins(PlayerNode player)
    {
        var pins = new List<Pin>();

        // Add informational pin
        var infoPin = new Pin
        {
            Id = "player-info",
            Label = "Player Info",
            Type = "player-info",
            Behavior = PinBehavior.Informational,
            Summary = new PinSummary
            {
                DisplayText = "Basic player information"
            }
        };

        pins.Add(infoPin);

        // Add expandable pin for friends
        var friendsPin = new Pin
        {
            Id = "friends",
            Label = "Friends",
            Type = "friends",
            Behavior = PinBehavior.Expandable,
            Summary = new PinSummary
            {
                DisplayText = "View player's friends"
            },
            Metadata = new PinMetadata
            {
                TargetNodeType = "player",
                OriginNodeId = $"player-{player.SteamId}",
                ApiEndpoint = "/api/player/friends",
                Parameters = new Dictionary<string, object>
                {
                    ["steamId"] = player.SteamId!
                }
            }
        };

        pins.Add(friendsPin);

        return pins;
    }
} 