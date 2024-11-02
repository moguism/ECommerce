import { Component, EventEmitter, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-search-bar',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './search-bar.component.html',
  styleUrl: './search-bar.component.css'
})
export class SearchBarComponent {
  query: string = '';

  @Output() newItemEvent = new EventEmitter<string>();

  async search() {
    const clearedQuery = this.query.trim(); //Si la query es nula guarda null, sino, llama al trim y almacena lo que devuelva
    this.newItemEvent.emit(clearedQuery);
  }
}
