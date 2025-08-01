using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;
using GamesService.Database;
using GamesService.PinGenerators;
using Microsoft.Extensions.Logging;
using Sgnome.Clients.Steam;
using GamesService.Actions;

namespace GamesService;

public class GamesService : IGamesService
{
    private readonly IGamesDatabase _database;
    private readonly GameInfoPinGenerator _infoPinGenerator;
    private readonly ILogger<GamesService> _logger;
    private readonly ISteamClient _steamClient;

    public GamesService(
        IGamesDatabase database,
        GameInfoPinGenerator infoPinGenerator,
        ISteamClient steamClient,
        ILogger<GamesService> logger)
    {
        _database = database;
        _infoPinGenerator = infoPinGenerator;
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

            // Generate enrichment pins for the resolved game
            var pins = await CreateGamePinsAsync(resolvedGame, resolvedGame.InternalId!, "game");

            return (pins, resolvedGame);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consuming GameNode");
            throw;
        }
    }

    public async Task<IEnumerable<Pin>> Consume(LibraryNode library)
    {
        _logger.LogInformation("Consuming LibraryNode for game data");

        try
        {
            var pins = new List<Pin>();

            if (library.LibrarySource == LibraryIdentifiers.Steam)
            {
                _logger.LogInformation("Processing Steam library");
                var context = new PinContext
                {
                    InputNodeId = library.InternalId,
                    InputNodeType = "library",
                    TargetNodeType = "game",
                    ApiEndpoint = "/api/game/select"
                };

                var ownedGames = await _steamClient.GetOwnedGamesAsync(library.Identifiers[LibraryIdentifiers.Steam], (response) => ExtractSteamGamePins.Extract(response, context));
                pins.AddRange(ownedGames);
            }

            return pins;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consuming LibraryNode for game data");
            throw;
        }
    }

    public async Task<IEnumerable<Pin>> Consume(PlayerNode player)
    {
        _logger.LogInformation("Consuming PlayerNode for game data");

        try
        {
            var pins = new List<Pin>();

            // For now, we don't have specific game-related pins for players
            // This could be expanded to show favorite games, recently played, etc.

            return pins;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consuming PlayerNode for game data");
            throw;
        }
    }

    public async Task<IEnumerable<Pin>> Consume(GamesListNode gamesList)
    {
        _logger.LogInformation("Consuming GamesListNode for game data");

        try
        {
            var pins = new List<Pin>();

            // For now, we don't have specific game-related pins for games lists
            // This could be expanded to show game statistics, popular games, etc.

            return pins;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consuming GamesListNode for game data");
            throw;
        }
    }

    private async Task<IEnumerable<Pin>> CreateGamePinsAsync(GameNode game, string originNodeId, string originNodeType)
    {
        var context = new PinContext
        {
            InputNodeId = originNodeId,
            InputNodeType = originNodeType,
            TargetNodeType = "game",
            ApiEndpoint = "/api/game/select",
            ApiParameters = new Dictionary<string, object>
            {
                ["internalId"] = game.InternalId!
            }
        };

        var allPins = new List<Pin>();

        // Generate info pins
        var infoPins = await _infoPinGenerator.GeneratePinsAsync(game);
        allPins.AddRange(infoPins);

        return allPins;
    }
}