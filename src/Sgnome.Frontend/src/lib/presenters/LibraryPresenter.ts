import { PIN_CONSTANTS } from "$lib/constants/pinTypes";
import type { Pin } from "../types/graph";

export class LibraryPresenter {

    getLibraryLabel({pins}: {pins: Pin[]}): string {
        let libraryPins = pins.filter(pin => pin.type === PIN_CONSTANTS.PIN_TYPES.LIBRARY_PINS.LIBRARY);
        return libraryPins.length > 0 ? libraryPins[0].label : 'Library';
    }

    getGameListWidgetInput(pins: Pin[]): { gamePins: Pin[] } {
        let gamePins = pins.filter(pin => pin.type === PIN_CONSTANTS.PIN_TYPES.GAME_PINS.GAME);
        return { gamePins };
    }


}