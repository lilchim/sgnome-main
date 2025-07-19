using Microsoft.AspNetCore.Mvc;
using Sgnome.Models.Nodes;
using Sgnome.Models.Graph;
using LibrariesService;
using LibraryService;

namespace Sgnome.Web.Controllers;

[ApiController]
[Route("api/libraries")]
public class LibrariesController : ControllerBase
{
    private readonly ILibrariesService _librariesService;
    private readonly ILibraryService _libraryService;
    private readonly ILogger<LibrariesController> _logger;

    public LibrariesController(
        ILibrariesService librariesService, 
        ILibraryService libraryService,
        ILogger<LibrariesController> logger)
    {
        _librariesService = librariesService;
        _libraryService = libraryService;
        _logger = logger;
    }

    /// <summary>
    /// Creates or returns a Libraries node for a player
    /// </summary>
    /// <param name="request">The libraries request containing player information</param>
    /// <returns>Graph response with libraries node and pins to individual libraries</returns>
    [HttpPost("select")]
    [ProducesResponseType(typeof(GraphResponse), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> SelectLibraries([FromBody] LibrariesSelectRequest request)
    {
        try
        {
            // Resolve the libraries node using the service
            var resolvedLibraries = await _librariesService.ResolveNodeAsync(new LibrariesNode
            {
                PlayerId = request.PlayerId,
                DisplayName = "Game Libraries",
                AvailableSources = new List<string> { "steam" }, // TODO: Add other sources
            });

            // Create the graph node from the resolved libraries
            var librariesGraphNode = NodeBuilder.CreateLibrariesNode(resolvedLibraries);

            // Get pins from services using the resolved libraries
            var librariesPins = await _librariesService.GetLibrariesPinsAsync(resolvedLibraries);
            // TODO: Update OrganizedLibraryService to work with new node types
            // var organizedLibraryPins = await _organizedLibraryService.GetOrganizedLibraryPinsAsync(resolvedLibraries);
            
            // Combine all pins
            var allPins = new List<Pin>();
            allPins.AddRange(librariesPins);
            // allPins.AddRange(organizedLibraryPins);
            
            // Attach pins to the libraries node
            librariesGraphNode.Data.Pins.AddRange(allPins);

            // Build the graph response
            var response = new GraphResponse
            {
                Nodes = new List<Node> { librariesGraphNode },
                Edges = request.OriginNodeId != null 
                    ? new List<Edge> { EdgeBuilder.CreatePlayerToLibrariesEdge(request.OriginNodeId, librariesGraphNode.Id) }
                    : new List<Edge>(),
                Metadata = new GraphMetadata
                {
                    QueryType = "SelectLibraries",
                    QueryId = $"libraries-{request.PlayerId}",
                    Timestamp = DateTime.UtcNow,
                    Context = new Dictionary<string, object>
                    {
                        ["playerId"] = request.PlayerId,
                        ["playerType"] = request.PlayerType,
                        ["operation"] = "libraries-selection"
                    }
                }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error selecting libraries for {PlayerId}", request.PlayerId);
            return BadRequest(new { error = "Failed to process libraries selection" });
        }
    }
}

public class LibrariesSelectRequest
{
    public string PlayerId { get; set; } = string.Empty;
    public string PlayerType { get; set; } = "steam"; // steam, epic, etc.
    public string? OriginNodeId { get; set; } // Optional origin node for edge generation
} 