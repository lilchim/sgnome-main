using Microsoft.AspNetCore.Mvc;
using Sgnome.Models.Nodes;
using Sgnome.Models.Graph;
using UserLibraryService;

namespace Sgnome.Web.Controllers;

[ApiController]
[Route("api/player")]
public class PlayerNodeController : ControllerBase
{
    private readonly IUserLibraryService _userLibraryService;
    private readonly ILogger<PlayerNodeController> _logger;

    public PlayerNodeController(IUserLibraryService userLibraryService, ILogger<PlayerNodeController> logger)
    {
        _userLibraryService = userLibraryService;
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
            var response = await _userLibraryService.GetUserLibraryAsync(player);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error selecting player {PlayerId}", player.SteamId);
            return BadRequest(new { error = "Failed to process player selection" });
        }
    }
} 