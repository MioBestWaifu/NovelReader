import { KanjiElement } from "./kanji-element";
import { ReadingElement } from "./reading-element";
import { SenseElement } from "./sense-element";

export class EdrdgEntry {
    entryId!:number;
    kanjiElements?: KanjiElement[];
    readingElements!: ReadingElement[];
    senseElements!: SenseElement[];
}