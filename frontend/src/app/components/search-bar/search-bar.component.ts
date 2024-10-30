import { Component, OnInit } from '@angular/core';
import { Product } from '../../models/product';

@Component({
  selector: 'app-search-bar',
  standalone: true,
  imports: [],
  templateUrl: './search-bar.component.html',
  styleUrl: './search-bar.component.css'
})
export class SearchBarComponent implements OnInit {

  allProducts: Product[] = [];
  filteredProducts: Product[] = [];
  query: string = '';



  constructor(private searchBar : SearchBarComponent) {}


  async ngOnInit(): Promise<void> {

    this.search();  
  }


  search() {

    const clearedQuery = this.query?.trim(); //Si la query es nula guarda null, sino, llama al trim y almacena lo que devuelva

    // Si NO es nulo, vacío o solo tiene espacios en blanco
    if (this.query && clearedQuery) {//trim quita espacios en blaco

      this.filteredProducts = this.allProducts.filter(product => //filter ---> devuelve todos los elementos del array que coincidan con la busqueda
      product.name.includes(clearedQuery)); //devuelve todos los pokemons que en su nombre incluya la query

    } else {
      //Si no se hace la busqueda, devuelve todos los pokemons
      this.filteredProducts = this.allProducts; 
    }
  }


}
