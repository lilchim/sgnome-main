using Microsoft.AspNetCore.Mvc;
using Sgnome.Models.Nodes;
using Sgnome.Models.Graph;
using PlayerService;
using LibraryService;
using OrganizedLibraryService;

namespace Sgnome.Web.Controllers;

[ApiController]
[Route("api/player")]
public class PlayerNodeController : ControllerBase
{
    private readonly IPlayerService _playerService;
    private readonly ILibraryService _libraryService;
    private readonly IOrganizedLibraryService _organizedLibraryService;
    private readonly ILogger<PlayerNodeController> _logger;

    public PlayerNodeController(
        IPlayerService playerService, 
        ILibraryService libraryService,
        IOrganizedLibraryService organizedLibraryService,
        ILogger<PlayerNodeController> logger)
    {
        _playerService = playerService;
        _libraryService = libraryService;
        _organizedLibraryService = organizedLibraryService;
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
    public async Task<IActionResult> SelectPlayer([FromBody] PlayerNode player)
    {
        try
        {
            // Resolve the player node using the service
            var resolvedPlayer = await _playerService.ResolveNodeAsync(player);
            
            // Create the graph node from the resolved player
            var playerNode = NodeBuilder.CreatePlayerNode(resolvedPlayer);
            
            // Get pins from all services using the resolved player
            var playerPins = await _playerService.GetPlayerInfoPinsAsync(resolvedPlayer);
            var libraryPins = await _libraryService.GetLibraryPinsAsync(resolvedPlayer);
            var organizedLibraryPins = await _organizedLibraryService.GetOrganizedLibraryPinsAsync(resolvedPlayer);
            
            // Combine all pins
            var allPins = new List<Pin>();
            allPins.AddRange(playerPins);
            allPins.AddRange(libraryPins);
            allPins.AddRange(organizedLibraryPins);
            
            // Attach pins to the player node
            playerNode.Data.Pins.AddRange(allPins);
            
            // Build the graph response
            var response = new GraphResponse
            {
                Nodes = new List<Node> { playerNode },
                Edges = new List<Edge>(), // No edges for single node operations
                Metadata = new GraphMetadata
                {
                    QueryType = "SelectPlayer",
                    QueryId = $"player-select-{resolvedPlayer.SteamId}",
                    Timestamp = DateTime.UtcNow,
                    Context = new Dictionary<string, object>
                    {
                        ["playerId"] = resolvedPlayer.SteamId ?? "unknown",
                        ["operation"] = "node-selection"
                    }
                }
            };
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error selecting player {PlayerId}", player.SteamId);
            return BadRequest(new { error = "Failed to process player selection" });
        }
    }
} 