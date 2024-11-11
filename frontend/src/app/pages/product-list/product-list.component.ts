import { Component, OnDestroy, OnInit } from '@angular/core';
import { SearchBarComponent } from '../../components/search-bar/search-bar.component';
import { HeaderComponent } from '../../components/header/header.component';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product.service';
import { Subscription } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductType } from '../../models/enums/product-type';
import { OrdinationType } from '../../models/enums/ordination-type';
import { OrdinationDirection } from '../../models/enums/ordination-direction';
import { QuerySelector } from '../../models/query-selector';
import { EurosToCentsPipe } from '../../pipes/euros-to-cents.pipe';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [HeaderComponent, SearchBarComponent, EurosToCentsPipe],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.css'
})
export class ProductListComponent implements OnInit, OnDestroy {
  allProducts: Product[] | null | undefined = [];
  filteredProducts: Product[] = [];
  routeParamMap$: Subscription | null = null;

  querySelector: QuerySelector;
  productTypeString: string = 'Producto';

  BtnPerName: boolean = true;
  BtnPerPrice: boolean = true;

  totalProducts: number = 0;
  totalPages: number = 1;
  currentPage: number = 1;

  constructor(
    private productService: ProductService,
    private activatedRoute: ActivatedRoute,
    private router: Router
  ) {
    const FIRST_PAGE = 1;
    const PRODUCT_PER_PAGE = 5;
    this.querySelector = new QuerySelector(
      ProductType.FRUITS,
      OrdinationType.NAME,
      OrdinationDirection.ASC,
      PRODUCT_PER_PAGE,
      FIRST_PAGE,
      ''
    );
  }

  async ngOnInit(): Promise<void> {
    this.restoreSessionData();
    this.getAllProducts();
  }

  getSearchedProducts(query: string) {
    this.querySelector.search = query
    sessionStorage.setItem('query', this.querySelector.search);
    this.getAllProducts();
    this.goToFirstPage();
  }

  restoreSessionData() {
    const savedCategory = sessionStorage.getItem('category');
    if (savedCategory) {
      this.querySelector.productType = parseInt(savedCategory);
      //sessionStorage.removeItem('category');
    }

    const currentPage = sessionStorage.getItem('currentPage');
    if (currentPage) {
      this.currentPage = parseInt(currentPage);
      this.querySelector.actualPage = this.currentPage;
      //sessionStorage.removeItem('currentPage');
    }

    const totalPages = sessionStorage.getItem('totalPages');
    if (totalPages) {
      this.totalPages = parseInt(totalPages);
      //sessionStorage.removeItem('totalPages');
    }

    const productsPerPage = sessionStorage.getItem('productsPerPage');
    const productsPerPageElement = document.getElementById("products-per-page") as HTMLInputElement | HTMLSelectElement;
    if (productsPerPage && productsPerPageElement) {
      this.querySelector.productPageSize = parseInt(productsPerPage);
      productsPerPageElement.value = this.querySelector.productPageSize.toString();
      //sessionStorage.removeItem('productsPerPage');
    }

    const ordinationType = sessionStorage.getItem('ordinationType');
    const ordinationOrder = sessionStorage.getItem('ordinationOrder');
    const orderBy = document.getElementById('order-by') as HTMLInputElement | HTMLSelectElement;
    if (ordinationType && ordinationOrder && orderBy) {
      this.querySelector.ordinationType = parseInt(ordinationType);
      this.querySelector.ordinationDirection = parseInt(ordinationOrder);

      if (this.querySelector.ordinationType == 0 && this.querySelector.ordinationDirection == 0) {
        orderBy.value = "name-asc"
      }
      else if (this.querySelector.ordinationType == 0 && this.querySelector.ordinationDirection == 1) {
        orderBy.value = "name-desc"
      }
      else if (this.querySelector.ordinationType == 1 && this.querySelector.ordinationDirection == 0) {
        orderBy.value = "price-asc"
      }
      else if (this.querySelector.ordinationType == 1 && this.querySelector.ordinationDirection == 1) {
        orderBy.value = "price-desc"
      }

      //sessionStorage.removeItem('ordinationType');
      //sessionStorage.removeItem('ordinationOrder');
    }

    const searchQuery = sessionStorage.getItem('query');
    if (searchQuery) {
      this.querySelector.search = searchQuery;
      //sessionStorage.removeItem('query');
    }
  }

  async getAllProducts() {
    this.routeParamMap$ = this.activatedRoute.paramMap.subscribe(async (paramMap) => {
      const category = paramMap.get('category');
      let type = ProductType.FRUITS
      if (category) {
        switch (category) {
          case 'frutas':
            type = ProductType.FRUITS;
            break;
          case 'verduras':
            type = ProductType.VEGETABLES;
            break;
          case 'carnes':
            type = ProductType.MEAT;
            break;
        }
      }

      if (type != this.querySelector.productType) {
        this.querySelector.productType = type
        this.querySelector.actualPage = 1
        this.currentPage = this.querySelector.actualPage
      }

      console.log(this.querySelector)

      const result = await this.productService.getAllProducts(this.querySelector);
      this.allProducts = result?.products;
      this.totalProducts = result?.totalProducts ?? 0;
      this.totalPages = Math.ceil(this.totalProducts / this.querySelector.productPageSize);

      //this.save = false;

      this.updatePaginationButtons();
    });
  }

  goToProduct(id: number) {
    const route: string = `product-view/${id}`;
    this.router.navigateByUrl(route);
  }

  updatePaginationButtons() {
    const previousButton = document.getElementById('prev-button') as HTMLButtonElement;
    const nextButton = document.getElementById('next-button') as HTMLButtonElement;
    const firstButton = document.getElementById('first-button') as HTMLButtonElement;
    const lastButton = document.getElementById('last-button') as HTMLButtonElement;

    if (firstButton) firstButton.disabled = this.querySelector.actualPage <= 1;
    if (previousButton) previousButton.disabled = this.querySelector.actualPage <= 1;
    if (nextButton) nextButton.disabled = this.querySelector.actualPage >= this.totalPages;
    if (lastButton) lastButton.disabled = this.querySelector.actualPage >= this.totalPages;
  }

  nextPage() {
    if (this.querySelector.actualPage < this.totalPages) {
      this.querySelector.actualPage++;
      this.saveCurrentPage();
      this.getAllProducts();
    }
  }

  previousPage() {
    if (this.querySelector.actualPage > 1) {
      this.querySelector.actualPage--;
      this.saveCurrentPage();
      this.getAllProducts();
    }
  }

  goToFirstPage() {
    if (this.querySelector.actualPage !== 1) {
      this.querySelector.actualPage = 1;
      this.saveCurrentPage();
      this.getAllProducts();
    }
  }

  goToLastPage() {
    if (this.querySelector.actualPage !== this.totalPages) {
      this.querySelector.actualPage = this.totalPages;
      this.saveCurrentPage();
      this.getAllProducts();
    }
  }

  saveCurrentPage()
  {
    this.currentPage = this.querySelector.actualPage;
    sessionStorage.setItem('currentPage', this.currentPage.toString());
  }

  newNumberOfProducts() {
    const productsPerPageElement = document.getElementById("products-per-page") as HTMLInputElement | HTMLSelectElement;
    if (productsPerPageElement) {
      sessionStorage.setItem('productsPerPage', productsPerPageElement.value.toString()); // Por si acaso, que luego falla
      this.querySelector.productPageSize = parseInt(productsPerPageElement.value);
      this.querySelector.actualPage = 1;
      this.currentPage = this.querySelector.actualPage;
      this.getAllProducts();
    }
  }

  order() {
    const orderBy = document.getElementById('order-by') as HTMLInputElement | HTMLSelectElement;
    if (orderBy) {
      this.sortBy(orderBy.value);
      this.getAllProducts();
    }
  }

  sortBy(order: string) {
    switch (order) {
      case 'name-asc':
        this.querySelector.ordinationType = OrdinationType.NAME;
        this.querySelector.ordinationDirection = OrdinationDirection.ASC;
        break;
      case 'name-desc':
        this.querySelector.ordinationType = OrdinationType.NAME;
        this.querySelector.ordinationDirection = OrdinationDirection.DESC;
        break;
      case 'price-asc':
        this.querySelector.ordinationType = OrdinationType.PRICE;
        this.querySelector.ordinationDirection = OrdinationDirection.ASC;
        break;
      case 'price-desc':
        this.querySelector.ordinationType = OrdinationType.PRICE;
        this.querySelector.ordinationDirection = OrdinationDirection.DESC;
        break;
    }

    sessionStorage.setItem('ordinationType', this.querySelector.ordinationType.toString());
    sessionStorage.setItem('ordinationOrder', this.querySelector.ordinationDirection.toString());
    this.goToFirstPage();
  }

  ngOnDestroy(): void {
    this.routeParamMap$?.unsubscribe();
    sessionStorage.setItem('category', this.querySelector.productType.toString());
    //sessionStorage.setItem('currentPage', this.currentPage.toString());
    //sessionStorage.setItem('totalPages', this.totalPages.toString());
    //sessionStorage.setItem('productsPerPage', this.querySelector.productPageSize.toString());
    //sessionStorage.setItem('ordinationType', this.querySelector.ordinationType.toString());
    //sessionStorage.setItem('ordinationOrder', this.querySelector.ordinationDirection.toString());
    //sessionStorage.setItem('query', this.querySelector.search);
  }
}