import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AlertifyService } from '../services/alertify.service';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router,
    private alertify: AlertifyService) { }

  canActivate(next: ActivatedRouteSnapshot,  state: RouterStateSnapshot): boolean {
    if (this.authService.loggedIn()) {
      const roles = next.firstChild.data['roles'] as Array<string>;
      if (roles) {
        const match = this.authService.roleMatch(roles);
        if (match) {
          return true;
        } else {
          this.router.navigate(['home']);
          this.alertify.error('Vous n\'êtes pas autorisé à accéder à cette zone');
        }
      }

      if (this.authService.loggedIn()) {
        return true;
      }

      // this.alertify.error('veuillez vous connecter...');
      this.router.navigate(['signIn']);
      return false;
    }
    this.authService.redirectUrl = state.url;
    this.router.navigate(['sessions/signin']);
    return false;

  }
}
