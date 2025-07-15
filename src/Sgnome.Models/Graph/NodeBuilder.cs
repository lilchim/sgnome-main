using Sgnome.Models.Nodes;
using System.Text.Json;

namespace Sgnome.Models.Graph;

// Helper to convert domain types to xyflow Nodes
public static class NodeBuilder
{
    public static Node CreatePlayerNode(PlayerNode player, double x = 0, double y = 0)
    {
        return new Node
        {
            Id = $"player-{player.SteamId ?? player.EpicId ?? Guid.NewGuid()}",
            Type = "default", // xyflow node type
            X = x,
            Y = y,
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

    public static Node CreateGameNode(GameNode game, double x = 0, double y = 0)
    {
        return new Node
        {
            Id = $"game-{game.SteamAppId ?? game.EpicId ?? Guid.NewGuid()}",
            Type = "default",
            X = x,
            Y = y,
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

    public static Node CreatePublisherNode(PublisherNode publisher, double x = 0, double y = 0)
    {
        return new Node
        {
            Id = $"publisher-{publisher.Name?.ToLower().Replace(" ", "-") ?? Guid.NewGuid()}",
            Type = "default",
            X = x,
            Y = y,
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

    private static Dictionary<string, object> SerializeToDictionary<T>(T obj)
    {
        var json = JsonSerializer.Serialize(obj);
        return JsonSerializer.Deserialize<Dictionary<string, object>>(json) ?? new();
    }
} 