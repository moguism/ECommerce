import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'translate',
  standalone: true
})
export class TranslatePipe implements PipeTransform {

  transform(value: string): string {
    switch(value)
    {
      case "fruits":
        return "Frutas"
      case "meat":
        return "Carne"
      case "vegetables":
        return "Verduras"
    }
    return "";
  }

}
