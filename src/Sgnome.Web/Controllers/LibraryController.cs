using Microsoft.AspNetCore.Mvc;
using Sgnome.Models.Nodes;
using Sgnome.Models.Graph;
using LibraryService;

namespace Sgnome.Web.Controllers;

[ApiController]
[Route("api/library")]
public class LibraryController : ControllerBase
{
    private readonly ILibraryService _libraryService;
    private readonly ILogger<LibraryController> _logger;

    public LibraryController(
        ILibraryService libraryService,
        ILogger<LibraryController> logger)
    {
        _libraryService = libraryService;
        _logger = logger;
    }

    /// <summary>
    /// Creates or returns an Organized Library node for a specific platform
    /// </summary>
    /// <param name="request">The organized library request containing platform and player information</param>
    /// <returns>Graph response with organized library node and pins to games lists</returns>
    [HttpPost("select")]
    [ProducesResponseType(typeof(GraphResponse), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> SelectLibrary([FromBody] LibrarySelectRequest request)
    {
        try
        {
            // Resolve the organized library node using the service
            var resolvedOrganizedLibrary = await _libraryService.ResolveNodeAsync(new LibraryNode
            {
                LibrarySource = request.LibrarySource,
                PlayerId = request.PlayerId,
                DisplayName = $"{request.LibrarySource} Library"
            });

            // Create the graph node from the resolved organized library
            var organizedLibraryGraphNode = NodeBuilder.CreateOrganizedLibraryNode(resolvedOrganizedLibrary);

            // Get pins from the service (these will include pins to games lists)
            var pins = await _libraryService.GetLibraryPinsAsync(resolvedOrganizedLibrary);
            
            // Attach pins to the organized library node
            organizedLibraryGraphNode.Data.Pins.AddRange(pins);

            // Build the graph response
            var response = new GraphResponse
            {
                Nodes = new List<Node> { organizedLibraryGraphNode },
                Edges = request.OriginNodeId != null 
                    ? new List<Edge> { EdgeBuilder.CreatePlayerToOrganizedLibraryEdge(request.OriginNodeId, organizedLibraryGraphNode.Id) }
                    : new List<Edge>(),
                Metadata = new GraphMetadata
                {
                    QueryType = "SelectLibrary",
                    QueryId = $"library-{request.LibrarySource}-{request.PlayerId}",
                    Timestamp = DateTime.UtcNow,
                    Context = new Dictionary<string, object>
                    {
                        ["playerId"] = request.PlayerId,
                        ["librarySource"] = request.LibrarySource,
                        ["operation"] = "library-selection"
                    }
                }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error selecting library {LibrarySource} for {PlayerId}", 
                request.LibrarySource, request.PlayerId);
            return BadRequest(new { error = "Failed to process library selection" });
        }
    }
}

public class LibrarySelectRequest
{
    public string PlayerId { get; set; } = string.Empty;
    public string LibrarySource { get; set; } = string.Empty; // steam, epic, etc.
    public string? OriginNodeId { get; set; } // Optional origin node for edge generation
} 