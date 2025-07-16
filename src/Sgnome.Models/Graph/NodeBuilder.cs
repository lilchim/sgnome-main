using Sgnome.Models.Nodes;
using System.Text.Json;

namespace Sgnome.Models.Graph;

// Helper to convert domain types to xyflow Nodes
public static class NodeBuilder
{
    public static Node CreatePlayerNode(PlayerNode player, double x = 300, double y = 200)
    {
        return new Node
        {
            Id = $"player-{player.SteamId ?? player.EpicId ?? Guid.NewGuid().ToString()}",
            Type = "default", // xyflow node type
            Position = new Position { X = x, Y = y },
            Data = new NodeData
            {
                Label = player.DisplayName ?? "Unknown Player",
                NodeType = "player",
                Properties = SerializeToDictionary(player),
                Pins = new List<Pin>(), // Will be populated by services
                State = NodeState.Loading
            }
        };
    }

    public static Node CreateGameNode(GameNode game, double x = 400, double y = 300)
    {
        return new Node
        {
            Id = $"game-{(game.SteamAppId?.ToString() ?? game.EpicId ?? Guid.NewGuid().ToString())}",
            Type = "default",
            Position = new Position { X = x, Y = y },
            Data = new NodeData
            {
                Label = game.Name ?? "Unknown Game",
                NodeType = "game",
                Properties = SerializeToDictionary(game),
                Pins = new List<Pin>(),
                State = NodeState.Loading
            }
        };
    }

    public static Node CreatePublisherNode(PublisherNode publisher, double x = 500, double y = 400)
    {
        return new Node
        {
            Id = $"publisher-{publisher.Name?.ToLower().Replace(" ", "-") ?? Guid.NewGuid().ToString()}",
            Type = "default",
            Position = new Position { X = x, Y = y },
            Data = new NodeData
            {
                Label = publisher.Name ?? "Unknown Publisher",
                NodeType = "publisher",
                Properties = SerializeToDictionary(publisher),
                Pins = new List<Pin>(),
                State = NodeState.Loading
            }
        };
    }

    public static Node CreateLibraryNode(LibraryNode library, double x = 400, double y = 200)
    {
        return new Node
        {
            Id = $"library-{library.PlayerId}",
            Type = "default",
            Position = new Position { X = x, Y = y },
            Data = new NodeData
            {
                Label = library.DisplayName ?? "Game Libraries",
                NodeType = "library",
                Properties = SerializeToDictionary(library),
                Pins = new List<Pin>(),
                State = NodeState.Loading
            }
        };
    }

    public static Node CreateOrganizedLibraryNode(OrganizedLibraryNode organizedLibrary, double x = 500, double y = 200)
    {
        return new Node
        {
            Id = $"organized-library-{organizedLibrary.LibrarySource}-{organizedLibrary.PlayerId}",
            Type = "default",
            Position = new Position { X = x, Y = y },
            Data = new NodeData
            {
                Label = organizedLibrary.DisplayName ?? $"{organizedLibrary.LibrarySource} Library",
                NodeType = "organized-library",
                Properties = SerializeToDictionary(organizedLibrary),
                Pins = new List<Pin>(),
                State = NodeState.Loading
            }
        };
    }

    public static Node CreateGamesListNode(GamesListNode gamesList, double x = 600, double y = 200)
    {
        return new Node
        {
            Id = $"games-list-{gamesList.Id}",
            Type = "default",
            Position = new Position { X = x, Y = y },
            Data = new NodeData
            {
                Label = gamesList.DisplayName ?? $"Games List ({gamesList.GameCount})",
                NodeType = "games-list",
                Properties = SerializeToDictionary(gamesList),
                Pins = new List<Pin>(),
                State = NodeState.Loading
            }
        };
    }

    private static Dictionary<string, object> SerializeToDictionary<T>(T obj)
    {
        var json = JsonSerializer.Serialize(obj);
        return JsonSerializer.Deserialize<Dictionary<string, object>>(json) ?? new();
    }
} 