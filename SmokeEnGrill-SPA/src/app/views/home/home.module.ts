import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HomeComponent } from './home/home.component';
import { HomeRoutingModule } from './home-routing.module';
import { MdbSelectModule } from 'mdb-angular-ui-kit/select';
import { SharedModule } from 'src/app/shared/shared.module';

@NgModule({
  imports: [
    CommonModule,
    HomeRoutingModule,
    MdbSelectModule,
    SharedModule

  ],
  declarations: [
    HomeComponent
  ]
})
export class HomeModule { }
