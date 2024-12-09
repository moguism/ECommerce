import { Component, OnInit } from '@angular/core';
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

@Component({
  selector: 'app-shopping-cart',
  standalone: true,
  imports: [HeaderComponent, FormsModule, EurosToCentsPipe],
  templateUrl: './shopping-cart.component.html',
  styleUrl: './shopping-cart.component.css'
})
export class ShoppingCartComponent implements OnInit {
  productsToBuy: CartContent[] = [];


  constructor(private productService: ProductService, private apiService: ApiService, private router: Router,
    public shoppingCartService: ShoppingCartService) { }

  async ngOnInit(): Promise<void> {
    if(localStorage.getItem("sync"))
    {
      await this.shoppingCartService.syncronizeCart(false)
      localStorage.removeItem("sync")
    }
    const goToCheckout = localStorage.getItem("goToCheckout")
    if (this.apiService.jwt != "" && goToCheckout && goToCheckout == "true") {
      await this.createDirectPayment();
      return
    }
    else {
      this.shoppingCartService.getShoppingCart();
    }

  }

  //Actualiza el contador del producto cuando se actualiza en el componente
  async onCountChange(event: { productId: number, newCount: number }) {
    const { productId, newCount } = event;

    const p = this.shoppingCartService.findProductInArray(productId);
    if (p) {
      p.total = newCount;
    }


  }

  async changeQuantity(product: Product) {
    const input = document.getElementById(product.id.toString()) as HTMLInputElement
    if (input && parseInt(input.value) <= 0) {
      alert("Cantidad no válida")
      return
    }

    if (parseInt(input.value) > product.stock) {
      alert("No hay stock suficiente")
      return
    }

    if (input) {
      if (this.apiService.jwt == "") {
        const p = this.shoppingCartService.findProductInArray(product.id)
        p.total = parseInt(input.value);
        localStorage.setItem("shoppingCart", JSON.stringify(this.shoppingCartService.shoppingCartProducts));
      }
      else {
        localStorage.removeItem("shoppingCart")

        const cartContent = new CartContent(product.id, parseInt(input.value), product)
        await this.apiService.post("ShoppingCart/addProductOrChangeQuantity", cartContent)
        this.shoppingCartService.getShoppingCart()
      }
    }
  }


  async deleteProduct(productId: number) {
    const index = this.shoppingCartService.shoppingCartProducts.findIndex(product => product.id === productId);

    if (index !== -1) {
      this.shoppingCartService.shoppingCartProducts.splice(index, 1);
      localStorage.setItem("shoppingCart", JSON.stringify(this.shoppingCartService.shoppingCartProducts))

      if (this.apiService.jwt != "") {
        await this.apiService.delete("ShoppingCart", { productId })
        this.shoppingCartService.getShoppingCart()
      }
    }
  }

  /*deleteFromArray(product: Product, showAlert: boolean) {
    const index = this.shoppingCartService.shoppingCartProducts.findIndex(p => p.id === product.id);
    console.log("ÌNDICE: ", index)
    this.shoppingCartService.shoppingCartProducts.splice(index, 1);
    localStorage.setItem("shoppingCart", JSON.stringify(this.shoppingCartService.shoppingCartProducts))
    if (showAlert) {
      alert("Uno o varios productos han sido eliminados por falta de stock")
    }
  }*/


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
    return totalcount;
  }

  /*

  sumar(index: number) {
    const quantity = this.shoppingCartProducts.findIndex(product => product.id === index);
    this.shoppingCartProducts[quantity].total++;
  }
  restar(index: number) {
    const quantity = this.shoppingCartProducts.findIndex(product => product.id === index);
    if (this.shoppingCartProducts[quantity].total > 0) {
      this.shoppingCartProducts[quantity].total--;
    }
  }
    */

}