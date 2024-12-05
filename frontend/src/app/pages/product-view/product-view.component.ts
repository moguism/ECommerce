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
import { CommonModule } from '@angular/common';
import { ShoppingCart } from '../../models/shopping-cart';


// Pipe Import
import { CorrectDatePipe } from '../../pipes/correct-date.pipe';
import { ShoppingCartService } from '../../services/shopping-cart.service';

@Component({
  selector: 'app-product-view',
  standalone: true,
  imports: [HeaderComponent, FormsModule, CommonModule, CorrectDatePipe],
  //providers: [{provide: LOCALE_ID, useValue: 'es'}],
  templateUrl: './product-view.component.html',
  styleUrl: './product-view.component.css'
})
export class ProductViewComponent implements OnInit {

  protected count = 1;
  product: Product | null = null;
  routeParamMap$: Subscription | null = null;
  //prductReviews: Review[] = []
  shoppingCart: ShoppingCart | null = null;
  div_text: String = "Prueba";

  constructor(private productService: ProductService, private activatedRoute: ActivatedRoute, private apiService: ApiService, private reviewService: ReviewService, private shoppingCartService: ShoppingCartService) { }

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

  isLogged() {
    let boolean = false;
    if (this.apiService.jwt != null && this.apiService.jwt != "") {
      boolean = true
    }
    return boolean
  }

  async addReview() {
    const reviewTextElement = document.getElementById("review-text") as HTMLTextAreaElement | null; //Elemento del textArea

    if (reviewTextElement == null || reviewTextElement?.value.trim() === "" || this.product == null) {
      var alert_div = document.getElementById("alert-div");
      this.div_text = "No has hecho ningun comentario";
      alert_div?.classList.remove("alert-div-none");
      alert_div?.classList.add("alert-div");
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
    if (this.product) {
      if (this.count + 1 <= this.product?.stock) {
        this.count++;
      }
    }
  }
  restar() {
    if (this.count > 0) {
      this.count--
    }
  }


  async addToCart(product: Product) {
    if (this.count <= 0) {
      var alert_div = document.getElementById("alert-div");
      this.div_text = "Candidad no válida";
      alert_div?.classList.remove("alert-div-none");
      alert_div?.classList.add("alert-div");
      return
    }

    if (this.apiService.jwt == "") {
      let allProducts: Product[] = []
      const productsLocalStore = localStorage.getItem("shoppingCart")
      if (productsLocalStore) {
        allProducts = JSON.parse(productsLocalStore)
        const index = allProducts.findIndex(p => p.id === product.id);
        let newProduct = product
        if (index != -1) {
          newProduct = allProducts[index]
          newProduct.total += this.count
        }
        else {
          newProduct.total = 1
          allProducts.push(product)
        }
      }
      else {
        product.total = this.count
        allProducts.push(product)
      }
      localStorage.setItem("shoppingCart", JSON.stringify(allProducts))

      //Contador en local storage del número de productos en el carrito
      this.shoppingCartService.contProduct = allProducts.length

    }
    else {
      localStorage.removeItem("shoppingCart")
      const cartContent = new CartContent(product.id, this.count, product)
      // Envía el objeto `cartContent` directamente, sin envolverlo en un objeto con clave `cartContent`
      const result = await this.apiService.post<ShoppingCart>("ShoppingCart/addProductOrChangeQuantity", cartContent, { "add" : true })

      if(result.data){
        const dataRaw : any =  result.data
        this.shoppingCart = JSON.parse(dataRaw)
      }

      //Contador en local storage del número de productos en el carrito
      if (this.shoppingCart?.cartContent) {
        var cont = this.shoppingCart?.cartContent.length
        console.log("Carrito " + cont)
        this.shoppingCartService.contProduct = this.shoppingCart?.cartContent.length
      }
    }
    var alert_div = document.getElementById("alert-div");
    this.div_text = "Producto añadido al carrito correctamente";
    alert_div?.classList.remove("alert-div-none");
    alert_div?.classList.add("alert-div");
    
    //this.shoppingCartService.getShoppingCartCount()
    
  }

  close_alert(){
    var alert_div = document.getElementById("alert-div");
    alert_div?.classList.remove("alert-div");
    alert_div?.classList.add("alert-div-none");
  }

  ngOnDestroy(): void {
    this.routeParamMap$?.unsubscribe();

  }
}
