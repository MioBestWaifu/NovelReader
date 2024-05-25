import { Component } from '@angular/core';
import { ExtensionCommunicationService } from './services/extension-communication.service';
import { CommonContainerService } from './services/common-container.service';
import { Router } from '@angular/router';

@Component({
  selector: 'mrx-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'Maria';

  //just forcing angular to instantiate the service
  constructor(comms:ExtensionCommunicationService, common:CommonContainerService, private router:Router){

  }

  ngOnInit() {
    this.router.navigateByUrl('/command');
  }
}
