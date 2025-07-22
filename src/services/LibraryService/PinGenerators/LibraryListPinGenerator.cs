using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;
using Sgnome.Clients.Steam;
using LibraryService.Actions;
using Microsoft.Extensions.Logging;

namespace LibraryService.PinGenerators;

/// <summary>
/// Generates pins for library list nodes (collections)
/// </summary>
public class LibraryListPinGenerator
{
    private readonly ISteamClient _steamClient;
    private readonly ILogger<LibraryListPinGenerator> _logger;

    public LibraryListPinGenerator(
        ISteamClient steamClient,
        ILogger<LibraryListPinGenerator> logger)
    {
        _steamClient = steamClient;
        _logger = logger;
    }

    /// <summary>
    /// Generates pins for a library list node (collection summary only)
    /// </summary>
    /// <param name="libraryList">The library list node</param>
    /// <param name="context">The pin context provided by the service</param>
    /// <returns>Collection of pins representing the library list</returns>
    public async Task<IEnumerable<Pin>> GeneratePinsAsync(LibraryListNode libraryList, PinContext context)
    {
        _logger.LogInformation("Generating pins for library collection {PlayerId}", libraryList.PlayerId);

        try
        {
            var pins = new List<Pin>();

            // Generate collection summary pin only
            var collectionPin = CreateCollectionSummaryPinAsync(libraryList, context);
            pins.Add(collectionPin);

            _logger.LogInformation("Generated {PinCount} pins for library collection {PlayerId}",
                pins.Count, libraryList.PlayerId);
            
            return pins;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating pins for library collection {PlayerId}", libraryList.PlayerId);
            throw;
        }
    }



    private Pin CreateCollectionSummaryPinAsync(LibraryListNode libraryList, PinContext context)
    {
        var libraryCount = libraryList.LibrarySourceMapping.Count;
        var displayText = $"{libraryCount} libraries";

        return new Pin
        {
            Id = $"librarylist-{libraryList.InternalId}",
            Label = libraryList.DisplayName,
            Type = "library-list",
            Behavior = PinBehavior.Expandable,
            Summary = new PinSummary
            {
                DisplayText = displayText,
                Count = libraryCount,
                Icon = "collection"
            },
            Metadata = new PinMetadata
            {
                TargetNodeType = context.TargetNodeType,
                OriginNodeId = context.InputNodeId,
                ApiEndpoint = context.ApiEndpoint,
                Parameters = context.ApiParameters
            }
        };
    }
} 