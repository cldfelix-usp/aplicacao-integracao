import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'device',
  standalone: true
})
export class DevicePipe implements PipeTransform {

  transform(value: unknown, ...args: unknown[]): unknown {
    return null;
  }

}
