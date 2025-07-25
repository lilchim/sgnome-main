using Microsoft.AspNetCore.Mvc;
using Sgnome.Models.Nodes;
using Sgnome.Models.Graph;
using Sgnome.Models.Requests;
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
            // Create a PlayerNode from the request
            var playerNode = new PlayerNode
            {
                InternalId = request.InternalId,
                Identifiers = request.Identifiers
            };
            
            // Consume the player node using the service (resolves and generates enrichment pins)
            var (playerPins, resolvedPlayer) = await _playerService.Consume(playerNode);
            
            // Create the graph node from the resolved player
            var playerGraphNode = NodeBuilder.CreatePlayerNode(resolvedPlayer);
            
            // Get cross-domain pins from library service
            var libraryPins = await _libraryService.Consume(resolvedPlayer);
            
            // Combine all pins
            var allPins = new List<Pin>();
            allPins.AddRange(playerPins);
            allPins.AddRange(libraryPins);
            
            // Attach pins to the player node
            playerGraphNode.Data.Pins.AddRange(allPins);
            
            // Build the graph response
            var response = new GraphResponse
            {
                Nodes = new List<Node> { playerGraphNode },
                Edges = request.OriginNodeId != null
                    ? new List<Edge> { EdgeBuilder.CreateEdge(request.OriginNodeId, playerGraphNode.Id, "expands_to", "Expands To") }
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
            _logger.LogError(ex, "Error selecting player {PlayerId}", request.InternalId ?? request.Identifiers.ToString());
            return BadRequest(new { error = "Failed to process player selection" });
        }
    }
}

 