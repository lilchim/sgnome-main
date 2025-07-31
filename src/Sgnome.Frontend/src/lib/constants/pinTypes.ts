export const PIN_CONSTANTS = {
    SOURCES: {
        STEAM: 'steam',
        EPIC: 'epic',
        RAWG: 'rawg',
        INTERNAL: 'internal'
    },
    PIN_TYPES: {
        PLAYER_INFO: {
            DISPLAY_NAME: 'player-info:display-name',
            REAL_NAME: 'player-info:real-name',
            PROFILE_URL: 'player-info:profile-url',
            AVATAR_URL: 'player-info:avatar-url',
            ONLINE_STATUS: 'player-info:online-status',
            LAST_ONLINE: 'player-info:last-online',
            ACCOUNT_CREATION_DATE: 'player-info:created-on'
        },
        LIBRARY_PINS: {
            LIBRARY: 'library:library',
            LIBRARY_LIST: 'library:library-list'
        }
    }
} as const;