import { Component, OnInit } from '@angular/core';
import { Utils } from 'src/app/shared/utils';

@Component({
  selector: 'app-add-order',
  templateUrl: './add-order.component.html',
  styleUrls: ['./add-order.component.scss']
})
export class AddOrderComponent implements OnInit {
  clients = [{id: 1, lastName: 'moulot', mobile: '0707104446'}, {id: 2, lastName: 'smokengrill', mobile: '0797979707'},
  {id: 3, lastName: 'bechio', mobile: '0787878707'}, {id: 4, lastName: 'rousseau', mobile: '0707101010'},
  {id: 5, lastName: 'adjadja', mobile: '0710101010'}, {id: 6, lastName: 'sonia', mobile: '0505050505'}]
  products = [{id: 1, name: 'poulet fumé', price: 6500}, {id: 2, name: 'ribs de porc', price: 9000},
  {id: 3, name: 'ribs de boeuf', price: 22000}, {id: 4, name: 'burger boeuf fumé', price: 6500},
  {id: 5, name: 'burger proc fumé', price: 5500}];
  datepickerOptions = Utils.translationOptions;
  showClientInfo = false;
  clientsFound = [];
  clientId = 0;
  clientFName = '';
  clientLName = '';
  clientMobile = '';
  clientAddress = '';
  totalOrder = 0;
  prodQties = [0 ,0 , 0, 0, 0, 0]

  constructor() { }

  ngOnInit() {
  }

  findClients(event) {
    var val = event.target.value;
    if (val) {
      val = val.toLowerCase();
    } else {
      return this.clientsFound = [];
    }
    const columns = Object.keys(this.clients[0]);
    if (!columns.length) {
      return;
    }

    const rows = this.clients.filter(function (d) {
      for (let i = 0; i <= columns.length; i++) {
        const column = columns[i];
        if (d[column] && d[column].toString().toLowerCase().indexOf(val) > -1) {
          return true;
        }
      }
    });
    this.clientsFound = rows;
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

  selectClient(id) {
    this.showClientInfo = true;
    const client = this.clients.find(c => c.id == id);
    this.clientId = client.id;
    this.clientLName = client.lastName;
    this.clientMobile = client.mobile;
  }

  qtyDown(index, unitPrice) {
    const oldQty = this.prodQties[index];
    if (oldQty > 0) {
      const newQty = this.prodQties[index]--;
      this.totalOrder -= unitPrice;
    }
  }

  qtyUp(index, unitPrice) {
    this.prodQties[index]++;
    this.totalOrder += unitPrice;
  }

}
