using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;
using LibraryService.Database;
using LibraryService.PinGenerators;
using Microsoft.Extensions.Logging;

namespace LibraryService;

/// <summary>
/// Service for consuming library-related nodes and generating pins about related data
/// </summary>
public class LibraryService : ILibraryService
{
    private readonly LibraryPinGenerator _libraryPinGenerator;
    private readonly LibraryListPinGenerator _libraryListPinGenerator;
    private readonly ILibraryDatabase _libraryDatabase;
    private readonly ILibraryListDatabase _libraryListDatabase;
    private readonly ILogger<LibraryService> _logger;

    public LibraryService(
        LibraryPinGenerator libraryPinGenerator,
        LibraryListPinGenerator libraryListPinGenerator,
        ILibraryDatabase libraryDatabase,
        ILibraryListDatabase libraryListDatabase,
        ILogger<LibraryService> logger)
    {
        _libraryPinGenerator = libraryPinGenerator;
        _libraryListPinGenerator = libraryListPinGenerator;
        _libraryDatabase = libraryDatabase;
        _libraryListDatabase = libraryListDatabase;
        _logger = logger;
    }

    /// <summary>
    /// Consumes a LibraryNode and returns pins + resolved node
    /// </summary>
    public async Task<(IEnumerable<Pin> Pins, LibraryNode ResolvedNode)> Consume(LibraryNode partial)
    {
        var playerId = partial.Identifiers.TryGetValue("player", out var pid) ? pid : "unknown";
        _logger.LogInformation("Consuming LibraryNode for {LibrarySource} player {PlayerId}",
            partial.LibrarySource, playerId);

        try
        {
            // Resolve the library node
            var resolvedNode = await _libraryDatabase.ResolveLibraryAsync(partial.Identifiers, partial.LibrarySource);
            var pins = await CreateLibraryPinsAsync(resolvedNode, partial.InternalId!, "library");
            _logger.LogInformation("Consumed library with internal ID {InternalId}", resolvedNode.InternalId);
            
            // Return empty pins (self domain)
            return (pins, resolvedNode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consuming LibraryNode for {LibrarySource} player {PlayerId}",
                partial.LibrarySource, playerId);
            throw;
        }
    }

    /// <summary>
    /// Consumes a LibraryListNode and returns pins + resolved node
    /// </summary>
    public async Task<(IEnumerable<Pin> Pins, LibraryListNode ResolvedNode)> Consume(LibraryListNode partial)
    {
        _logger.LogInformation("Consuming LibraryListNode for player {PlayerId}", partial.PlayerId);

        try
        {
            // Resolve the library list node
            var resolvedNode = await _libraryListDatabase.ResolveLibraryListAsync(partial.PlayerId);
            _logger.LogInformation("Consumed library list with internal ID {InternalId}", resolvedNode.InternalId);
            
            // Generate pins for member libraries
            var memberLibraries = new List<LibraryNode>();
            var pins = new List<Pin>();
            foreach (var (source, libraryId) in resolvedNode.LibrarySourceMapping)
            {
                var library = await _libraryDatabase.GetByInternalIdAsync(libraryId);
                if (library != null)
                {
                    memberLibraries.Add(library);
                    pins.AddRange(await CreateLibraryPinsAsync(library, resolvedNode.InternalId!, "library-list"));
                }
            }
            
            var listPins = await CreateLibraryListPinsAsync(resolvedNode, resolvedNode.InternalId!, "library-list");
            pins.AddRange(listPins);
            
            return (pins, resolvedNode);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consuming LibraryListNode for player {PlayerId}", partial.PlayerId);
            throw;
        }
    }

    /// <summary>
    /// Consumes a PlayerNode and returns pins about their libraries
    /// </summary>
    public async Task<IEnumerable<Pin>> Consume(PlayerNode player)
    {
        if (string.IsNullOrEmpty(player.InternalId))
        {
            _logger.LogWarning("Player missing InternalId, cannot consume for libraries");
            return Enumerable.Empty<Pin>();
        }

        _logger.LogInformation("Consuming PlayerNode for libraries, player {PlayerId}", player.InternalId);

        try
        {
            // Extract library-relevant identifiers from player
            var libraryIdentifiers = new Dictionary<string, string>();
            
            // Add internal player ID for library relationships
            libraryIdentifiers["player"] = player.InternalId;
            
            // Find all library sources the player has
            foreach (var source in LibraryIdentifiers.AllSources)
            {
                if (player.Identifiers.TryGetValue(source, out var sourceId) && !string.IsNullOrEmpty(sourceId))
                {
                    libraryIdentifiers[source] = sourceId;
                    _logger.LogDebug("Found {Source} identifier for player {PlayerId}: {SourceId}", 
                        source, player.InternalId, sourceId);
                }
            }

            if (libraryIdentifiers.Count <= 1) // Only has "player" identifier
            {
                _logger.LogInformation("Player {PlayerId} has no library sources, returning empty pins", player.InternalId);
                return Enumerable.Empty<Pin>();
            }

            // Resolve each library and convert to pins
            var allPins = new List<Pin>();
            var resolvedLibraries = new List<LibraryNode>();
            
            foreach (var source in LibraryIdentifiers.AllSources)
            {
                if (libraryIdentifiers.ContainsKey(source))
                {
                    // Resolve this specific library
                    var library = await _libraryDatabase.ResolveLibraryAsync(
                        new Dictionary<string, string> { ["player"] = player.InternalId, [source] = libraryIdentifiers[source] },
                        source
                    );
                    
                    resolvedLibraries.Add(library);
                    
                    // Convert library to pins using LibraryPinGenerator
                    var libraryPins = await CreateLibraryPinsAsync(library, player.InternalId, "player");
                    allPins.AddRange(libraryPins);
                    
                    _logger.LogDebug("Generated {PinCount} pins for {Source} library", libraryPins.Count(), source);
                }
            }

            // Generate collection pin by resolving LibraryListNode and using CollectionHandler
            if (resolvedLibraries.Count > 0)
            {
                var libraryList = await _libraryListDatabase.ResolveLibraryListAsync(player.InternalId);
                
                // Add library source mappings to the library list
                var librarySourceMappings = resolvedLibraries.ToDictionary(
                    lib => lib.LibrarySource, 
                    lib => lib.InternalId!
                );
                
                await _libraryListDatabase.AddLibrarySourceMappingsAsync(
                    libraryList.InternalId!, 
                    librarySourceMappings
                );
                
                // Get the updated library list with the new mappings
                var updatedLibraryList = await _libraryListDatabase.ResolveLibraryListAsync(player.InternalId);
                
                // Generate individual library pins for each resolved library
                // foreach (var library in resolvedLibraries)
                // {
                //     var libraryPins = await CreateLibraryPinsAsync(library, player.InternalId, "player");
                //     allPins.AddRange(libraryPins);
                // }

                // Generate collection summary pin
                var collectionPins = await CreateLibraryListPinsAsync(updatedLibraryList, player.InternalId, "player");
                allPins.AddRange(collectionPins);
            }

            _logger.LogInformation("Generated {PinCount} total pins for player {PlayerId} with {LibraryCount} library sources", 
                allPins.Count, player.InternalId, resolvedLibraries.Count);
            
            return allPins;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consuming PlayerNode for libraries, player {PlayerId}", player.InternalId);
            throw;
        }
    }

    #region Private Helper Methods

    /// <summary>
    /// Creates pins for a library node with proper context
    /// </summary>
    private async Task<IEnumerable<Pin>> CreateLibraryPinsAsync(LibraryNode library, string inputNodeId, string inputNodeType)
    {
        var context = new PinContext
        {
            InputNodeId = inputNodeId,
            InputNodeType = inputNodeType,
            TargetNodeType = "library",
            ApiEndpoint = "/api/library/select",
            ApiParameters = new Dictionary<string, object>
            {
                ["playerId"] = library.Identifiers["player"],
                ["librarySource"] = library.LibrarySource
            }
        };

        return await _libraryPinGenerator.GeneratePinsAsync(library, context);
    }

    /// <summary>
    /// Creates pins for a library list node with proper context
    /// </summary>
    private async Task<IEnumerable<Pin>> CreateLibraryListPinsAsync(LibraryListNode libraryList, string inputNodeId, string inputNodeType)
    {
        var context = new PinContext
        {
            InputNodeId = inputNodeId,
            InputNodeType = inputNodeType,
            TargetNodeType = "library-list",
            ApiEndpoint = "/api/library-list/select",
            ApiParameters = new Dictionary<string, object>
            {
                ["playerId"] = libraryList.PlayerId
            }
        };

        return await _libraryListPinGenerator.GeneratePinsAsync(libraryList, context);
    }

    #endregion
}
