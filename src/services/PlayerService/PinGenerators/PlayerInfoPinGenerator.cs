using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;
using SteamApi.Models.Steam.Player;
using Sgnome.Clients.Steam;
using PlayerService.Actions;

namespace PlayerService.PinGenerators;

public class PlayerInfoPinGenerator
{
    private readonly ISteamClient _steamClient;

    public PlayerInfoPinGenerator(ISteamClient steamClient)
    {
        _steamClient = steamClient;
    }

    public async Task<IEnumerable<Pin>> GeneratePinsAsync(PlayerNode player, PinContext context)
    {
        var pins = new List<Pin>();

        // Get Steam-specific player info if available
        if (player.Identifiers.TryGetValue(PlayerIdentifiers.Steam, out var steamId) && !string.IsNullOrEmpty(steamId))
        {
            var playerInfoPins = await _steamClient.GetPlayerDetailsAsync(steamId, (response) => ExtractSteamPlayerInfoPins.ExtractPins(response, context));
            pins.AddRange(playerInfoPins);
        }

        return pins;
    }
} 