import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MdbCollapseModule } from 'mdb-angular-ui-kit/collapse';

@NgModule({
  imports: [
    CommonModule,
    MdbCollapseModule
  ],
  declarations: [

  ],
  exports: [
    MdbCollapseModule
  ]
})
export class SharedModule { }
