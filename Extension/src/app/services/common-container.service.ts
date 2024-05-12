import { Injectable } from '@angular/core';
import { ExtensionCommunicationService } from './extension-communication.service';
import { EdrdgEntry } from '../classes/edrdg/edrdg-entry';

@Injectable({
  providedIn: 'root'
})
export class CommonContainerService {
  allTranslations:EdrdgEntry[] = []

  constructor(comms:ExtensionCommunicationService) {
    comms.newTranslationsEvent.subscribe((translations:EdrdgEntry[]) => {
      this.allTranslations = this.allTranslations.concat(translations);
    });
  }
}
