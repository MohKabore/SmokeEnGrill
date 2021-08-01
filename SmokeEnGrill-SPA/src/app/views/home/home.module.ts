import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HomeComponent } from './home/home.component';
import { HomeRoutingModule } from './home-routing.module';
import { MdbSelectModule } from 'mdb-angular-ui-kit/select'

@NgModule({
  imports: [
    CommonModule,
    HomeRoutingModule,
    MdbSelectModule
  ],
  declarations: [HomeComponent]
})
export class HomeModule { }
