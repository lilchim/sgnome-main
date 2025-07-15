using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;
using UserLibraryService.Providers;
using Microsoft.Extensions.Logging;
using SteamApi.Models.Steam.Responses;

namespace UserLibraryService;

public class UserLibraryAggregator
{
    private readonly ISteamUserLibraryProvider _steamProvider;
    private readonly ILogger<UserLibraryAggregator> _logger;

    public UserLibraryAggregator(
        ISteamUserLibraryProvider steamProvider,
        ILogger<UserLibraryAggregator> logger)
    {
        _steamProvider = steamProvider;
        _logger = logger;
    }

    public async Task<GraphResponse> GetUserLibraryAsync(PlayerNode player)
    {
        var response = new GraphResponse
        {
            Metadata = new GraphMetadata
            {
                QueryType = "GetUserLibrary",
                QueryId = $"user-library-{player.SteamId}",
                Timestamp = DateTime.UtcNow,
                Context = new Dictionary<string, object>
                {
                    ["playerId"] = player.SteamId ?? "unknown",
                    ["provider"] = "Steam"
                }
            }
        };

        // Start with the player node
        var playerNode = NodeBuilder.CreatePlayerNode(player);
        response.Nodes.Add(playerNode);

        // Get Steam library data
        if (!string.IsNullOrEmpty(player.SteamId))
        {
            var steamLibrary = await _steamProvider.GetUserLibraryAsync(player.SteamId);
            if (steamLibrary != null)
            {
                await ProcessSteamLibraryAsync(response, playerNode, steamLibrary);
            }
        }

        // Add informational pins to the player node
        AddPlayerPins(playerNode, response);

        return response;
    }

    public async Task<GraphResponse> GetRecentlyPlayedGamesAsync(PlayerNode player)
    {
        var response = new GraphResponse
        {
            Metadata = new GraphMetadata
            {
                QueryType = "GetRecentlyPlayedGames",
                QueryId = $"recent-games-{player.SteamId}",
                Timestamp = DateTime.UtcNow,
                Context = new Dictionary<string, object>
                {
                    ["playerId"] = player.SteamId ?? "unknown",
                    ["provider"] = "Steam"
                }
            }
        };

        // Start with the player node
        var playerNode = NodeBuilder.CreatePlayerNode(player);
        response.Nodes.Add(playerNode);

        // Get recently played games
        if (!string.IsNullOrEmpty(player.SteamId))
        {
            var recentGames = await _steamProvider.GetRecentlyPlayedGamesAsync(player.SteamId);
            if (recentGames != null)
            {
                await ProcessRecentlyPlayedGamesAsync(response, playerNode, recentGames);
            }
        }

        return response;
    }

    private async Task ProcessSteamLibraryAsync(GraphResponse response, Node playerNode, SteamResponse<OwnedGamesResponse> steamLibraryResponse)
    {
        if (steamLibraryResponse?.Response == null)
        {
            _logger.LogWarning("No Steam library response data");
            return;
        }

        var ownedGames = steamLibraryResponse.Response;
        var games = ownedGames.Games;
        var gameCount = ownedGames.GameCount;
        
        _logger.LogInformation("Processing Steam library with {GameCount} games", gameCount);

        // Create game nodes for the library (limit to first 10 for performance)
        int processedCount = 0;
        foreach (var game in games)
        {
            if (processedCount >= 10) break;
            
            var gameNode = new GameNode
            {
                SteamAppId = game.AppId,
                Name = game.Name ?? "Unknown Game"
            };
            gameNode.Identifiers["playtime"] = game.PlaytimeForever;

            var xyflowGameNode = NodeBuilder.CreateGameNode(gameNode);
            response.Nodes.Add(xyflowGameNode);

            // Create edge from player to game
            var edge = new Edge
            {
                Id = $"edge-{playerNode.Id}-{xyflowGameNode.Id}",
                Source = playerNode.Id,
                Target = xyflowGameNode.Id,
                Type = "default",
                Data = new EdgeData
                {
                    Label = "Owns",
                    EdgeType = "owns",
                    Properties = new Dictionary<string, object>
                    {
                        ["playtime"] = game.PlaytimeForever,
                        ["playtime2Weeks"] = game.Playtime2Weeks,
                        ["lastPlayed"] = "Never" // TODO: Check if this property exists
                    }
                }
            };
            response.Edges.Add(edge);
            processedCount++;
        }

        _logger.LogInformation("Creating library pin with GameCount={GameCount}, ProcessedGames={ProcessedGames}", 
            gameCount, processedCount);
            
        // Add library pin to player node
        var libraryPin = new Pin
        {
            Id = "steam-library",
            Label = $"Steam Library ({gameCount} games)",
            Type = "library",
            Behavior = PinBehavior.Expandable,
            Summary = new PinSummary
            {
                DisplayText = $"Owns {gameCount} games on Steam",
                Count = gameCount
            },
            Metadata = new PinMetadata
            {
                ApiEndpoint = "/api/player/library",
                TargetNodeType = "game",
                Parameters = new Dictionary<string, object>
                {
                    ["gameCount"] = gameCount,
                    ["lastUpdated"] = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm")
                }
            }
        };

        playerNode.Data.Pins.Add(libraryPin);
    }

    private async Task ProcessRecentlyPlayedGamesAsync(GraphResponse response, Node playerNode, object steamRecentGamesResponse)
    {
        // Use reflection to access the Steam API response properties
        var responseType = steamRecentGamesResponse.GetType();
        var responseProperty = responseType.GetProperty("Response");
        var responseData = responseProperty?.GetValue(steamRecentGamesResponse);
        if (responseData == null)
        {
            _logger.LogWarning("Could not access Response property from Steam API response");
            return;
        }

        dynamic dynamicResponse = responseData;
        var games = dynamicResponse.Games;
        var gameCount = (int)(dynamicResponse.TotalCount ?? 0);
        
        _logger.LogInformation("Processing {GameCount} recently played games", gameCount);

        // Create game nodes for recently played games
        foreach (var game in games)
        {
            var gameNode = new GameNode
            {
                SteamAppId = game.AppId,
                Name = game.Name ?? "Unknown Game"
            };
            gameNode.Identifiers["playtime"] = game.PlaytimeForever;

            var xyflowGameNode = NodeBuilder.CreateGameNode(gameNode);
            response.Nodes.Add(xyflowGameNode);

            // Create edge from player to game
            var edge = new Edge
            {
                Id = $"edge-{playerNode.Id}-{xyflowGameNode.Id}",
                Source = playerNode.Id,
                Target = xyflowGameNode.Id,
                Type = "default",
                Data = new EdgeData
                {
                    Label = "Recently Played",
                    EdgeType = "recently-played",
                    Properties = new Dictionary<string, object>
                    {
                        ["playtime2Weeks"] = game.Playtime2Weeks ?? 0,
                        ["lastPlayed"] = "Unknown" // TODO: Check if this property exists
                    }
                }
            };
            response.Edges.Add(edge);
        }

        // Add recently played pin to player node
        var recentPin = new Pin
        {
            Id = "recently-played",
            Label = $"Recently Played ({gameCount} games)",
            Type = "recently-played",
            Behavior = PinBehavior.Expandable,
            Summary = new PinSummary
            {
                DisplayText = $"Played {gameCount} games recently",
                Count = gameCount
            },
            Metadata = new PinMetadata
            {
                ApiEndpoint = "/api/player/recent",
                TargetNodeType = "game",
                Parameters = new Dictionary<string, object>
                {
                    ["gameCount"] = gameCount
                }
            }
        };

        playerNode.Data.Pins.Add(recentPin);
    }

    private void AddPlayerPins(Node playerNode, GraphResponse response)
    {
        // Add informational pins
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

        playerNode.Data.Pins.Add(infoPin);

        // Add expandable pins for future features
        var friendsPin = new Pin
        {
            Id = "friends",
            Label = "Friends",
            Type = "friends",
            Behavior = PinBehavior.Expandable,
            Summary = new PinSummary
            {
                DisplayText = "View player's friends"
            },
            Metadata = new PinMetadata
            {
                ApiEndpoint = "/api/player/friends",
                TargetNodeType = "player"
            }
        };

        playerNode.Data.Pins.Add(friendsPin);
    }
} 