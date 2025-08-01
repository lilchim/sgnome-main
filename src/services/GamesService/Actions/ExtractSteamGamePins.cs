using Sgnome.Models.Graph;
using SteamApi.Models.Steam.Responses;

namespace GamesService.Actions;

public static class ExtractSteamGamePins
{
    public static IEnumerable<Pin> Extract(OwnedGamesResponse response, PinContext context)
    {
        var pins = new List<Pin>();
        response.Games.ForEach(game =>
        {
            var pin = new Pin
            {
                Id = game.AppId.ToString(),
                Label = game.Name,
                Type = PinConstants.PinTypes.GamePins.Game,
                Behavior = PinBehavior.Expandable,
                Summary = new PinSummary
                {
                    DisplayText = game.Name,
                    Icon = "game",
                    Source = "steam",
                    Preview = new Dictionary<string, object>
                    {
                        ["playtime"] = game.PlaytimeForever
                    }
                },
                Metadata = new PinMetadata
                {
                    OriginNodeId = context.InputNodeId,
                    TargetNodeId = game.AppId.ToString(),
                    TargetNodeType = context.TargetNodeType,
                    ApiEndpoint = "/api/game/select",
                    Parameters = new Dictionary<string, object>
                    {
                        ["steamId"] = game.AppId.ToString()
                    }
                }
            };
            pins.Add(pin);
        });
        return pins;
    }
}