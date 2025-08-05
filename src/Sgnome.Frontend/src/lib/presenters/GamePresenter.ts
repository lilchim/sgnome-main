import { PIN_CONSTANTS } from "$lib/constants/pinTypes";
import type { Pin } from "$lib/types/graph";

export interface GameSlugger {
    appId: string;
    gameName: string;
    headerImageUrl: string;
}

export interface SteamGameAppDetails {
    appId: string;
    name: string;
    headerImageUrl: string;
    releaseDate: string;
    publishers: string[];
    developers: string[];
    genres: string[];
    platforms: string[];
    descriptionShort: string;
    descriptionLong: string;
    website: string;
}

export class GamePresenter {
    getSteamGameData({pins}: {pins: Pin[]}): SteamGameAppDetails | null {
        const steamAppIdPin = pins.find(pin => pin.type === PIN_CONSTANTS.PIN_TYPES.GAME_PINS.STEAM_APP_ID);
        const gameNamePin = pins.find(pin => pin.type === PIN_CONSTANTS.PIN_TYPES.GAME_PINS.GAME_NAME);
        const headerImageUrlPin = pins.find(pin => pin.type === PIN_CONSTANTS.PIN_TYPES.GAME_PINS.HEADER_IMAGE_URL);
        const releaseDatePin = pins.find(pin => pin.type === PIN_CONSTANTS.PIN_TYPES.GAME_PINS.RELEASE_DATE);
        const publisherPins = pins.filter(pin => pin.type === PIN_CONSTANTS.PIN_TYPES.GAME_PINS.PUBLISHER);
        const developerPins = pins.filter(pin => pin.type === PIN_CONSTANTS.PIN_TYPES.GAME_PINS.DEVELOPER);
        const genrePins = pins.filter(pin => pin.type === PIN_CONSTANTS.PIN_TYPES.GAME_PINS.GENRE);
        const platformPins = pins.filter(pin => pin.type === PIN_CONSTANTS.PIN_TYPES.GAME_PINS.PLATFORM);
        const descriptionShortPin = pins.find(pin => pin.type === PIN_CONSTANTS.PIN_TYPES.GAME_PINS.DESCRIPTION_SHORT);
        const descriptionLongPin = pins.find(pin => pin.type === PIN_CONSTANTS.PIN_TYPES.GAME_PINS.DESCRIPTION_LONG);
        const websitePin = pins.find(pin => pin.type === PIN_CONSTANTS.PIN_TYPES.GAME_PINS.WEBSITE);

        if (!steamAppIdPin || !gameNamePin) {
            return null;
        }

        return {
            appId: steamAppIdPin.summary.preview.steamAppId as string,
            name: gameNamePin.summary.preview.gameName as string,
            headerImageUrl: headerImageUrlPin?.summary.preview.headerImageUrl as string,
            releaseDate: releaseDatePin?.summary.preview.releaseDate as string,
            publishers: publisherPins.map(pin => pin.summary.preview.publisher as string),
            developers: developerPins.map(pin => pin.summary.preview.developer as string),
            genres: genrePins.map(pin => pin.summary.preview.genreDescription as string),
            platforms: platformPins.map(pin => pin.summary.preview.platform as string),
            descriptionShort: descriptionShortPin?.summary.preview.description as string,
            descriptionLong: descriptionLongPin?.summary.preview.descriptionLong as string,
            website: websitePin?.summary.preview.website as string
        }
    }
}