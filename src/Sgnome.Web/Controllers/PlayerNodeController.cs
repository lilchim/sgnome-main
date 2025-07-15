using Microsoft.AspNetCore.Mvc;
using Sgnome.Models.Nodes;
using Sgnome.Models.Graph;

namespace Sgnome.Web.Controllers;

[ApiController]
[Route("api/player")]
public class PlayerNodeController : ControllerBase
{
    /// <summary>
    /// Handles selection of a PlayerNode and returns a graph response.
    /// </summary>
    /// <param name="player">The player node to select.</param>
    /// <returns>Graph response for the selected player node.</returns>
    [HttpPost("select")]
    [ProducesResponseType(typeof(GraphResponse), 200)]
    [ProducesResponseType(400)]
    public IActionResult SelectPlayer([FromBody] PlayerNode player)
    {
        // TODO: Implement service call to build graph response
        return Ok();
    }
} 