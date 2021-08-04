import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';

const routes: Routes = [
  {
    path: 'sessions',
    loadChildren: () => import('./views/sessions/sessions.module').then(m => m.SessionsModule)
  },

  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [
      {
        path: 'home',
        loadChildren: () => import('./views/home/home.module').then(m => m.HomeModule)
      },
      {
        path: 'orders',
        loadChildren: () => import('./views/orders/orders.module').then(m => m.OrdersModule)
      }
    ]
  }

];

@NgModule({
  imports: [RouterModule.forRoot(routes, { relativeLinkResolution: 'legacy' })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
