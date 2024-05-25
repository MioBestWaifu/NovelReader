import { Component } from '@angular/core';
import { Options } from '../../../classes/options';
import { ExtensionCommunicationService } from '../../../services/extension-communication.service';
import { CommonContainerService } from '../../../services/common-container.service';

@Component({
  selector: 'mrx-options-page',
  templateUrl: './options-page.component.html',
  styleUrl: './options-page.component.scss'
})
export class OptionsPageComponent {
  trackPages!:boolean;
  translateSelection!:boolean;

  //There is no subscription for options changes from elsewhere because that is not suposed to happen.
  constructor(private comms:ExtensionCommunicationService, private common:CommonContainerService){
    let options = this.common.initialOptions;
    if (options) {
      this.trackPages = options.trackPages;
      this.translateSelection = options.translateSelection;
    }
  }


  // 0 = trackPages; 1 = translateSelection.
  onOptionChange(switchCode:number) {
    switch (switchCode) {
      case 0:
        this.trackPages = !this.trackPages;
        this.sendOptionsToExtension();
        break;
      case 1:
        this.translateSelection = !this.translateSelection;
        this.sendOptionsToExtension();
        break;
    }
  }

  buildOptions(): Options {
    let options = new Options();
    options.trackPages = this.trackPages;
    options.translateSelection = this.translateSelection;
    return options;
  }

  sendOptionsToExtension(){
    let options = this.buildOptions();
    this.comms.saveOptions(options);
  }

  
}
