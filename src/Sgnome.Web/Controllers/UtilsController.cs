using Microsoft.AspNetCore.Mvc;
using Sgnome.Clients.Steam;

namespace Sgnome.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UtilsController : ControllerBase
{
    private readonly ILogger<UtilsController> _logger;
    private readonly ISteamClient _steamClient;
    public UtilsController(ILogger<UtilsController> logger, ISteamClient steamClient)
    {
        _logger = logger;
        _steamClient = steamClient;
    }

    [HttpGet("steam/resolveVanityUrl")]
    public async Task<ActionResult<string>> ResolveVanityUrl([FromQuery] string vanityUrl)
    {
        var response = await _steamClient.ResolveVanityUrlAsync(vanityUrl, response => response);
        // Steam Web API returns {"response":{"success":42,"message":"No match"}}
        if (response.Success == 42)
        {
            return BadRequest($"No steam ID found for {vanityUrl}");
        }
        return Ok(response.SteamId);
    }

}