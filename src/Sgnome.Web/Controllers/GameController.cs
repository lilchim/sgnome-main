using Microsoft.AspNetCore.Mvc;
using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;
using Sgnome.Models.Requests;
using GamesService;

namespace Sgnome.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly IGamesService _gamesService;
    private readonly ILogger<GameController> _logger;

    public GameController(IGamesService gamesService, ILogger<GameController> logger)
    {
        _gamesService = gamesService;
        _logger = logger;
    }

    /// <summary>
    /// Select a game by identifiers or internal ID
    /// </summary>
    /// <param name="request">Game selection request containing identifiers</param>
    /// <returns>Graph response with the resolved game node and pins</returns>
    [HttpPost("select")]
    public async Task<ActionResult<GraphResponse>> SelectGame([FromBody] GameSelectRequest request)
    {
        try
        {
            // Create a GameNode from the request
            var gameNode = new GameNode
            {
                InternalId = request.InternalId,
                Identifiers = request.Identifiers
            };

            var (pins, resolvedGame) = await _gamesService.Consume(gameNode);
            var gameGraphNode = NodeBuilder.CreateGameNode(resolvedGame, request.X, request.Y);

            
            // Add pins to the game node's data
            gameGraphNode.Data.Pins.AddRange(pins);
            var response = new GraphResponse
            {
                Nodes = new List<Node> { gameGraphNode }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error selecting game");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("selectByInternalId")]
    public async Task<ActionResult<GraphResponse>> SelectGame([FromQuery] string internalId)
    {
        try
        {
            // For now, we'll create a simple game node with the internal ID
            // In the future, this could load from the database
            var gameNode = new GameNode
            {
                InternalId = internalId,
                Identifiers = new Dictionary<string, object>
                {
                    ["internalId"] = internalId
                }
            };

            var (pins, resolvedGame) = await _gamesService.Consume(gameNode);
            var gameGraphNode = NodeBuilder.CreateGameNode(resolvedGame);
            
            // Add pins to the game node's data
            gameGraphNode.Data.Pins.AddRange(pins);
            
            var response = new GraphResponse
            {
                Nodes = new List<Node> { gameGraphNode }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error selecting game with internal ID {InternalId}", internalId);
            return StatusCode(500, "Internal server error");
        }
    }
} 