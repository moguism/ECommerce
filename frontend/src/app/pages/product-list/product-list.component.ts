import { Component, OnDestroy, OnInit, Query } from '@angular/core';
import { SearchBarComponent } from '../../components/search-bar/search-bar.component';
import { HeaderComponent } from '../../components/header/header.component';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product.service';
import { Subscription, catchError, forkJoin, lastValueFrom } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { ProductType } from '../../models/enums/product-type';
import { OrdinationType } from '../../models/enums/ordination-type';
import { OrdinationDirection } from '../../models/enums/ordination-direction';
import { QuerySelector } from '../../models/query-selector';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [HeaderComponent, SearchBarComponent],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.css'
})
export class ProductListComponent implements OnInit, OnDestroy {
  allProducts: Product[] | null = [];
  filteredProducts: Product[] = [];
  routeParamMap$: Subscription | null = null;

  querySelector: QuerySelector;



  constructor(private productService: ProductService, private activatedRoute: ActivatedRoute) {
    //QuerySelector por defecto para pruebas
    this.querySelector = new QuerySelector(ProductType.FRUITS, OrdinationType.NAME, OrdinationDirection.ASC, 1,4,2);
  }

  async ngOnInit(): Promise<void> {
    
    this.getAllProducts()

  }



  getAllProducts(){


    const currentPageElement = document.getElementById("pagination-numbers");
    const productsPerPageElement = document.getElementById("products-per-page");

    
    //Obtiene la página actual
    if(currentPageElement != null){
      this.querySelector.actualPage = parseInt(currentPageElement.innerText, 10); // Convertir texto a número
    }
    //Obtiene el número de productos que el usuario quiere
    if(productsPerPageElement != null){
      this.querySelector.actualPage = parseInt(productsPerPageElement.innerText, 10); // Convertir texto a número
    }
    

    this.routeParamMap$ = this.activatedRoute.paramMap.subscribe(async paramMap => {
      const category = paramMap.get('category') as unknown as string;
      switch (category) {
        case "frutas":
          this.querySelector.productType = ProductType.FRUITS;
          const fruits = await this.productService.getAllProducts(this.querySelector);
          console.log("fruits",fruits)
          this.allProducts = fruits.data;
          console.log(this.allProducts);
          break;
        case "verduras":
          this.querySelector.productType = ProductType.VEGETABLES;
          const vegatables = await this.productService.getAllProducts(this.querySelector);
          this.allProducts = vegatables.data
          break;
        case "carnes":
          this.querySelector.productType = ProductType.MEAT;
          const meats = await this.productService.getAllProducts(this.querySelector);
          this.allProducts = meats.data
          break;
      }
    });


    console.log('Query Selector:', this.querySelector);
    console.log("All products ",this.allProducts);
  }



  nextPage(){
    const currentPageElement = document.getElementById("pagination-numbers");
    //Obtiene la página actual
    if(currentPageElement != null){
      const actualPage = parseInt(currentPageElement.innerText, 10) +1; // Convertir texto a número
      currentPageElement.innerText = actualPage.toString(); // Actualizar el texto en el DOM
    }

  }



  getSearchedProducts(products: Product[] | null) {
    this.allProducts = products;
  }

  /*async getProducts() {
    const request = await this.productService.getAllProducts();

    if (request.success) {
      this.allProducts = request.data;
    }

    console.log(this.allProducts); //para pruebas
  }*/

  ngOnDestroy(): void {
    this.routeParamMap$?.unsubscribe();
  }
}
