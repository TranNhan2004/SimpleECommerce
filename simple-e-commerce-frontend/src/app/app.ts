import { Component } from '@angular/core';
import { environment } from '../environments/environment';
import { Navbar } from "./shared/navbar/navbar";

@Component({
  selector: 'app-root',
  imports: [Navbar],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  constructor() {
    console.log('App component initialized');
    console.log('Welcome to the Simple E-Commerce Frontend!');
    // console.log(`API URL: ${environment.apiUrl}`);
    // console.log(`Environment: ${environment.production ? 'Production' : 'Development'}`);
    // console.log(`Keycloak URL: ${environment.keycloak.url}`);
    // console.log(`Keycloak Realm: ${environment.keycloak.realm}`);
    // console.log(`Keycloak Client ID: ${environment.keycloak.clientId}`);
  }
}
