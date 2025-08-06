using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;
using GamesService.Database;
using Microsoft.Extensions.Logging;
using Sgnome.Clients.Steam;
using GamesService.Actions;
using Sgnome.Services.GamesService.Consumers;

namespace GamesService;

public class GamesService : IGamesService
{
    private readonly IGamesDatabase _database;
    private readonly ILogger<GamesService> _logger;
    private readonly ISteamClient _steamClient;
    private readonly PlayerNodeConsumer _playerNodeConsumer;
    private readonly GameNodeConsumer _gameNodeConsumer;

    public GamesService(
        IGamesDatabase database,
        ISteamClient steamClient,
        GameNodeConsumer gameNodeConsumer,
        PlayerNodeConsumer playerNodeConsumer,
        ILogger<GamesService> logger)
    {
        _database = database;
        _steamClient = steamClient;
        _gameNodeConsumer = gameNodeConsumer;
        _playerNodeConsumer = playerNodeConsumer;
        _logger = logger;
    }

    public async Task<(IEnumerable<Pin> Pins, GameNode ResolvedNode)> Consume(GameNode partial) => await _gameNodeConsumer.Consume(partial);

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

                var ownedGames = await _steamClient.GetOwnedGamesAsync(library.Identifiers[LibraryIdentifiers.Steam], (response) => ExtractSteamGameReferencePins.Extract(response, context));
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

    public async Task<IEnumerable<Pin>> Consume(PlayerNode player) => await _playerNodeConsumer.Consume(player);

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

}