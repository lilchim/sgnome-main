using System.Diagnostics;
using GamesService.Actions;
using GamesService.Database;
using Microsoft.Extensions.Logging;
using Sgnome.Clients.Steam;
using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;

public class GameNodeConsumer
{
    private readonly IGamesDatabase _database;
    private readonly ISteamClient _steamClient;
    private readonly ILogger<GameNodeConsumer> _logger;

    public GameNodeConsumer(
        IGamesDatabase database,
        ISteamClient steamClient,
        ILogger<GameNodeConsumer> logger)
    {
        _database = database;
        _steamClient = steamClient;
        _logger = logger;
    }

    public async Task<(IEnumerable<Pin> Pins, GameNode ResolvedNode)> Consume(GameNode partial)
    {
        _logger.LogInformation("Consuming GameNode");

        try
        {
            // Resolve game from database (creates or updates as needed)
            var resolvedGame = await _database.ResolveGameAsync(partial.Identifiers);
            _logger.LogInformation("Resolved game with internal ID {InternalId}", resolvedGame.InternalId);

            // var pins = await CreateGamePinsAsync(resolvedGame, resolvedGame.InternalId!, "game");
            var pins = new List<Pin>();

            var steamPins = await ProcessSteamGame(resolvedGame);
            pins.AddRange(steamPins);

            return (pins, resolvedGame);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consuming GameNode");
            throw;
        }
    }

    private async Task<IEnumerable<Pin>> ProcessSteamGame(GameNode game)
    {
        var pins = new List<Pin>();
        var context = new PinContext { InputNodeId = game.InternalId!, TargetNodeType = "game" };

        var steamAppId = game.Identifiers.FirstOrDefault(i => i.Key == "steam").Value.ToString();
        if (steamAppId != null)
        {
            _logger.LogDebug("This is a steam gaben.");
            var steamGamePins = await _steamClient.GetAppDetailsAsync<IEnumerable<Pin>>(steamAppId, (response) => ExtractSteamGameInfoPins.Extract(response, context));
            pins.AddRange(steamGamePins);
        }

        return pins;
    }
}   