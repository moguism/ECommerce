import { Component, OnInit } from '@angular/core';
import { SearchBarComponent } from '../../components/search-bar/search-bar.component';
import { HeaderComponent } from '../../components/header/header.component';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product.service';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [HeaderComponent, SearchBarComponent],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.css'
})
export class ProductListComponent implements OnInit {


  allProducts: Product[] | null = [];
  query: string = '';
  filteredProducts: Product[] = [];

  
  constructor(private productService : ProductService){}



  async ngOnInit(): Promise<void> {

    this.getProducts();

  }


  async getProducts() {
    const request = await this.productService.getAllProducts();

    if (request.success) {
      this.allProducts = request.data;
    }

    console.log(this.allProducts); 
  }



}
