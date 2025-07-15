using Microsoft.AspNetCore.Mvc;
using UserLibraryService;

namespace Sgnome.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserLibraryController : ControllerBase
{
    private readonly IUserLibraryService _userLibraryService;

    public UserLibraryController(IUserLibraryService userLibraryService)
    {
        _userLibraryService = userLibraryService;
    }

    [HttpGet("{steamId}")]
    public async Task<ActionResult<UserLibraryResponse>> GetUserLibrary(string steamId, [FromQuery] bool includeFreeGames = true)
    {
        if (string.IsNullOrWhiteSpace(steamId))
        {
            return BadRequest("Steam ID is required");
        }

        var result = await _userLibraryService.GetUserLibraryAsync(steamId, includeFreeGames);
        return Ok(result);
    }

    [HttpGet("{steamId}/recently-played")]
    public async Task<ActionResult<UserLibraryResponse>> GetRecentlyPlayedGames(string steamId, [FromQuery] int count = 10)
    {
        if (string.IsNullOrWhiteSpace(steamId))
        {
            return BadRequest("Steam ID is required");
        }

        if (count <= 0 || count > 100)
        {
            return BadRequest("Count must be between 1 and 100");
        }

        var result = await _userLibraryService.GetRecentlyPlayedGamesAsync(steamId, count);
        return Ok(result);
    }
} 