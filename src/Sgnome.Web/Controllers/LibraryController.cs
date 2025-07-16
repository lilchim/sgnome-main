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
    /// Handles library selection and returns a graph response with library node and organized library.
    /// </summary>
    /// <param name="request">The library request containing player and library source</param>
    /// <returns>Graph response with library and organized library nodes</returns>
    [HttpPost("steam")]
    [ProducesResponseType(typeof(GraphResponse), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> SelectSteamLibrary([FromBody] LibraryRequest request)
    {
        try
        {
            // Create the library node
            var libraryNode = new LibraryNode
            {
                PlayerId = request.SteamId,
                DisplayName = "Game Libraries",
                AvailableSources = new List<string> { "steam" },
                TotalGameCount = 0, // Will be populated by service
                LastUpdated = DateTime.UtcNow
            };

            var libraryGraphNode = NodeBuilder.CreateLibraryNode(libraryNode);

            // Create the organized library node
            var organizedLibraryNode = new OrganizedLibraryNode
            {
                LibrarySource = "steam",
                PlayerId = request.SteamId,
                DisplayName = "Steam Library",
                TotalGameCount = 0, // Will be populated by service
                AvailableCategories = new List<string> { "recently-played", "all-games" },
                LastUpdated = DateTime.UtcNow
            };

            var organizedLibraryGraphNode = NodeBuilder.CreateOrganizedLibraryNode(organizedLibraryNode);

            // Get pins from the service
            var pins = await _libraryService.GetOrganizedLibraryPinsAsync("steam", request.SteamId);
            
            // Attach pins to the organized library node
            organizedLibraryGraphNode.Data.Pins.AddRange(pins);

            // Create edge from library to organized library
            var edge = new Edge
            {
                Id = $"edge-{libraryGraphNode.Id}-{organizedLibraryGraphNode.Id}",
                Source = libraryGraphNode.Id,
                Target = organizedLibraryGraphNode.Id,
                Type = "default",
                Data = new EdgeData
                {
                    Label = "Contains",
                    EdgeType = "contains",
                    Properties = new Dictionary<string, object>
                    {
                        ["source"] = "steam"
                    }
                }
            };

            // Build the graph response
            var response = new GraphResponse
            {
                Nodes = new List<Node> { libraryGraphNode, organizedLibraryGraphNode },
                Edges = new List<Edge> { edge },
                Metadata = new GraphMetadata
                {
                    QueryType = "SelectSteamLibrary",
                    QueryId = $"steam-library-{request.SteamId}",
                    Timestamp = DateTime.UtcNow,
                    Context = new Dictionary<string, object>
                    {
                        ["steamId"] = request.SteamId,
                        ["librarySource"] = "steam",
                        ["operation"] = "library-expansion"
                    }
                }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error selecting Steam library for {SteamId}", request.SteamId);
            return BadRequest(new { error = "Failed to process library selection" });
        }
    }
}

public class LibraryRequest
{
    public string SteamId { get; set; } = string.Empty;
} 