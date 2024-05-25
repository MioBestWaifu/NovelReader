import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HttpHeaders } from '@angular/common/http';
import { Command } from '../classes/command';

@Injectable({
  providedIn: 'root'
})

//Thats a silly name, but what can be done? It IS a service to communicate with Services.
export class ServicesCommunicationService {
  private baseUrl = "http://localhost:47100/";
  
  private defaultOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor(private http:HttpClient) {
  }

  sendCommand(command:Command){
    //As of 0.4 (May 12th, 2024), Services does not care about the path
    return this.http.post(this.baseUrl, command, this.defaultOptions);
  }
}
