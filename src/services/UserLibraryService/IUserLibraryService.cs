using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;

namespace UserLibraryService;

public interface IUserLibraryService
{
    /// <summary>
    /// Gets the user's complete library with games and pins
    /// </summary>
    /// <param name="player">The player node to get library for</param>
    /// <returns>Graph response with player node, game nodes, and pins</returns>
    Task<GraphResponse> GetUserLibraryAsync(PlayerNode player);
    
    /// <summary>
    /// Gets the user's recently played games
    /// </summary>
    /// <param name="player">The player node to get recent games for</param>
    /// <returns>Graph response with player node, recent game nodes, and pins</returns>
    Task<GraphResponse> GetRecentlyPlayedGamesAsync(PlayerNode player);
} 