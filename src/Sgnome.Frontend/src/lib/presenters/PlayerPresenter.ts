import { PIN_CONSTANTS } from "$lib/constants/pinTypes";
import type { Pin } from "../types/graph";

export class PlayerPresenter {
    getDisplayName({pins}: {pins: Pin[]}): string {
        let displayNamePins = pins.filter(pin => pin.type === PIN_CONSTANTS.PIN_TYPES.PLAYER_INFO.DISPLAY_NAME);
        return displayNamePins.length > 0 ? displayNamePins[0].summary.displayText : 'Unknown Player';
    }
    
    getLibraryCount({pins}: {pins: Pin[]}): number {
        let libraryPins = pins.filter(pin => pin.type === PIN_CONSTANTS.PIN_TYPES.LIBRARY_PINS.LIBRARY);
        return libraryPins.length;
    }

    getAvatarUrl({pins}: {pins: Pin[]}): string {
        let avatarUrlPins = pins.filter(pin => pin.type === PIN_CONSTANTS.PIN_TYPES.PLAYER_INFO.AVATAR_URL);
        return avatarUrlPins.length > 0 ? avatarUrlPins[0].summary.displayText : '';
    }

    getProfilesBySource({pins}: {pins: Pin[]}): Record<string, any> {
        let result: Record<string, any> = {};
        for(let source of Object.values(PIN_CONSTANTS.SOURCES)) {
            let pinsBySource = pins.filter(pin => pin.summary.source === source);
            result[source] = pinsBySource;
        }
        console.log(result);
        return result;
    }

    getAvailableProfileSources({pins}: {pins: Pin[]}): string[] {
        let result: string[] = [];
        for(let source of Object.values(PIN_CONSTANTS.SOURCES)) {
            let pinsBySource = pins.filter(pin => pin.summary.source === source);
            if(pinsBySource.length > 0) {
                result.push(source);
            }
        }
        return result;
    }

    getLibraryPins({pins}: {pins: Pin[]}): Pin[] {
        const r = pins.filter(pin => pin.type === PIN_CONSTANTS.PIN_TYPES.LIBRARY_PINS.LIBRARY);
        console.log(r);
        return r;
    }
}