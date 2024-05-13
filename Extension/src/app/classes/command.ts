import { Dictionary } from "./dictionary";

export class Command {
    action = "";
    module = "";
    submodule = "";
    options:Dictionary<string> = {};
    optionsAsString? = "";

    static startTrackingProcessCommand:Command = {
        action: "start",
        module: "tracking",
        submodule: "process",
        options: {}
    }

    static stopTrackingProcessCommand:Command = {
        action: "stop",
        module: "tracking",
        submodule: "process",
        options: {}
    }

    static startTranslationJpCommand:Command = {
        action: "start",
        module: "translation",
        submodule: "jp",
        options: {}
    }

    static stopTranslationJpCommand:Command = {
        action: "stop",
        module: "translation",
        submodule: "jp",
        options: {}
    }
}