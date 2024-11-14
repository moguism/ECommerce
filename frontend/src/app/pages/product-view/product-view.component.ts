import { Component, LOCALE_ID, OnInit } from '@angular/core';
import { HeaderComponent } from '../../components/header/header.component';
import { ActivatedRoute } from '@angular/router';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product.service';
import { ApiService } from '../../services/api.service';
import { CartContent } from '../../models/cart-content';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { NewReview } from '../../models/new-review';
import { ReviewService } from '../../services/review.service';
import { CommonModule} from '@angular/common';

// Date Import
import locale from '@angular/common/locales/es'
import { registerLocaleData } from '@angular/common';
import { CorrectDatePipe } from '../../pipes/correct-date.pipe';
registerLocaleData(locale, 'es');

@Component({
  selector: 'app-product-view',
  standalone: true,
  imports: [HeaderComponent, FormsModule, CommonModule, CorrectDatePipe],
  //providers: [{provide: LOCALE_ID, useValue: 'es'}],
  templateUrl: './product-view.component.html',
  styleUrl: './product-view.component.css'
})
export class ProductViewComponent implements OnInit {

  protected count = 0;
  product: Product | null = null;
  routeParamMap$: Subscription | null = null;
  //prductReviews: Review[] = []

  constructor(private productService: ProductService, private activatedRoute: ActivatedRoute, private apiService: ApiService, private reviewService: ReviewService) { }

  ngOnInit(): void {
    //const id = this.activatedRoute.snapshot.paramMap.get('id') as unknown as number;
    this.getProduct()
    //this.makeReviews()
  }

  async getProduct() {

    this.routeParamMap$ = this.activatedRoute.paramMap.subscribe(async paramMap => {
      const id = paramMap.get('id') as unknown as number;
      const result = await this.productService.getById(id)
      if (result != null) {
        this.product = result
        console.log("PRODUCTO: ", this.product)
      }
    });

  }

  async addReview() {
    const reviewTextElement = document.getElementById("review-text") as HTMLTextAreaElement | null; //Elemento del textArea

    if (reviewTextElement == null || reviewTextElement?.value.trim() === "" || this.product == null) {
      alert("No has hecho ningun comentario");
    } else {
      const newReview = new NewReview(reviewTextElement.value, this.product.id, new Date().toISOString());

      console.log(newReview)

      await this.reviewService.addReview(newReview); 

      if (reviewTextElement) {
        reviewTextElement.value = "";
      }

      this.getProduct();
    }

  }

  sumar() {
    if(this.product)
    {
      if(this.count + 1 <= this.product?.stock)
      {
        this.count++;
      }
    }
  }
  restar() {
    if (this.count > 0) {
      this.count--
    }
  }


  async addToCart(product: Product)
  {
    if(this.count <= 0)
    {
      alert("Cantidad no válida")
      return
    }
    if(this.apiService.jwt == "")
    {
        let allProducts : Product[] = []
        const productsLocalStore = localStorage.getItem("shoppingCart")
        if(productsLocalStore)
        {
          allProducts = JSON.parse(productsLocalStore)
          const index = allProducts.findIndex(p => p.id === product.id);
          let newProduct = product
          if(index != -1)
          {
            newProduct = allProducts[index]
            newProduct.total += this.count
          }
          else
          {
            newProduct.total = 1
          }
        }
        else
        {
          product.total = this.count
          allProducts.push(product)
        }
        localStorage.setItem("shoppingCart", JSON.stringify(allProducts))

    }
    else {
      localStorage.removeItem("shoppingCart")
      const cartContent = new CartContent(this.count, product.id) // 0 no es para borrar, sino para agregar uno nuevo
      await this.apiService.post("ShoppingCart/add", cartContent)
    }
    alert("Producto añadido al carrito correctamente")
  }

  ngOnDestroy(): void {
    this.routeParamMap$?.unsubscribe();

  }
}
