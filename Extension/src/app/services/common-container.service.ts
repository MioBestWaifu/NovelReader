import { Injectable } from '@angular/core';
import { ExtensionCommunicationService } from './extension-communication.service';
import { EdrdgEntry } from '../classes/edrdg/edrdg-entry';
import { Options } from '../classes/options';

@Injectable({
  providedIn: 'root'
})
export class CommonContainerService {
  allTranslations:EdrdgEntry[] = [];
  //May be null if the extension was never used before or if the user cleared the storage.
  initialOptions!: Options | null;

  constructor(comms:ExtensionCommunicationService) {
    comms.newTranslationsEvent.subscribe((translations:EdrdgEntry[]) => {
      this.allTranslations = this.allTranslations.concat(translations);
    });

    comms.getOptions().then((options:Options | null) => {
      this.initialOptions = options;
    });
  }
}
