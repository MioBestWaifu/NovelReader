import { Injectable } from '@angular/core';
import { EdrdgEntry } from '../classes/edrdg/edrdg-entry';
import { Subject } from 'rxjs/internal/Subject';
declare var chrome: any;

@Injectable({
  providedIn: 'root'
})
export class ExtensionCommunicationService {

  private newTranslationsSubject = new Subject<EdrdgEntry[]>();
  public newTranslationsEvent = this.newTranslationsSubject.asObservable();

  constructor() {
    chrome.runtime.onMessage.addListener((message: any, sender: any, sendResponse: any) => {
      if (message && message.type === 'translation') {
        this.TranslationsArrived(message.text);
      }
    });
  }

  public TranslationsArrived(translations: string) {
    console.log("Received translation");
    const translationObject = JSON.parse(translations) as EdrdgEntry[];
    this.newTranslationsSubject.next(translationObject);
  }
}
