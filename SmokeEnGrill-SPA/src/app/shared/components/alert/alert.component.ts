import { Component, OnInit } from '@angular/core';
import { MdbNotificationRef } from 'mdb-angular-ui-kit/notification';

@Component({
  selector: 'app-alert',
  templateUrl: './alert.component.html',
  styleUrls: ['./alert.component.scss']
})
export class AlertComponent implements OnInit {
  text: string;
  class: string;

  constructor(public notificationRef: MdbNotificationRef<AlertComponent>) { }

  ngOnInit() {
  }

}
