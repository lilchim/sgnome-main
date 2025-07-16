using Sgnome.Models.Graph;
using Sgnome.Models.Nodes;

namespace PlayerService;

public interface IPlayerService
{
    /// <summary>
    /// Gets player information pins for a player node
    /// </summary>
    /// <param name="player">The player node to get info for</param>
    /// <returns>Collection of pins representing player information</returns>
    Task<IEnumerable<Pin>> GetPlayerInfoPinsAsync(PlayerNode player);
    
    /// <summary>
    /// Gets friends pins for a player node
    /// </summary>
    /// <param name="player">The player node to get friends for</param>
    /// <returns>Collection of pins representing player's friends</returns>
    Task<IEnumerable<Pin>> GetFriendsPinsAsync(PlayerNode player);
    
    /// <summary>
    /// Gets activity pins for a player node
    /// </summary>
    /// <param name="player">The player node to get activity for</param>
    /// <returns>Collection of pins representing player's activity</returns>
    Task<IEnumerable<Pin>> GetActivityPinsAsync(PlayerNode player);
} 