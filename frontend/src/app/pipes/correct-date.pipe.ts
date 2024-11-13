import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'correctDate',
  standalone: true
})
export class CorrectDatePipe implements PipeTransform {

  transform(value: Date): string {
    // "new Date()" siempre toma la fecha del usuario local, y con "Z" le dice que tenga en cuenta que es formato UTC (porque si no le da la paja al convertir)
    const date = new Date(value + 'Z')
    return date.toLocaleString(); // Creo que poniendo "toLocale" queda más claro lo que hace más que con otra cosa xD
  }

}
