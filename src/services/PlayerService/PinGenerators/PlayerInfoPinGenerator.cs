using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;
using PlayerService.Providers;

namespace PlayerService.PinGenerators;

public class PlayerInfoPinGenerator
{
    private readonly ISteamPlayerProvider _steamProvider;

    public PlayerInfoPinGenerator(ISteamPlayerProvider steamProvider)
    {
        _steamProvider = steamProvider;
    }

    public async Task<IEnumerable<Pin>> GeneratePinsAsync(PlayerNode player, PinContext context)
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
            },
            Metadata = new PinMetadata
            {
                OriginNodeId = context.InputNodeId,
                TargetNodeId = null,
                TargetNodeType = context.TargetNodeType
            }
        };
        pins.Add(infoPin);

        // Get Steam-specific player info if available
        if (player.Identifiers.TryGetValue(PlayerIdentifiers.Steam, out var steamId) && !string.IsNullOrEmpty(steamId))
        {
            var steamPlayerData = await _steamProvider.GetPlayerDataAsync(steamId);
            if (steamPlayerData != null)
            {
                var steamPins = CreateSteamPlayerInfoPins(player, steamPlayerData, context);
                pins.AddRange(steamPins);
            }
        }

        return pins;
    }

    private IEnumerable<Pin> CreateSteamPlayerInfoPins(PlayerNode player, SteamPlayerData steamPlayerData, PinContext context)
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
                },
                Metadata = new PinMetadata
                {
                    OriginNodeId = context.InputNodeId,
                    TargetNodeId = null,
                    TargetNodeType = context.TargetNodeType
                }
            };
            pins.Add(visibilityPin);
        }

        return pins;
    }
} 