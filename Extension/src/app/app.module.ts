import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { OptionsPageComponent } from './components/pages/options-page/options-page.component';
import { TranslationsPageComponent } from './components/pages/translations-page/translations-page.component';
import { NavbarComponent } from './components/structure/navbar/navbar.component';
import { TranslationCardComponent } from './components/translation-card/translation-card.component';
import { FormsModule } from '@angular/forms';
import { CommandPageComponent } from './components/pages/command-page/command-page.component';
import { HttpClientModule } from '@angular/common/http';

@NgModule({
  declarations: [
    AppComponent,
    OptionsPageComponent,
    TranslationsPageComponent,
    NavbarComponent,
    TranslationCardComponent,
    CommandPageComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
