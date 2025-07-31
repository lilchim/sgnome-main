using Sgnome.Models.Graph;
using SteamApi.Models.Steam.Responses;

namespace PlayerService.Actions;

public static class ExtractSteamPlayerInfoPins
{
    public static IEnumerable<Pin> ExtractPins(PlayerSummariesResponse response, PinContext context)
    {
        var pins = new List<Pin>();

        var playerSummary = response.Players.FirstOrDefault();

        // Add Steam-specific info pins
        if (playerSummary?.PersonaName != null)
        {
            var displayNamePin = new Pin
            {
                Id = "steam-display-name",
                Label = "Display Name",
                Type = PinConstants.PinTypes.PlayerInfo.DisplayName,
                Behavior = PinBehavior.Informational,
                Summary = new PinSummary
                {
                    DisplayText = playerSummary.PersonaName,
                    Icon = "user",
                    Source = "steam"
                },
                Metadata = new PinMetadata
                {
                    OriginNodeId = context.InputNodeId,
                    TargetNodeId = null,
                    TargetNodeType = context.TargetNodeType
                }
            };
            pins.Add(displayNamePin);
        }

        // Real Name
        if (playerSummary?.RealName != null)
        {
            var realNamePin = new Pin
            {
                Id = "steam-real-name",
                Label = "Real Name",
                Type = PinConstants.PinTypes.PlayerInfo.RealName,
                Behavior = PinBehavior.Informational,
                Summary = new PinSummary
                {
                    DisplayText = playerSummary.RealName,
                    Icon = "user",
                    Source = "steam"
                },
                Metadata = new PinMetadata
                {
                    OriginNodeId = context.InputNodeId,
                    TargetNodeId = null,
                    TargetNodeType = context.TargetNodeType
                }
            };
            pins.Add(realNamePin);
        }
        
        // Profile URL
        if (playerSummary?.ProfileUrl != null)
        {
            var profileUrlPin = new Pin
            {
                Id = "steam-profile-url",
                Label = "Profile URL",
                Type = PinConstants.PinTypes.PlayerInfo.ProfileUrl,
                Behavior = PinBehavior.Informational,
                Summary = new PinSummary
                {
                    DisplayText = playerSummary.ProfileUrl,
                    Icon = "link",
                    Source = "steam"
                },
                Metadata = new PinMetadata
                {
                    OriginNodeId = context.InputNodeId,
                    TargetNodeId = null,
                    TargetNodeType = context.TargetNodeType
                }
            };
            pins.Add(profileUrlPin);
        }

        // Avatar URL
        if (playerSummary?.Avatar != null)
        {
            var avatarUrlPin = new Pin
            {
                Id = "steam-avatar-url",
                Label = "Avatar URL",
                Type = PinConstants.PinTypes.PlayerInfo.AvatarUrl,
                Behavior = PinBehavior.Informational,
                Summary = new PinSummary
                {
                    DisplayText = playerSummary.Avatar,
                    Icon = "image",
                    Source = "steam"
                },
                Metadata = new PinMetadata
                {
                    OriginNodeId = context.InputNodeId,
                    TargetNodeId = null,
                    TargetNodeType = context.TargetNodeType
                }
            };
            pins.Add(avatarUrlPin);
        }

        // Online Status
        if (playerSummary?.Status != null)
        {
            var onlineStatusPin = new Pin
            {
                Id = "steam-online-status",
                Label = "Online Status",
                Type = PinConstants.PinTypes.PlayerInfo.OnlineStatus,
                Behavior = PinBehavior.Informational,
                Summary = new PinSummary
                {
                    DisplayText = "TODO: Online Status",
                    Icon = "user",
                    Source = "steam"
                },
                Metadata = new PinMetadata
                {
                    OriginNodeId = context.InputNodeId,
                    TargetNodeId = null,
                    TargetNodeType = context.TargetNodeType
                }
            };
            pins.Add(onlineStatusPin);
        }

        // Last Online
        if (playerSummary?.LastLogoff != null)
        {
            var lastOnlinePin = new Pin
            {
                Id = "steam-last-online",
                Label = "Last Online",
                Type = PinConstants.PinTypes.PlayerInfo.LastOnline,
                Behavior = PinBehavior.Informational,
                Summary = new PinSummary
                {
                    Count = (int?)playerSummary.LastLogoff,
                    Icon = "clock",
                    Source = "steam"
                },
                Metadata = new PinMetadata
                {
                    OriginNodeId = context.InputNodeId,
                    TargetNodeId = null,
                    TargetNodeType = context.TargetNodeType
                }
            };
            pins.Add(lastOnlinePin);
        }

        // Account Creation Date
        if (playerSummary?.TimeCreated != null)
        {
            var accountCreationDatePin = new Pin
            {
                Id = "steam-account-creation-date",
                Label = "Account Creation Date",
                Type = PinConstants.PinTypes.PlayerInfo.AccountCreationDate,
                Behavior = PinBehavior.Informational,
                Summary = new PinSummary
                {
                    Count = (int?)playerSummary.TimeCreated,
                    Icon = "clock",
                    Source = "steam"
                },
                Metadata = new PinMetadata
                {
                    OriginNodeId = context.InputNodeId,
                    TargetNodeId = null,
                    TargetNodeType = context.TargetNodeType
                }
            };
            pins.Add(accountCreationDatePin);
        }

        return pins;
    }
}
