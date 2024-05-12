import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { OptionsPageComponent } from './components/pages/options-page/options-page.component';
import { TranslationsPageComponent } from './components/pages/translations-page/translations-page.component';
import { NavbarComponent } from './components/structure/navbar/navbar.component';
import { TranslationCardComponent } from './components/translation-card/translation-card.component';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    AppComponent,
    OptionsPageComponent,
    TranslationsPageComponent,
    NavbarComponent,
    TranslationCardComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
