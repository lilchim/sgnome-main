namespace PlayerService.Providers;

public class SteamPlayerData
{
    public string SteamId { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
    public string? ProfileVisibility { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

public class SteamFriendsData
{
    public string SteamId { get; set; } = string.Empty;
    public int FriendCount { get; set; }
    public List<SteamFriendData> Friends { get; set; } = new();
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

public class SteamFriendData
{
    public string SteamId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public DateTime FriendSince { get; set; }
}

public class SteamActivityData
{
    public string SteamId { get; set; } = string.Empty;
    public List<SteamActivityItem> RecentActivity { get; set; } = new();
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

public class SteamActivityItem
{
    public int AppId { get; set; }
    public string GameName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string ActivityType { get; set; } = string.Empty; // "played", "achievement", etc.
} 