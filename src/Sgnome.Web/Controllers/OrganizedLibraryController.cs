using Microsoft.AspNetCore.Mvc;
using Sgnome.Models.Nodes;
using Sgnome.Models.Graph;
using OrganizedLibraryService;

namespace Sgnome.Web.Controllers;

[ApiController]
[Route("api/organized-library")]
public class OrganizedLibraryController : ControllerBase
{
    private readonly IOrganizedLibraryService _organizedLibraryService;
    private readonly ILogger<OrganizedLibraryController> _logger;

    public OrganizedLibraryController(
        IOrganizedLibraryService organizedLibraryService,
        ILogger<OrganizedLibraryController> logger)
    {
        _organizedLibraryService = organizedLibraryService;
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
    public async Task<IActionResult> SelectOrganizedLibrary([FromBody] OrganizedLibrarySelectRequest request)
    {
        try
        {
            // Resolve the organized library node using the service
            var resolvedOrganizedLibrary = await _organizedLibraryService.ResolveNodeAsync(new OrganizedLibraryNode
            {
                LibrarySource = request.LibrarySource,
                PlayerId = request.PlayerId,
                DisplayName = $"{request.LibrarySource} Library"
            });

            // Create the graph node from the resolved organized library
            var organizedLibraryGraphNode = NodeBuilder.CreateOrganizedLibraryNode(resolvedOrganizedLibrary);

            // Get pins from the service (these will include pins to games lists)
            var pins = await _organizedLibraryService.GetOrganizedLibraryPinsAsync(resolvedOrganizedLibrary);
            
            // Attach pins to the organized library node
            organizedLibraryGraphNode.Data.Pins.AddRange(pins);

            // Build the graph response
            var response = new GraphResponse
            {
                Nodes = new List<Node> { organizedLibraryGraphNode },
                Edges = new List<Edge>(), // No edges created yet - they'll be created when pins are expanded
                Metadata = new GraphMetadata
                {
                    QueryType = "SelectOrganizedLibrary",
                    QueryId = $"organized-library-{request.LibrarySource}-{request.PlayerId}",
                    Timestamp = DateTime.UtcNow,
                    Context = new Dictionary<string, object>
                    {
                        ["playerId"] = request.PlayerId,
                        ["librarySource"] = request.LibrarySource,
                        ["operation"] = "organized-library-selection"
                    }
                }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error selecting organized library {LibrarySource} for {PlayerId}", 
                request.LibrarySource, request.PlayerId);
            return BadRequest(new { error = "Failed to process organized library selection" });
        }
    }
}

public class OrganizedLibrarySelectRequest
{
    public string PlayerId { get; set; } = string.Empty;
    public string LibrarySource { get; set; } = string.Empty; // steam, epic, etc.
} 