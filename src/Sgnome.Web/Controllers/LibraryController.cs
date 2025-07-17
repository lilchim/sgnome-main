using Microsoft.AspNetCore.Mvc;
using Sgnome.Models.Nodes;
using Sgnome.Models.Graph;
using LibraryService;
using OrganizedLibraryService;

namespace Sgnome.Web.Controllers;

[ApiController]
[Route("api/library")]
public class LibraryController : ControllerBase
{
    private readonly ILibraryService _libraryService;
    private readonly IOrganizedLibraryService _organizedLibraryService;
    private readonly ILogger<LibraryController> _logger;

    public LibraryController(
        ILibraryService libraryService, 
        IOrganizedLibraryService organizedLibraryService,
        ILogger<LibraryController> logger)
    {
        _libraryService = libraryService;
        _organizedLibraryService = organizedLibraryService;
        _logger = logger;
    }

    /// <summary>
    /// Creates or returns a Library node for a player
    /// </summary>
    /// <param name="request">The library request containing player information</param>
    /// <returns>Graph response with library node and pins to organized libraries</returns>
    [HttpPost("select")]
    [ProducesResponseType(typeof(GraphResponse), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> SelectLibrary([FromBody] LibrarySelectRequest request)
    {
        try
        {
            // Resolve the library node using the service
            var resolvedLibrary = await _libraryService.ResolveNodeAsync(new LibraryNode
            {
                PlayerId = request.PlayerId,
                DisplayName = "Game Libraries",
                AvailableSources = new List<string> { "steam" }, // TODO: Add other sources
            });

            // Create the graph node from the resolved library
            var libraryGraphNode = NodeBuilder.CreateLibraryNode(resolvedLibrary);

            // Get pins from services using the resolved library
            var libraryPins = await _libraryService.GetLibraryPinsAsync(resolvedLibrary);
            var organizedLibraryPins = await _organizedLibraryService.GetOrganizedLibraryPinsAsync(resolvedLibrary);
            
            // Combine all pins
            var allPins = new List<Pin>();
            allPins.AddRange(libraryPins);
            allPins.AddRange(organizedLibraryPins);
            
            // Attach pins to the library node
            libraryGraphNode.Data.Pins.AddRange(allPins);

            // Build the graph response
            var response = new GraphResponse
            {
                Nodes = new List<Node> { libraryGraphNode },
                Edges = request.OriginNodeId != null 
                    ? new List<Edge> { EdgeBuilder.CreatePlayerToLibraryEdge(request.OriginNodeId, libraryGraphNode.Id) }
                    : new List<Edge>(),
                Metadata = new GraphMetadata
                {
                    QueryType = "SelectLibrary",
                    QueryId = $"library-{request.PlayerId}",
                    Timestamp = DateTime.UtcNow,
                    Context = new Dictionary<string, object>
                    {
                        ["playerId"] = request.PlayerId,
                        ["playerType"] = request.PlayerType,
                        ["operation"] = "library-selection"
                    }
                }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error selecting library for {PlayerId}", request.PlayerId);
            return BadRequest(new { error = "Failed to process library selection" });
        }
    }
}

public class LibrarySelectRequest
{
    public string PlayerId { get; set; } = string.Empty;
    public string PlayerType { get; set; } = "steam"; // steam, epic, etc.
    public string? OriginNodeId { get; set; } // Optional origin node for edge generation
} 