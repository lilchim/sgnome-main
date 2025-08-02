using Sgnome.Models.Graph;
using SteamApi.Models.Steam.Player;
using SteamApi.Models.Steam.Responses;
using SteamApi.Models.Steam.Store;

namespace GamesService.Actions;

public static class ExtractSteamGameInfoPins
{
    public static IEnumerable<Pin> Extract(Dictionary<string, StoreAppDetailsResponse> response, PinContext context)
    {
        var pins = new List<Pin>();
        response.Values.ToList().ForEach(result =>
        {
            if (result.Success && result.Data != null)
            {
                pins.Add(MakeSteamAppIdPin(result.Data, context));
                pins.Add(MakeGameNamePin(result.Data, context));
            }
        });
        return pins;
    }


    private static Pin MakeSteamAppIdPin(StoreAppDetails data, PinContext context)
    {
        return new Pin
        {
            Id = context.InputNodeId,
            Label = "Steam App ID",
            Type = PinConstants.PinTypes.GamePins.SteamAppId,
            Behavior = PinBehavior.Informational,
            Summary = new PinSummary
            {
                DisplayText = data.SteamAppId.ToString(),
                Icon = "game",
                Source = "steam",
                Preview = new Dictionary<string, object>
                {
                    ["steamAppId"] = data.SteamAppId.ToString()
                }
            }
        };
    }

    private static Pin MakeGameNamePin(StoreAppDetails data, PinContext context)
    {
        return new Pin
        {
            Id = context.InputNodeId,
            Label = "Game Name",
            Type = PinConstants.PinTypes.GamePins.GameName,
            Behavior = PinBehavior.Informational,
            Summary = new PinSummary
            {
                DisplayText = data.Name,
                Icon = "game",
                Source = "steam",
                Preview = new Dictionary<string, object>
                {
                    ["gameName"] = data.Name
                }
            }
        };
    }
}