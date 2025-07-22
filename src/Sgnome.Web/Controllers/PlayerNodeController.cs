using Microsoft.AspNetCore.Mvc;
using Sgnome.Models.Nodes;
using Sgnome.Models.Graph;
using PlayerService;
using LibraryService;

namespace Sgnome.Web.Controllers;

[ApiController]
[Route("api/player")]
public class PlayerNodeController : ControllerBase
{
    private readonly IPlayerService _playerService;
    private readonly ILibraryService _libraryService;
    private readonly ILogger<PlayerNodeController> _logger;

    public PlayerNodeController(
        IPlayerService playerService, 
        ILibraryService libraryService,
        ILogger<PlayerNodeController> logger)
    {
        _playerService = playerService;
        _libraryService = libraryService;
        _logger = logger;
    }

    /// <summary>
    /// Handles selection of a PlayerNode and returns a graph response.
    /// </summary>
    /// <param name="player">The player node to select.</param>
    /// <returns>Graph response for the selected player node.</returns>
    [HttpPost("select")]
    [ProducesResponseType(typeof(GraphResponse), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> SelectPlayer([FromBody] PlayerSelectRequest request)
    {
        try
        {
            // Consume the player node using the service (resolves and generates enrichment pins)
            var (playerPins, resolvedPlayer) = await _playerService.Consume(request.Player);
            
            // Create the graph node from the resolved player
            var playerNode = NodeBuilder.CreatePlayerNode(resolvedPlayer);
            
            // Get cross-domain pins from library service
            var libraryPins = await _libraryService.Consume(resolvedPlayer);
            
            // Combine all pins
            var allPins = new List<Pin>();
            allPins.AddRange(playerPins);
            allPins.AddRange(libraryPins);
            
            // Attach pins to the player node
            playerNode.Data.Pins.AddRange(allPins);
            
            // Build the graph response
            var response = new GraphResponse
            {
                Nodes = new List<Node> { playerNode },
                Edges = request.OriginNodeId != null
                    ? new List<Edge> { EdgeBuilder.CreateEdge(request.OriginNodeId, playerNode.Id, "expands_to", "Expands To") }
                    : new List<Edge>(),
                Metadata = new GraphMetadata
                {
                    QueryType = "SelectPlayer",
                    QueryId = $"player-select-{resolvedPlayer.InternalId}",
                    Timestamp = DateTime.UtcNow,
                    Context = new Dictionary<string, object>
                    {
                        ["playerId"] = resolvedPlayer.InternalId!,
                        ["operation"] = "node-selection"
                    }
                }
            };
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error selecting player {PlayerId}", request.Player?.Identifiers);
            return BadRequest(new { error = "Failed to process player selection" });
        }
    }
}

public class PlayerSelectRequest
{
    public PlayerNode Player { get; set; } = new PlayerNode();
    public string? OriginNodeId { get; set; } // Optional origin node for edge generation
} 