import { Component, Input, OnInit } from '@angular/core';
import { EdrdgEntry } from '../../classes/edrdg/edrdg-entry';

@Component({
  selector: 'mrx-translation-card',
  templateUrl: './translation-card.component.html',
  styleUrl: './translation-card.component.scss'
})
export class TranslationCardComponent implements OnInit {

  @Input() entry!: EdrdgEntry;
  kanjisJoined = "";
  readingsJoined = "";

  //Maybe should be onChanges
  ngOnInit(): void {
    let buffer: string[] = [];

    if (this.entry.kanjiElements) {
      for (let kanji of this.entry.kanjiElements) {
        buffer.push(kanji.kanji);
      }
      this.kanjisJoined = buffer.join(", ");
    }

    buffer = [];

    for (let reading of this.entry.readingElements) {
      buffer.push(reading.reading);
    }
    this.readingsJoined = buffer.join(",  ");

  }
}
