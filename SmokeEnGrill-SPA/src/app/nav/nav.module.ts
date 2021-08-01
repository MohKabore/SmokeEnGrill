import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavGPanelComponent } from './navGPanel/navGPanel.component';
import { NavNotLoggedComponent } from './navNotLogged/navNotLogged.component';

@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [
    NavGPanelComponent,
    NavNotLoggedComponent
  ]
})
export class NavModule { }
