using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;
using PlayerService.Providers;

namespace PlayerService.PinGenerators;

public class PlayerFriendsPinGenerator
{
    private readonly ISteamPlayerProvider _steamProvider;

    public PlayerFriendsPinGenerator(ISteamPlayerProvider steamProvider)
    {
        _steamProvider = steamProvider;
    }

    public async Task<IEnumerable<Pin>> GeneratePinsAsync(PlayerNode player, PinContext context)
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
                        OriginNodeId = context.InputNodeId,
                        TargetNodeId = null,
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
} 