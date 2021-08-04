import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { NavGPanelComponent } from './nav-bar/navGPanel/navGPanel.component';
import { RouterModule } from '@angular/router';

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
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
