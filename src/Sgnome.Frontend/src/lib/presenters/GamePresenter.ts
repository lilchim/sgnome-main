import { PIN_CONSTANTS } from "$lib/constants/pinTypes";
import type { Pin } from "$lib/types/graph";

export interface GameSlugger {
    appId: string;
    gameName: string;
    headerImageUrl: string;
}

export class GamePresenter {
    getSteamGameData({pins}: {pins: Pin[]}): GameSlugger | null {
        const steamAppIdPin = pins.find(pin => pin.type === PIN_CONSTANTS.PIN_TYPES.GAME_PINS.STEAM_APP_ID);
        const gameNamePin = pins.find(pin => pin.type === PIN_CONSTANTS.PIN_TYPES.GAME_PINS.GAME_NAME);
        const headerImageUrlPin = pins.find(pin => pin.type === PIN_CONSTANTS.PIN_TYPES.GAME_PINS.HEADER_IMAGE_URL);
        console.log(pins);
        console.log(steamAppIdPin, gameNamePin);

        if (!steamAppIdPin || !gameNamePin) {
            return null;
        }

        return {
            appId: steamAppIdPin.summary.preview.steamAppId as string,
            gameName: gameNamePin.summary.preview.gameName as string,
            headerImageUrl: headerImageUrlPin?.summary.preview.headerImageUrl as string
        }
    }
}