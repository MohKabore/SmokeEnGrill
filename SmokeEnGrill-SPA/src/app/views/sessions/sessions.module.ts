import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SessionsRoutingModule } from './sessions-routing.module';
import { SharedModule } from 'src/app/shared/shared.module';
import { SigninComponent } from './signin/signin.component';
import { LockoutComponent } from './lockout/lockout.component';
import { ForgotComponent } from './forgot/forgot.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SessionsRoutingModule,
    SharedModule
  ],
  declarations: [
    SigninComponent,
    LockoutComponent,
    ForgotComponent
  ]
})
export class SessionsModule { }
