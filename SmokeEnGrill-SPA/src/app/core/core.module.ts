import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { NavGPanelComponent } from './nav-bar/navGPanel/navGPanel.component';

@NgModule({
  imports: [
    CommonModule,
    SharedModule
  ],
  declarations: [
    NavGPanelComponent
  ],
  exports: [
    NavGPanelComponent
  ]
})
export class CoreModule { }
