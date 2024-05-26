import { Injectable } from '@angular/core';
import { EdrdgEntry } from '../classes/edrdg/edrdg-entry';
import { Subject } from 'rxjs/internal/Subject';
import { Options } from '../classes/options';
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
        this.translationsArrived(message.text);
      }
    });
  }

  public translationsArrived(translations: string) {
    console.log("Received translation");
    const translationObject = JSON.parse(translations) as EdrdgEntry[];
    this.newTranslationsSubject.next(translationObject);
  }

  //Overall rule is: things other than background can read storage. Only background can write to storage.
  public async getOptions() : Promise<Options | null> {
    console.log("Getting options from comms");
    let options = null;
    let res = await chrome.storage.local.get(["options"]);
    console.log('got: ', res);
    if (res.options) {
      options = JSON.parse(res.options);
    }
    return options;
  }

  //Which is why this sends a message to background to save options.
  public saveOptions(options: Options) {
    console.log("Sending options to background");
    console.log(options);
    chrome.runtime.sendMessage({ type: 'options', text: JSON.stringify(options) });
  }
}
