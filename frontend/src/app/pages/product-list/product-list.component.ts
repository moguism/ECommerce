import { Component, OnDestroy, OnInit } from '@angular/core';
import { SearchBarComponent } from '../../components/search-bar/search-bar.component';
import { HeaderComponent } from '../../components/header/header.component';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product.service';
import { Subscription, forkJoin, lastValueFrom } from 'rxjs';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [HeaderComponent, SearchBarComponent],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.css'
})
export class ProductListComponent implements OnInit, OnDestroy {


  allProducts: Product[] | null = [];
  query: string = '';
  filteredProducts: Product[] = [];
  routeParamMap$: Subscription | null = null;


  constructor(private productService: ProductService, private activatedRoute: ActivatedRoute) { }

  async ngOnInit(): Promise<void> {
    this.routeParamMap$ = this.activatedRoute.paramMap.subscribe(async paramMap => {
        const category = paramMap.get('category') as unknown as string;
        switch(category)
        {
          case "frutas":
            const fruits = await this.productService.getAllFruits();
            this.allProducts = fruits.data
            break;
          case "verduras":
            const vegatables = await this.productService.getAllVegetables();
            this.allProducts = vegatables.data
            break;
          case "carnes":
            const meats = await this.productService.getAllMeats();
            this.allProducts = meats.data
            break;
        }
    });
  }

  /*async getProducts() {
    const request = await this.productService.getAllProducts();

    if (request.success) {
      this.allProducts = request.data;
    }

    console.log(this.allProducts); //para pruebas
  }*/

    ngOnDestroy(): void 
    {
      this.routeParamMap$?.unsubscribe();
    }
}
