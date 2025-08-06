using GamesService.Actions;
using GamesService.Database;
using Microsoft.Extensions.Logging;
using Sgnome.Clients.Steam;
using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;

namespace Sgnome.Services.GamesService.Consumers;

public class PlayerNodeConsumer
{
    private readonly IGamesDatabase _database;
    private readonly ISteamClient _steamClient;
    private readonly ILogger<PlayerNodeConsumer> _logger;

    public PlayerNodeConsumer(
        IGamesDatabase database,
        ISteamClient steamClient,
        ILogger<PlayerNodeConsumer> logger)
    {
        _database = database;
        _steamClient = steamClient;
        _logger = logger;
    }

    public async Task<IEnumerable<Pin>> Consume(PlayerNode partial)
    {
        _logger.LogInformation("Consuming PlayerNode");

        try
        {
            var pins = new List<Pin>();

            var steamPins = await ProcessSteamPlayer(partial);
            pins.AddRange(steamPins);

            return pins;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consuming PlayerNode");
            throw;
        }
    }

    private async Task<IEnumerable<Pin>> ProcessSteamPlayer(PlayerNode partial)
    {
        var context = new PinContext { InputNodeId = partial.InternalId!, TargetNodeType = "game" };
        var recentlyPlayedPins = await _steamClient.GetRecentlyPlayedGamesAsync(partial.Identifiers["steam"], (response) => ExtractSteamGameReferencePins.Extract(response, context));
        return recentlyPlayedPins;
    }
}