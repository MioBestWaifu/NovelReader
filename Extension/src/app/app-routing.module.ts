import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OptionsPageComponent } from './components/pages/options-page/options-page.component';
import { TranslationsPageComponent } from './components/pages/translations-page/translations-page.component';

const routes: Routes = [
  {path:'options',component:OptionsPageComponent},
  {path:'translations',component:TranslationsPageComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { initialNavigation: 'enabledBlocking' })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
