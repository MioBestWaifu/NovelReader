import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { CommonContainerService } from '../../../services/common-container.service';
import { ExtensionCommunicationService } from '../../../services/extension-communication.service';

@Component({
  selector: 'mrx-translations-page',
  templateUrl: './translations-page.component.html',
  styleUrl: './translations-page.component.scss'
})
export class TranslationsPageComponent implements OnInit{

  translations = this.common.allTranslations;
  constructor(public comms:ExtensionCommunicationService, public common:CommonContainerService,private cdr: ChangeDetectorRef) {
    
  }

  ngOnInit(): void {
    this.comms.newTranslationsEvent.subscribe((translations) => {
      console.log("translations page received");
      this.translations.push(...translations);
      this.cdr.detectChanges();
    });
  }


}
