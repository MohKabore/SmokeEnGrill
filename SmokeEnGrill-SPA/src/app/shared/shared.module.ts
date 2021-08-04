import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MdbSelectComponent, MdbSelectModule } from 'mdb-angular-ui-kit/select';
import { MdbCollapseModule } from 'mdb-angular-ui-kit/collapse';
import { MdbFormsModule } from 'mdb-angular-ui-kit/forms';
import { MdbAutocompleteModule } from 'mdb-angular-ui-kit/autocomplete';
import { MdbDatepickerModule } from 'mdb-angular-ui-kit/datepicker';
import { MdbTimepickerModule } from 'mdb-angular-ui-kit/timepicker';

@NgModule({
  imports: [
    CommonModule,
    MdbSelectModule,
    MdbCollapseModule,
    MdbFormsModule,
    MdbAutocompleteModule,
    MdbDatepickerModule,
    MdbTimepickerModule
  ],
  declarations: [

  ],
  exports: [
    MdbSelectComponent,
    MdbCollapseModule,
    MdbFormsModule,
    MdbAutocompleteModule,
    MdbDatepickerModule,
    MdbTimepickerModule
  ]
})
export class SharedModule { }
