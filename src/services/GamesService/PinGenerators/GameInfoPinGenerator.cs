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
                        ["iconUrl"] = game.IconUrl,
                        ["logoUrl"] = game.LogoUrl
                    }
                }
            });
        }

        // Storefront availability pins - will be generated from identifiers
        foreach (var (key, value) in game.Identifiers)
        {
            if (key.StartsWith("storefront:"))
            {
                var storefront = key.Substring("storefront:".Length);
                pins.Add(new Pin
                {
                    Id = $"{storefront}-availability-{game.InternalId}",
                    Label = $"Available on {storefront}",
                    Type = "availability",
                    Behavior = PinBehavior.Informational,
                    Summary = new PinSummary
                    {
                        DisplayText = $"{storefront} ID: {value}",
                        Icon = "🔗",
                        Preview = new Dictionary<string, object>
                        {
                            ["storefront"] = storefront,
                            ["id"] = value
                        }
                    }
                });
            }
        }

        _logger.LogInformation("Generated {PinCount} info pins for game {GameId}", pins.Count, game.InternalId);
        return pins;
    }
} 