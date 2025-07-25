using Microsoft.AspNetCore.Mvc;
using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;
using GamesService;

namespace Sgnome.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesListController : ControllerBase
{
    private readonly IGamesService _gamesService;
    private readonly ILogger<GamesListController> _logger;

    public GamesListController(IGamesService gamesService, ILogger<GamesListController> logger)
    {
        _gamesService = gamesService;
        _logger = logger;
    }

    /// <summary>
    /// Select a games list by ID
    /// </summary>
    /// <param name="id">Games list ID</param>
    /// <returns>Graph response with the games list node and pins</returns>
    [HttpGet("select")]
    public async Task<ActionResult<GraphResponse>> SelectGamesList([FromQuery] string id)
    {
        try
        {
            // For now, we'll create a simple games list node
            // In the future, this could load from the database
            var gamesListNode = new GamesListNode
            {
                Id = id,
                DisplayName = $"Games List {id}",
                Source = "system",
                ListType = "custom",
                GameCount = 0
            };

            var pins = await _gamesService.Consume(gamesListNode);
            
            // Create a Node wrapper for the games list
            var node = new Node
            {
                Id = gamesListNode.Id,
                Type = "gamesListNode",
                Data = new NodeData
                {
                    Label = gamesListNode.DisplayName,
                    NodeType = "gamesList",
                    Pins = pins.ToList(),
                    State = NodeState.Loaded
                }
            };
            
            var response = new GraphResponse
            {
                Nodes = new List<Node> { node }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error selecting games list with ID {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }
} 