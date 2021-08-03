import { Component, OnInit } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { map, tap } from 'rxjs/operators';

@Component({
  selector: 'app-add-order',
  templateUrl: './add-order.component.html',
  styleUrls: ['./add-order.component.scss']
})
export class AddOrderComponent implements OnInit {
  searchText = new Subject();
  results: Observable<string[]>;
  notFound = false;
  clients = [{lastName: 'moulot', mobile: '0707104446'}, {lastName: 'smokengrill', mobile: '0797979707'},
  {lastName: 'bechio', mobile: '0787878707'}, {lastName: 'rousseau', mobile: '0707101010'},
  {lastName: 'adjadja', mobile: '0710101010'}, {lastName: 'sonia', mobile: '0505050505'}]
  showClientInfo = false;

  constructor() { }

  ngOnInit() {
  }

  counter(i: number) {
    return new Array(i);
  }

  goToNextPhoneDigit(event: any, previousElement: any, nextElement: any): void {
    if (event.code !== 'Backspace' && nextElement !== null) {
        nextElement.focus();
    }

    if (event.code === 'Backspace' && previousElement !== null) {
        previousElement.focus();
        previousElement.value = '';
    }
  }

}
