using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;
using Microsoft.Extensions.Logging;

namespace GamesService.PinGenerators;

public class GameInfoPinGenerator
{
    private readonly ILogger<GameInfoPinGenerator> _logger;

    public GameInfoPinGenerator(ILogger<GameInfoPinGenerator> logger)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<Pin>> GeneratePinsAsync(GameNode game)
    {
        var pins = new List<Pin>();

        // Basic game info pin
        if (!string.IsNullOrEmpty(game.Name))
        {
            pins.Add(new Pin
            {
                Id = $"game-info-{game.InternalId}",
                Label = "Game Information",
                Type = "info",
                Behavior = PinBehavior.Informational,
                Summary = new PinSummary
                {
                    DisplayText = game.Name,
                    Icon = "🎮",
                    Preview = new Dictionary<string, object>
                    {
                        ["name"] = game.Name,
                        ["steamAppId"] = game.SteamAppId,
                        ["epicId"] = game.EpicId,
                        ["iconUrl"] = game.IconUrl,
                        ["logoUrl"] = game.LogoUrl
                    }
                }
            });
        }

        // Storefront availability pins
        if (game.SteamAppId.HasValue)
        {
            pins.Add(new Pin
            {
                Id = $"steam-availability-{game.InternalId}",
                Label = "Available on Steam",
                Type = "availability",
                Behavior = PinBehavior.Informational,
                Summary = new PinSummary
                {
                    DisplayText = $"Steam App ID: {game.SteamAppId}",
                    Icon = "🔗",
                    Preview = new Dictionary<string, object>
                    {
                        ["storefront"] = "steam",
                        ["appId"] = game.SteamAppId.Value,
                        ["url"] = $"https://store.steampowered.com/app/{game.SteamAppId}"
                    }
                }
            });
        }

        if (!string.IsNullOrEmpty(game.EpicId))
        {
            pins.Add(new Pin
            {
                Id = $"epic-availability-{game.InternalId}",
                Label = "Available on Epic",
                Type = "availability",
                Behavior = PinBehavior.Informational,
                Summary = new PinSummary
                {
                    DisplayText = $"Epic ID: {game.EpicId}",
                    Icon = "🔗",
                    Preview = new Dictionary<string, object>
                    {
                        ["storefront"] = "epic",
                        ["epicId"] = game.EpicId,
                        ["url"] = $"https://store.epicgames.com/p/{game.EpicId}"
                    }
                }
            });
        }

        _logger.LogInformation("Generated {PinCount} info pins for game {GameId}", pins.Count, game.InternalId);
        return pins;
    }
} 