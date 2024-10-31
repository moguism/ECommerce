import { Component, EventEmitter, Output } from '@angular/core';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product.service';
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

  @Output() newItemEvent = new EventEmitter<Product[] | null>();

  constructor(private productService: ProductService) {}

  async search() {
    const clearedQuery = this.query.trim(); //Si la query es nula guarda null, sino, llama al trim y almacena lo que devuelva
    if (clearedQuery) {
      const productsNames = await this.productService.getProductByName(clearedQuery);
      console.log(productsNames)
      //this.newItemEvent.emit(products.data);
    }
  }
}
