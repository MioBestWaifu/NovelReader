import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { OptionsPageComponent } from './components/pages/options-page/options-page.component';
import { TranslationsPageComponent } from './components/pages/translations-page/translations-page.component';
import { NavbarComponent } from './components/structure/navbar/navbar.component';

@NgModule({
  declarations: [
    AppComponent,
    OptionsPageComponent,
    TranslationsPageComponent,
    NavbarComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
