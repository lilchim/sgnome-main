using StackExchange.Redis;
using Sgnome.Models.Nodes;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace GamesService.Database;

public class RedisGamesDatabase : IGamesDatabase
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisGamesDatabase> _logger;
    private const string GameKeyPrefix = "game:";
    private const string GameIdIndexPrefix = "game_id:";

    public RedisGamesDatabase(IConnectionMultiplexer redis, ILogger<RedisGamesDatabase> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task<GameNode> ResolveGameAsync(Dictionary<string, object> identifiers)
    {
        var db = _redis.GetDatabase();
        
        // Try to find existing game by any identifier
        string? existingInternalId = null;
        foreach (var (key, value) in identifiers)
        {
            var indexKey = $"{GameIdIndexPrefix}{key}:{value}";
            var internalId = await db.StringGetAsync(indexKey);
            if (!internalId.IsNull)
            {
                existingInternalId = internalId!;
                break;
            }
        }

        GameNode game;
        if (existingInternalId != null)
        {
            // Found existing game, load and update identifiers
            var gameJson = await db.StringGetAsync($"{GameKeyPrefix}{existingInternalId}");
            if (!gameJson.IsNull)
            {
                game = JsonSerializer.Deserialize<GameNode>(gameJson!)!;
                
                // Update with any new identifiers
                foreach (var (key, value) in identifiers)
                {
                    if (!game.Identifiers.ContainsKey(key) || !game.Identifiers[key].Equals(value))
                    {
                        game.Identifiers[key] = value;
                    }
                }
                
                // Update the game
                await UpdateGameAsync(game);
                return game;
            }
        }

        // Create new game
        var newInternalId = Guid.NewGuid().ToString();
        game = new GameNode
        {
            InternalId = newInternalId,
            Identifiers = new Dictionary<string, object>(identifiers)
        };

        await UpdateGameAsync(game);
        return game;
    }

    public async Task<GameNode?> GetGameAsync(string internalId)
    {
        var db = _redis.GetDatabase();
        var gameJson = await db.StringGetAsync($"{GameKeyPrefix}{internalId}");
        
        if (gameJson.IsNull)
            return null;

        return JsonSerializer.Deserialize<GameNode>(gameJson!);
    }

    public async Task UpdateGameAsync(GameNode game)
    {
        var db = _redis.GetDatabase();
        var internalId = game.InternalId;
        
        if (string.IsNullOrEmpty(internalId))
        {
            throw new InvalidOperationException("Game must have an InternalId");
        }

        var gameJson = JsonSerializer.Serialize(game);
        await db.StringSetAsync($"{GameKeyPrefix}{internalId}", gameJson);

        // Update identifier indexes
        foreach (var (key, value) in game.Identifiers)
        {
            var indexKey = $"{GameIdIndexPrefix}{key}:{value}";
            await db.StringSetAsync(indexKey, internalId);
        }

        _logger.LogInformation("Updated game {InternalId} with {IdentifierCount} identifiers", 
            internalId, game.Identifiers.Count);
    }
} 