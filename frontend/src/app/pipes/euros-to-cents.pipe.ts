import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'eurosToCents',
  standalone: true
})
export class EurosToCentsPipe implements PipeTransform {

  transform(cents: number): unknown {
    const euros: string = (cents / 100).toLocaleString("es-ES", {style:"currency", currency:"EUR"});
    return euros;
  }

}
