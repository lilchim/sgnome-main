using Microsoft.AspNetCore.Mvc;
using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;
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

    [HttpPost("select")]
    public async Task<ActionResult<GraphResponse>> SelectGame([FromBody] GameNode gameNode)
    {
        try
        {
            var (pins, resolvedGame) = await _gamesService.Consume(gameNode);
            
            // Add pins to the game node's data
            resolvedGame.Data.Pins.AddRange(pins);
            
            var response = new GraphResponse
            {
                Nodes = new List<Node> { resolvedGame }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consuming game node");
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
                Name = $"Game {internalId}",
                Identifiers = new Dictionary<string, object>
                {
                    ["internalId"] = internalId
                }
            };

            var (pins, resolvedGame) = await _gamesService.Consume(gameNode);
            
            // Add pins to the game node's data
            resolvedGame.Data.Pins.AddRange(pins);
            
            var response = new GraphResponse
            {
                Nodes = new List<Node> { resolvedGame }
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