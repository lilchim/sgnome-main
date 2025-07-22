using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;
using LibraryService.Actions;
using Sgnome.Clients.Steam;
using Microsoft.Extensions.Logging;

namespace LibraryService.PinGenerators;

/// <summary>
/// Generates pins for individual library nodes
/// </summary>
public class LibraryPinGenerator
{
    private readonly ISteamClient _steamClient;
    private readonly ILogger<LibraryPinGenerator> _logger;

    public LibraryPinGenerator(
        ISteamClient steamClient,
        ILogger<LibraryPinGenerator> logger)
    {
        _steamClient = steamClient;
        _logger = logger;
    }

    /// <summary>
    /// Generates pins for a library node
    /// </summary>
    /// <param name="library">The library node to generate pins for</param>
    /// <param name="context">The pin context provided by the service</param>
    /// <returns>Collection of pins representing the library</returns>
    public async Task<IEnumerable<Pin>> GeneratePinsAsync(LibraryNode library, PinContext context)
    {
        var playerId = library.Identifiers.TryGetValue("player", out var pid) ? pid : "unknown";
        _logger.LogInformation("Generating pins for {LibrarySource} library", library.LibrarySource);

        try
        {
            // Generate pins based on library source using provided context
            switch (library.LibrarySource.ToLowerInvariant())
            {
                case "steam":
                    if (library.Identifiers.TryGetValue("steam", out var steamId))
                    {
                        var pins = await _steamClient.GetOwnedGamesAsync(steamId,
                            response => SteamLibraryPins.CreateLibraryInfoPins(response, context));
                        _logger.LogInformation("Generated {PinCount} pins for Steam library", pins.Count());
                        return pins;
                    }
                    break;
                
                // TODO: Add Epic, GOG, etc. clients as they become available
                // case "epic":
                //     if (library.Identifiers.TryGetValue("epic", out var epicId))
                //     {
                //         return await _epicClient.GetLibraryAsync(epicId,
                //             response => EpicLibraryPins.CreateLibraryInfoPins(response, context));
                //     }
                //     break;
                
                default:
                    _logger.LogWarning("No API client available for {LibrarySource} library", library.LibrarySource);
                    break;
            }

            return Enumerable.Empty<Pin>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating pins for {LibrarySource} library", library.LibrarySource);
            return Enumerable.Empty<Pin>();
        }
    }
} 