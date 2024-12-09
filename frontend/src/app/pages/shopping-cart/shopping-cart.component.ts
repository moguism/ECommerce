import { Component, HostListener, OnDestroy, OnInit } from '@angular/core';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product.service';
import { ApiService } from '../../services/api.service';
import { CartContent } from '../../models/cart-content';
import { FormsModule } from '@angular/forms';
import { EurosToCentsPipe } from '../../pipes/euros-to-cents.pipe';
import { Router } from '@angular/router';
import { TemporalOrder } from '../../models/temporal-order';
import { HeaderComponent } from '../../components/header/header.component';
import { ShoppingCartService } from '../../services/shopping-cart.service';
import { QuantityModifierComponent } from '../../components/quantity-modifier/quantity-modifier.component';

@Component({
  selector: 'app-shopping-cart',
  standalone: true,
  imports: [HeaderComponent, FormsModule, EurosToCentsPipe, QuantityModifierComponent],
  templateUrl: './shopping-cart.component.html',
  styleUrl: './shopping-cart.component.css'
})
export class ShoppingCartComponent implements OnInit, OnDestroy {
  productsToBuy: CartContent[] = [];


  constructor(private productService: ProductService, private apiService: ApiService, private router: Router,
    public shoppingCartService: ShoppingCartService) { }

  async ngOnInit(): Promise<void> {
    
    const goToCheckout = localStorage.getItem("goToCheckout")
    if (this.apiService.jwt != "" && goToCheckout && goToCheckout == "true") {
      await this.createDirectPayment();
      return
    }
    else {
      this.shoppingCartService.getShoppingCart();
    }

  }

  async ngOnDestroy(): Promise<void> {
    await this.shoppingCartService.saveShoppingCart()
  }

  //Actualiza el contador del producto cuando se actualiza en el componente
  async onCountChange(event: { productId: number, newCount: number }) {
    console.log(this.shoppingCartService.isSaved)
    const { productId, newCount } = event;

    const p = this.shoppingCartService.findProductInArray(productId);
    if (p) {
      p.total = newCount;
    }

    this.shoppingCartService.isSaved = false
    console.log("no guardado")

  }

  async pay(method: string) {


    if (this.shoppingCartService.shoppingCartProducts.length == 0) {
      alert("No hay nada que pagar, bobolón")
      return
    }
    for (const product of this.shoppingCartService.shoppingCartProducts) {
      const newProduct = await this.productService.getById(product.id)
      if (newProduct) {
        const difference = newProduct.stock - product.stock;
        if (difference < 0) {
          this.shoppingCartService.deleteFromArray(product, true)
          return
        }
      }
      else {
        this.shoppingCartService.deleteFromArray(product, true)
        return
      }

      //Añade productos a lista de los productos que el usuario quiere comprar
      const orderProduct: CartContent = {
        ProductId: product.id,
        Quantity: product.total ?? product.stock,
        Product: product // Usar `product.stock` si `product.total` no existe
      };

      this.productsToBuy.push(orderProduct)

    }

    localStorage.setItem("method", method)
    if (this.apiService.jwt != "") {
      const result = await this.apiService.post("TemporalOrder/newTemporalOrder", new TemporalOrder(this.productsToBuy, false))
      console.log("ORDEN TEMPORAL: ", result)
      this.goToCheckout(result)
      localStorage.setItem("goToCheckout", "false")
    }
    else {
      localStorage.setItem("goToCheckout", "true")
      this.router.navigateByUrl("login")
    }
  }

  // Esta función solo debería servir para el carrito local
  async createDirectPayment() {
    this.shoppingCartService.getLocalStorageCart()
    const cartContents: CartContent[] = []
    for (const product of this.shoppingCartService.shoppingCartProducts) {
      const cartContent = new CartContent(product.id, product.total, product)
      cartContents.push(cartContent)
    }
    const result = await this.apiService.post("TemporalOrder/newTemporalOrder", new TemporalOrder(cartContents, true))
    console.log("PAGO DIRECTO: ", result)
    localStorage.removeItem("shoppingCart")
    this.goToCheckout(result)
  }

  goToCheckout(result: any) {
    if (result.data) {
      const data = JSON.parse(result.data)
      const url: string = "checkout/" + localStorage.getItem("method") + "/" + data.id
      this.router.navigateByUrl(url)
      localStorage.setItem("temporal", JSON.stringify(data))
    }
    else {
      alert("Ha ocurrido un error")
    }
  }


  totalprice() {
    let totalcount = 0;
    for (const product of this.shoppingCartService.shoppingCartProducts) {
      totalcount += product.total * product.price;
    }
    if(totalcount<5000 && totalcount>0){
      return totalcount+300;
    }else{
      return totalcount;
    }
  }


}