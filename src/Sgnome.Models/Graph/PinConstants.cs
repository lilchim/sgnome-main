namespace Sgnome.Models.Graph;

public static class PinConstants
{
    public static class Sources
    {
        public const string Steam = "steam";
        public const string Epic = "epic";
        public const string Rawg = "rawg";
        public const string Internal = "internal";
    }

    public static class PinTypes
    {
        public static class PlayerInfo
        {
            public const string DisplayName = "player-info:display-name";
            public const string RealName = "player-info:real-name";
            public const string ProfileUrl = "player-info:profile-url";
            public const string AvatarUrl = "player-info:avatar-url";
            public const string OnlineStatus = "player-info:online-status";
            public const string LastOnline = "player-info:last-online";
            public const string AccountCreationDate = "player-info:created-on";
        }

        public static class LibraryPins
        {
            public const string Library = "library:library";
            public const string LibraryList = "library:library-list";
        }
        public static class GamePins
        {
            public const string Game = "game:game";
            public const string SteamAppId = "game:steam-app-id";
            public const string GameName = "game:name";
            public const string HeaderImageUrl = "game:header-image-url";
            public const string ReleaseDate = "game:release-date";
            public const string Publisher = "game:publisher";
            public const string Developer = "game:developer";
            public const string Genre = "game:genre";
            public const string Platform = "game:platform";
            public const string Description_Short = "game:description-short";
            public const string Description_Long = "game:description-long";
            public const string Website = "game:website";   
        }
    }
}
