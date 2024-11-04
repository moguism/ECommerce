import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'eurosToCents',
  standalone: true
})
export class EurosToCentsPipe implements PipeTransform {

  transform(cents: number): unknown {
    const euros: string = (cents / 100).toLocaleString("en-US", {style:"currency", currency:"EUR"});
    return euros;
  }

}
