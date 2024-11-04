import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-search-bar',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './search-bar.component.html',
  styleUrl: './search-bar.component.css'
})
export class SearchBarComponent implements OnInit {
  query: string = '';

  @Output() newItemEvent = new EventEmitter<string>();

  async ngOnInit(): Promise<void> {
    const query = sessionStorage.getItem("query");
    if (query) {
      this.query = query;
      await this.search();
    }
  }

  async search() {
    const clearedQuery = this.query.trim(); //Si la query es nula guarda null, sino, llama al trim y almacena lo que devuelva
    this.newItemEvent.emit(clearedQuery);
  }
}
