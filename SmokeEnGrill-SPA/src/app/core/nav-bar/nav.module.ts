import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavGPanelComponent } from './navGPanel/navGPanel.component';
import { NavNotLoggedComponent } from './navNotLogged/navNotLogged.component';
import { SharedModule } from 'src/app/shared/shared.module';

@NgModule({
  imports: [
    CommonModule,
    SharedModule
  ],
  declarations: [
    NavGPanelComponent,
    NavNotLoggedComponent
  ]
})
export class NavModule { }
