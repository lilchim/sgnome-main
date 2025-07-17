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

    public LibraryController(ILibraryService libraryService, ILogger<LibraryController> logger)
    {
        _libraryService = libraryService;
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

            // Get pins from the service (these will include pins to organized libraries)
            var pins = await _libraryService.GetLibraryPinsAsync(new PlayerNode 
            {
                SteamId = request.PlayerId,
                DisplayName = request.PlayerId
            });
            
            // Attach pins to the library node
            libraryGraphNode.Data.Pins.AddRange(pins);

            // Build the graph response
            var response = new GraphResponse
            {
                Nodes = new List<Node> { libraryGraphNode },
                Edges = new List<Edge>(), // No edges created yet - they'll be created when pins are expanded
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
} 