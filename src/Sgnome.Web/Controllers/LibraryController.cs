using Microsoft.AspNetCore.Mvc;
using Sgnome.Models.Nodes;
using Sgnome.Models.Graph;
using Sgnome.Models.Requests;
using LibraryService;
using GamesService;

namespace Sgnome.Web.Controllers;

[ApiController]
[Route("api/library")]
public class LibraryController : ControllerBase
{
    private readonly ILibraryService _libraryService;
    private readonly IGamesService _gamesService;
    private readonly ILogger<LibraryController> _logger;    

    public LibraryController(
        ILibraryService libraryService,
        IGamesService gamesService,
        ILogger<LibraryController> logger)
    {
        _libraryService = libraryService;
        _gamesService = gamesService;
        _logger = logger;
    }

    /// <summary>
    /// Creates or returns a Library node for a specific platform
    /// </summary>
    /// <param name="request">The library request containing platform and player information</param>
    /// <returns>Graph response with library node and pins</returns>
    [HttpPost("select")]
    [ProducesResponseType(typeof(GraphResponse), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> SelectLibrary([FromBody] LibrarySelectRequest request)
    {
        try
        {
            // Create partial library node from request
            var partialLibrary = new LibraryNode
            {
                LibrarySource = request.LibrarySource,
                Identifiers = new Dictionary<string, string>
                {
                    ["player"] = request.PlayerId
                },
            };

            // Consume the library node using the service
            var (pins, resolvedLibrary) = await _libraryService.Consume(partialLibrary);

            // Create the graph node from the resolved library
            var libraryGraphNode = NodeBuilder.CreateLibraryNode(resolvedLibrary);
            
            // Attach pins to the library node
            libraryGraphNode.Data.Pins.AddRange(pins);

            // Get Game Pins
            var gamePins = await _gamesService.Consume(resolvedLibrary);

            // Attach game pins to the library node
            libraryGraphNode.Data.Pins.AddRange(gamePins);

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

[ApiController]
[Route("api/librarylist")]
public class LibraryListController : ControllerBase
{
    private readonly ILibraryService _libraryService;
    private readonly ILogger<LibraryListController> _logger;

    public LibraryListController(
        ILibraryService libraryService,
        ILogger<LibraryListController> logger)
    {
        _libraryService = libraryService;
        _logger = logger;
    }

    /// <summary>
    /// Creates or returns a LibraryList node for a player
    /// </summary>
    /// <param name="request">The library list request containing player information</param>
    /// <returns>Graph response with library list node and pins to individual libraries</returns>
    [HttpPost("select")]
    [ProducesResponseType(typeof(GraphResponse), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> SelectLibraryList([FromBody] LibraryListSelectRequest request)
    {
        try
        {
            // Create partial library list node from request
            var partialLibraryList = new LibraryListNode
            {
                PlayerId = request.PlayerId,
                LibrarySourceMapping = new Dictionary<string, string>() // Will be populated by service
            };

            // Consume the library list node using the service
            var (pins, resolvedLibraryList) = await _libraryService.Consume(partialLibraryList);

            // Create the graph node from the resolved library list
            var libraryListGraphNode = NodeBuilder.CreateLibrariesNode(resolvedLibraryList);
            
            // Attach pins to the library list node
            libraryListGraphNode.Data.Pins.AddRange(pins);

            // Build the graph response
            var response = new GraphResponse
            {
                Nodes = new List<Node> { libraryListGraphNode },
                Edges = request.OriginNodeId != null 
                    ? new List<Edge> { EdgeBuilder.CreatePlayerToLibrariesEdge(request.OriginNodeId, libraryListGraphNode.Id) }
                    : new List<Edge>(),
                Metadata = new GraphMetadata
                {
                    QueryType = "SelectLibraryList",
                    QueryId = $"librarylist-{request.PlayerId}",
                    Timestamp = DateTime.UtcNow,
                    Context = new Dictionary<string, object>
                    {
                        ["playerId"] = request.PlayerId,
                        ["operation"] = "librarylist-selection"
                    }
                }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error selecting library list for {PlayerId}", request.PlayerId);
            return BadRequest(new { error = "Failed to process library list selection" });
        }
    }
}

 