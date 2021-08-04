export class Utils {
  static phoneMask = [/\d/, /\d/, '.', /\d/, /\d/, '.', /\d/, /\d/, '.', /\d/, /\d/, '.', /\d/, /\d/];
  static phone10digitsMask = [/\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/, /\d/];
  static birthDateMask = [/\d/, /\d/, '/', /\d/, /\d/, '/', /\d/, /\d/, /\d/, /\d/];
  static validEmailRegex = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

  static translationOptions = {
    title: 'choisir une date',
    monthsFull: [
      'Janvier',
      'Février',
      'Mars',
      'Avril',
      'Mai',
      'Juin',
      'Juillet',
      'Aout',
      'Septembre',
      'Octobre',
      'Novembre',
      'Decembre',
    ],
    monthsShort: [
      'Jan',
      'Fev',
      'Mar',
      'Avr',
      'Mai',
      'Jun',
      'Jul',
      'Aou',
      'Sep',
      'Oct',
      'Nov',
      'Dec',
    ],
    weekdaysFull: ['Dimanche', 'Lundi', 'Mardi', 'Mercredi', 'Jeudi', 'Vendredi', 'Samedi'],
    weekdaysShort: ['Dim', 'Lun', 'Mar', 'Mer', 'Jeu', 'Ven', 'Sam'],
    weekdaysNarrow: ['D', 'L', 'M', 'M', 'J', 'F', 'S'],
    okBtnText: 'Ok',
    clearBtnText: 'Effacer',
    cancelBtnText: 'Annuler',
    format: 'dd/mm/yyyy',
    closeAfterSelect: true
  };

  static isMobile() {
    return window && window.matchMedia('(max-width: 767px)').matches;
  }
  static ngbDateToDate(ngbDate: { month, day, year }) {
    if (!ngbDate) {
        return null;
    }
    return new Date(`${ngbDate.month}/${ngbDate.day}/${ngbDate.year}`);
  }

  static dateToNgbDate(date: Date) {
    if (!date) {
        return null;
    }
    date = new Date(date);
    return { month: date.getMonth() + 1, day: date.getDate(), year: date.getFullYear() };
  }

  static scrollToTop(selector: string) {
    if (document) {
      const element = <HTMLElement>document.querySelector(selector);
      element.scrollTop = 0;
    }
  }

  static genId() {
      let text = '';
      const possible = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
      for (let i = 0; i < 5; i++) {
          text += possible.charAt(Math.floor(Math.random() * possible.length));
      }
      return text;
  }

  // date : dd/mm/yyyy where char = /
  static inputDateDDMMYY(date: string, char: string) {
      const dt = date.split(char);
      return new Date(Number(dt[2]), Number(dt[1]) - 1, Number(dt[0]));
  }

  static smoothScrollToTop(step: number) {
      const scrollToTop = window.setInterval(() => {
        const pos = window.pageYOffset;
        if (pos > 0) {
            window.scrollTo(0, pos - step); // how far to scroll on each step
        } else {
            window.clearInterval(scrollToTop);
        }
      }, 10);
  }

  static isNumber(value: string | number): boolean {
    return ((value != null) && (value !== '') && !isNaN(Number(value.toString())));
  }

  static formatPhoneNumber(num: string) {
    if (num.length !== 8) {
      return num;
    }
    return num.substr(0, 1) + '.' + num.substr(2, 1) + num.substr(4, 1) + '.' + num.substr(6, 1);
  }
}