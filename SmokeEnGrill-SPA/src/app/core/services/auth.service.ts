import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
import { Router } from '@angular/router';
import { AlertifyService } from './alertify.service';
import { User } from 'src/app/shared/models/user';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseUrl = environment.apiUrl + 'auth/';
  redirectUrl: string;
  adminTypeId = environment.adminTypeId;
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  currentUser: User;
  lockedOut = false;
  loginPwdFailed = false;
  settings: any;
  newUser = <User>{ id: 0 };

  photoUrl = new BehaviorSubject<string>('../../assets/user.png');
  currentPhotoUrl = this.photoUrl.asObservable();

  constructor(private http: HttpClient, private alertify: AlertifyService,
    private router: Router) { }

  changeUserPhoto(photoUrl: string) {
    this.photoUrl.next(photoUrl);
  }

  login(model: any) {
    return this.http.post(this.baseUrl + 'login', model)
      .pipe(
        map((response: any) => {
          const user = response;
          this.lockedOut = false;
          this.loginPwdFailed = false;
          if (user) {
            if (user.lockedOut) {
              this.lockedOut = true;
            } else if (user.failed) {
              this.loginPwdFailed = true;
            } else {
              localStorage.setItem('token', user.token);
              localStorage.setItem('user', JSON.stringify(user.user));
              localStorage.setItem('settings', JSON.stringify(user.settings));
              this.decodedToken = this.jwtHelper.decodeToken(user.token);
              this.currentUser = user.user;
              this.settings = user.settings;
              this.changeUserPhoto(this.currentUser.photoUrl);
            }
          }
        })
      );
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    localStorage.removeItem('settings');
    localStorage.removeItem('currentPeriod');
    localStorage.removeItem('currentChild');
    localStorage.removeItem('currentClassId');
    this.decodedToken = null;
    this.currentUser = null;
    this.settings = null;
    this.alertify.info('vous êtes déconnecté');
    if (this.redirectUrl) {
      this.router.navigate(['/signin']);
      // this.router.navigate([this.redirectUrl]);
    } else {
      this.router.navigate(['/signin']);
    }
  }

  register(user: User) {
    return this.http.post(this.baseUrl + 'register', user);
  }

  setUserPassword(id: number, password: string) {
    return this.http.post(this.baseUrl + id + '/setPassword/' + password, {})
      .pipe(
        map((response: any) => {
          const user = response;
          if (user) {
            localStorage.setItem('token', user.token);
            localStorage.setItem('user', JSON.stringify(user.user));
            this.decodedToken = this.jwtHelper.decodeToken(user.token);
            this.currentUser = user.user;
            this.changeUserPhoto(this.currentUser.photoUrl);
          }
        })
      );
  }

  getUser(id) {
    return this.http.get(this.baseUrl + 'GetUser/' + id);
  }

  resendConfrimEmail() {

  }

  setUserAccountData(id: number, userData: any) {
    return this.http.post(this.baseUrl + id + '/setUserAccountData', userData);
  }

  forgotPassord(model: any) {
    return this.http.post(this.baseUrl + 'ForgotPassword', model);
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    if (token !== 'undefined') {
      return !this.jwtHelper.isTokenExpired(token);
    }
    return false;
  }

  accountValidated() {
    if (this.loggedIn() === true) {
      return this.currentUser.validated;
    } else {
      return false;
    }
  }

  adminLoggedIn() {
    return this.currentUser.userTypeId === this.adminTypeId;
  }

  roleMatch(allowedRoles): boolean {
    let isMatch = false;
    const userRoles = this.decodedToken.role as Array<string>;
    allowedRoles.forEach(elt => {
      if (userRoles.includes(elt)) {
        isMatch = true;
        return;
      }
    });
    return isMatch;
  }

  confirmEmail(token: string) {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.decodedToken = null;
    this.currentUser = null;
    return this.http.get(this.baseUrl + 'emailValidation/' + token);
  }

  resetPassword(token: string) {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.decodedToken = null;
    this.currentUser = null;
    return this.http.get(this.baseUrl + 'ResetPassword/' + token);

  }

  getCities() {
    return this.http.get(this.baseUrl + 'GetAllCities');
  }

  getDistrictsByCityId(id: number) {
    return this.http.get(this.baseUrl + 'GetDistrictsByCityId/' + id);
  }

  emailExist(email: string) {
    return this.http.get(this.baseUrl + email + '/VerifyEmail');
  }

  userNameExist(userName: string, currentUserId: number) {
    return this.http.get(this.baseUrl + 'VerifyUserName/' + currentUserId + '/' + userName);
  }

  addUserPhoto(userId: number, photo: any) {
    return this.http.post(this.baseUrl + userId + '/AddPhotoForUser', photo);
  }

}
