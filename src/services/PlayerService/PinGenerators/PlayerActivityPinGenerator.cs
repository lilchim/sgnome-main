using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;
using PlayerService.Providers;

namespace PlayerService.PinGenerators;

public class PlayerActivityPinGenerator
{
    private readonly ISteamPlayerProvider _steamProvider;

    public PlayerActivityPinGenerator(ISteamPlayerProvider steamProvider)
    {
        _steamProvider = steamProvider;
    }

    public async Task<IEnumerable<Pin>> GeneratePinsAsync(PlayerNode player, PinContext context)
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
                        OriginNodeId = context.InputNodeId,
                        TargetNodeId = null,
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
} 