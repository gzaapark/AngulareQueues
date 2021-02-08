import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'timespan'
})
export class TimespanPipe implements PipeTransform {

  transform(value: any, ...args: any[]): any {
    let elems = value ? String(value).split(':') : undefined;
    return elems && elems.length == 3 ? elems[0] + ':' + elems[1] : value;
  }

}
