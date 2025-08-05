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
                pins.Add(MakeHeaderImageUrlPin(result.Data, context));
                pins.AddRange(MakePublisherPins(result.Data, context));
                pins.Add(MakeReleaseDatePin(result.Data, context));
                pins.AddRange(MakePublisherPins(result.Data, context));
                pins.AddRange(MakeDeveloperPins(result.Data, context));
                pins.AddRange(MakeGenrePins(result.Data, context));
                pins.AddRange(MakePlatformsPins(result.Data, context));
                pins.AddRange(MakeDescriptionPins(result.Data, context));

                var websitePin = MakeWebsitePin(result.Data, context);
                if (websitePin != null) { pins.Add(websitePin); }
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

    private static Pin MakeHeaderImageUrlPin(StoreAppDetails data, PinContext context)
    {
        return new Pin
        {
            Id = context.InputNodeId,
            Label = "Header Image URL",
            Type = PinConstants.PinTypes.GamePins.HeaderImageUrl,
            Behavior = PinBehavior.Informational,
            Summary = new PinSummary
            {
                DisplayText = "Header Image",
                Icon = "game",
                Source = "steam",
                Preview = new Dictionary<string, object>
                {
                    ["headerImageUrl"] = data.HeaderImage != null ? data.HeaderImage : "https://eu-images.contentstack.com/v3/assets/bltbc1876152fcd9f07/blt95e8cadd92d93725/673b0c6cd404ec7e8d30694a/steam.png"
                }
            }
        };
    }

    private static Pin MakeReleaseDatePin(StoreAppDetails data, PinContext context)
    {

        return new Pin
        {
            Id = context.InputNodeId,
            Label = "Release Date",
            Type = PinConstants.PinTypes.GamePins.ReleaseDate,
            Behavior = PinBehavior.Informational,
            Summary = new PinSummary
            {
                DisplayText = "Release Date",
                Icon = "game",
                Source = "steam",
                Preview = new Dictionary<string, object>
                {
                    ["releaseDate"] = data.ReleaseDate != null ? data.ReleaseDate!.Date.ToString() : "Unknown"
                }
            }
        };
    }

    private static IEnumerable<Pin> MakePublisherPins(StoreAppDetails data, PinContext context)
    {
        var pins = new List<Pin>();
        if (data.Publishers != null)
        {
            foreach (var publisher in data.Publishers)
            {
                pins.Add(new Pin
                {
                    Id = context.InputNodeId,
                    Label = "Publisher",
                    Type = PinConstants.PinTypes.GamePins.Publisher,
                    Behavior = PinBehavior.Informational,
                    Summary = new PinSummary
                    {
                        DisplayText = publisher,
                        Icon = "game",
                        Source = "steam",
                        Preview = new Dictionary<string, object>
                        {
                            ["publisher"] = publisher
                        }
                    }
                });
            }
        }
        return pins;
    }

    private static IEnumerable<Pin> MakeDeveloperPins(StoreAppDetails data, PinContext context)
    {
        var pins = new List<Pin>();
        if (data.Developers != null)
        {
            foreach (var developer in data.Developers)
            {
                pins.Add(new Pin
                {
                    Id = context.InputNodeId,
                    Label = "Developer",
                    Type = PinConstants.PinTypes.GamePins.Developer,
                    Behavior = PinBehavior.Informational,
                    Summary = new PinSummary
                    {
                        DisplayText = developer,
                        Icon = "game",
                        Source = "steam",
                        Preview = new Dictionary<string, object>
                        {
                            ["developer"] = developer
                        }
                    }
                });
            }
        }
        return pins;
    }

    private static IEnumerable<Pin> MakeGenrePins(StoreAppDetails data, PinContext context)
    {
        var pins = new List<Pin>();
        if (data.Genres != null)
        {
            foreach (var genre in data.Genres)
            {
                pins.Add(new Pin
                {
                    Id = context.InputNodeId,
                    Label = "Genre",
                    Type = PinConstants.PinTypes.GamePins.Genre,
                    Behavior = PinBehavior.Informational,
                    Summary = new PinSummary
                    {
                        DisplayText = genre.Description,
                        Icon = "game",
                        Source = "steam",
                        Preview = new Dictionary<string, object>
                        {
                            ["genreId"] = genre.Id,
                            ["genreDescription"] = genre.Description
                        }
                    }
                });
            }
        }
        return pins;
    }

    private static IEnumerable<Pin> MakePlatformsPins(StoreAppDetails data, PinContext context)
    {
        var pins = new List<Pin>();
        if (data.Platforms != null)
        {
            if (data.Platforms.Windows) { pins.Add(MakePlatformPin("Windows", context)); }
            if (data.Platforms.Mac) { pins.Add(MakePlatformPin("Mac", context)); }
            if (data.Platforms.Linux) { pins.Add(MakePlatformPin("Linux", context)); }
        }
        return pins;
    }

    private static Pin MakePlatformPin(string platform, PinContext context)
    {
        return new Pin
        {
            Id = context.InputNodeId,
            Label = "Platform",
            Type = PinConstants.PinTypes.GamePins.Platform,
            Behavior = PinBehavior.Informational,
            Summary = new PinSummary
            {
                DisplayText = platform,
                Icon = "game",
                Source = "steam",
                Preview = new Dictionary<string, object>
                {
                    ["platform"] = platform
                }
            }
        };
    }

    private static IEnumerable<Pin> MakeDescriptionPins(StoreAppDetails data, PinContext context)
    {
        var pins = new List<Pin>();
        if (data.AboutTheGame != null)
        {
            pins.Add(new Pin
            {
                Id = context.InputNodeId,
                Label = "Description",
                Type = PinConstants.PinTypes.GamePins.Description_Short,
                Behavior = PinBehavior.Informational,
                Summary = new PinSummary
                {
                    DisplayText = data.AboutTheGame,
                    Icon = "game",
                    Source = "steam",
                    Preview = new Dictionary<string, object>
                    {
                        ["description"] = data.AboutTheGame
                    }
                }
            });
        }

        if (data.DetailedDescription != null)
        {
            pins.Add(new Pin
            {
                Id = context.InputNodeId,
                Label = "Detailed Description",
                Type = PinConstants.PinTypes.GamePins.Description_Long,
                Behavior = PinBehavior.Informational,
                Summary = new PinSummary
                {
                    DisplayText = data.DetailedDescription,
                    Icon = "game",
                    Source = "steam",
                    Preview = new Dictionary<string, object>
                    {
                        ["detailedDescription"] = data.DetailedDescription
                    }
                }
            });
        }
        return pins;
    }

    private static Pin? MakeWebsitePin(StoreAppDetails data, PinContext context)
    {
        if (data.Website != null)
        {
            return new Pin
            {
                Id = context.InputNodeId,
                Label = "Website",
                Type = PinConstants.PinTypes.GamePins.Website,
                Behavior = PinBehavior.Informational,
                Summary = new PinSummary
                {
                    DisplayText = data.Website,
                    Icon = "game",
                    Source = "steam",
                    Preview = new Dictionary<string, object>
                    {
                        ["website"] = data.Website
                    }
                }
            };
        }
        return null;
    }
}