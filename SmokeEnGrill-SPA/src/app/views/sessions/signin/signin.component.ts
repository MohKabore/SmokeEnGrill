import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router, RouteConfigLoadStart, ResolveStart, RouteConfigLoadEnd, ResolveEnd, ActivatedRouteSnapshot, ActivatedRoute } from '@angular/router';
import { AuthService } from 'src/app/core/services/auth.service';
import { AlertifyService } from 'src/app/core/services/alertify.service';
import { User } from 'src/app/shared/models/user';

@Component({
    selector: 'app-signin',
    templateUrl: './signin.component.html',
    styleUrls: ['./signin.component.scss']
})
export class SigninComponent implements OnInit {
  model: any = {};
  user: User;
  verifyModel: any = {};
  notConfirmed: boolean;
  showValidationForm: boolean;
  isVisible: boolean;
  passwordForm: FormGroup;
  signinForm: FormGroup;
  loading: boolean;
  loadingText: string;
  formValid: boolean;
  infos: any;
  showInfoBox = false;
  alertInfo: string;
  maxFailed = 3;
  failedCount = 0;

  constructor(private authService: AuthService, private router: Router, private fb: FormBuilder,
      private route: ActivatedRoute, private alertify: AlertifyService) { }

  ngOnInit() {
    if (this.loggedIn()) {
      this.router.navigate(['/home']);
    } else {
      this.router.events.subscribe(event => {
        if (event instanceof RouteConfigLoadStart || event instanceof ResolveStart) {
          this.loadingText = 'Loading Dashboard Module...';
          this.loading = true;
        }
        if (event instanceof RouteConfigLoadEnd || event instanceof ResolveEnd) {
          this.loading = false;
        }
      });

      this.signinForm = this.fb.group({
        username: ['', Validators.required],
        password: ['', Validators.required]
      });
    }
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

  signin() {
    this.loading = true;
    this.notConfirmed = false;
    this.authService.login(this.signinForm.value).subscribe(() => {
      if (this.authService.lockedOut) {
        this.router.navigate(['/lockout']);
      } else if (this.authService.loginPwdFailed) {
        this.alertify.error('utilisateur ou mot de passe incorrect...');
        this.failedCount++;
        this.showInfoBox = true;
      } else {
        this.user = this.authService.currentUser;
        if (this.user.emailConfirmed && this.user.phoneNumberConfirmed && this.user.validated) {
          this.router.navigate(['/home']);
        } else {
          // this.router.navigate(['invalidAccount']);
          this.router.navigate(['/home']);
        }
        this.loading = false;
      }
    }, error => {
      this.alertify.error(error);
      this.loading = false;
    });
  }

}
