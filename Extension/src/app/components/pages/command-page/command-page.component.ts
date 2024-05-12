import { ChangeDetectorRef, Component } from '@angular/core';
import { Command } from '../../../classes/command';
import { ServicesCommunicationService } from '../../../services/service-communication.service';

@Component({
  selector: 'mrx-command-page',
  templateUrl: './command-page.component.html',
  styleUrl: './command-page.component.scss'
})
export class CommandPageComponent {
  command = new Command();

  constructor(private servicesComms:ServicesCommunicationService,private cdr:ChangeDetectorRef) {
  }

  sendCommand(){
    this.servicesComms.sendCommand(this.command).subscribe(
      (response) => {
        console.log(response);
      }
    );
  }

  startTrackingProcessCommand(){
    this.command = Command.startTrackingProcessCommand;
    this.cdr.detectChanges();
  }

  stopTrackingProcessCommand(){
    this.command = Command.stopTrackingProcessCommand;
    this.cdr.detectChanges();
  }

  startTranslationJpCommand(){
    this.command = Command.startTranslationJpCommand;
    this.cdr.detectChanges();
  }

  stopTranslationJpCommand(){
    this.command = Command.stopTranslationJpCommand;
    this.cdr.detectChanges();
  }
}
