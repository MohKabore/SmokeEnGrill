import { Injectable } from '@angular/core';
import { MdbNotificationService, MdbNotificationRef } from 'mdb-angular-ui-kit/notification';
import { AlertComponent } from 'src/app/shared/components/alert/alert.component';

declare let alertify: any;

@Injectable({
  providedIn: 'root'
})
export class AlertifyService {
  notificationRef: MdbNotificationRef<AlertComponent>;

  constructor(private notificationService: MdbNotificationService) { }

  options = {
    opacity: 0.8,
    timeOut: 3000,
    positionClass: 'toast-bottom-right'
  };

  confirm(message: string, okCallback: () => any) {
    alertify.confirm(message, function(e) {
      if (e) {
        okCallback();
      } else {}
    });
  }

  success(message: string) {
    this.notificationRef = this.notificationService.open(AlertComponent, { data: { text: message, class: 'alert alert-success' } });
  }
  warning(message: string) {
    this.notificationRef = this.notificationService.open(AlertComponent, { data: { text: message, class: 'alert alert-warning' } });
  }
  info(message: string) {
    this.notificationRef = this.notificationService.open(AlertComponent, { data: { text: message, class: 'alert alert-info' } });
  }
  error(message: string) {
    this.notificationRef = this.notificationService.open(AlertComponent, { data: { text: message, class: 'alert alert-error' } });
  }

}
