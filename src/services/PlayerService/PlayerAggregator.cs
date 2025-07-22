using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;
using PlayerService.Providers;
using Microsoft.Extensions.Logging;

namespace PlayerService;

public class PlayerAggregator
{
    private readonly ISteamPlayerProvider _steamProvider;
    private readonly ILogger<PlayerAggregator> _logger;

    public PlayerAggregator(
        ISteamPlayerProvider steamProvider,
        ILogger<PlayerAggregator> logger)
    {
        _steamProvider = steamProvider;
        _logger = logger;
    }

    public async Task<IEnumerable<Pin>> GetPlayerInfoPinsAsync(PlayerNode player)
    {
        var pins = new List<Pin>();

        // Add basic player info pin
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

        // Get Steam-specific player info if available
        if (player.Identifiers.TryGetValue(PlayerIdentifiers.Steam, out var steamId) && !string.IsNullOrEmpty(steamId))
        {
            var steamPlayerData = await _steamProvider.GetPlayerDataAsync(steamId);
            if (steamPlayerData != null)
            {
                var steamPins = CreateSteamPlayerInfoPins(player, steamPlayerData);
                pins.AddRange(steamPins);
            }
        }

        return pins;
    }

    public async Task<IEnumerable<Pin>> GetFriendsPinsAsync(PlayerNode player)
    {
        var pins = new List<Pin>();

        // Get Steam friends if available
        if (player.Identifiers.TryGetValue(PlayerIdentifiers.Steam, out var steamId) && !string.IsNullOrEmpty(steamId))
        {
            var steamFriendsData = await _steamProvider.GetFriendsDataAsync(steamId);
            if (steamFriendsData != null)
            {
                var friendsPin = new Pin
                {
                    Id = "friends",
                    Label = "Friends",
                    Type = "friends",
                    Behavior = PinBehavior.Expandable,
                    Summary = new PinSummary
                    {
                        DisplayText = $"View player's friends ({steamFriendsData.FriendCount})",
                        Count = steamFriendsData.FriendCount
                    },
                    Metadata = new PinMetadata
                    {
                        TargetNodeType = "player",
                        OriginNodeId = $"player-{player.InternalId}",
                        ApiEndpoint = "/api/player/friends",
                        Parameters = new Dictionary<string, object>
                        {
                            ["playerId"] = player.InternalId ?? "unknown"
                        }
                    }
                };
                pins.Add(friendsPin);
            }
        }

        return pins;
    }

    public async Task<IEnumerable<Pin>> GetActivityPinsAsync(PlayerNode player)
    {
        var pins = new List<Pin>();

        // Get Steam activity if available
        if (player.Identifiers.TryGetValue(PlayerIdentifiers.Steam, out var steamId) && !string.IsNullOrEmpty(steamId))
        {
            var steamActivityData = await _steamProvider.GetActivityDataAsync(steamId);
            if (steamActivityData != null)
            {
                var activityPin = new Pin
                {
                    Id = "recent-activity",
                    Label = "Recent Activity",
                    Type = "activity",
                    Behavior = PinBehavior.Expandable,
                    Summary = new PinSummary
                    {
                        DisplayText = "View recent activity",
                        Icon = "clock"
                    },
                    Metadata = new PinMetadata
                    {
                        TargetNodeType = "activity",
                        OriginNodeId = $"player-{player.InternalId}",
                        ApiEndpoint = "/api/player/activity",
                        Parameters = new Dictionary<string, object>
                        {
                            ["playerId"] = player.InternalId ?? "unknown"
                        }
                    }
                };
                pins.Add(activityPin);
            }
        }

        return pins;
    }

    private IEnumerable<Pin> CreateSteamPlayerInfoPins(PlayerNode player, SteamPlayerData steamPlayerData)
    {
        var pins = new List<Pin>();

        // Add Steam-specific info pins
        if (steamPlayerData.ProfileVisibility != null)
        {
            var visibilityPin = new Pin
            {
                Id = "profile-visibility",
                Label = "Profile Visibility",
                Type = "profile-info",
                Behavior = PinBehavior.Informational,
                Summary = new PinSummary
                {
                    DisplayText = $"Profile: {steamPlayerData.ProfileVisibility}",
                    Icon = "eye"
                }
            };
            pins.Add(visibilityPin);
        }

        return pins;
    }
} 