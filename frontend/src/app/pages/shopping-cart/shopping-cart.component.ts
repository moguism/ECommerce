import { Component, OnInit } from '@angular/core';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product.service';
import { ApiService } from '../../services/api.service';
import { CartContent } from '../../models/cart-content';
import { HeaderShopComponent } from '../../components/header-shop/header-shop.component';
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
  shoppingCartProducts: Product[] = []
  productsToBuy: CartContent[] = [];


  constructor(private productService: ProductService, private apiService: ApiService, private router: Router, private shoppingCartService: ShoppingCartService) { }

  async ngOnInit(): Promise<void> {
    const goToCheckout = localStorage.getItem("goToCheckout")
    if (this.apiService.jwt != "" && goToCheckout && goToCheckout == "true") {
      await this.createDirectPayment();
      return
    }
    else {
      this.getShoppingCart();
    }
  }

  getLocalStorageCart() {
    //this.shoppingCartProducts = [];
    const productsRaw = localStorage.getItem("shoppingCart");
    if (productsRaw) {
      this.shoppingCartProducts = JSON.parse(productsRaw);
    }
  }



  async getShoppingCart() {
    //this.shoppingCartService.getShoppingCartCount()
    this.shoppingCartProducts = [];

    /*if (this.apiService.jwt !== "" && this.shoppingCartProducts.length > 0) {
      console.log("Sincronizando productos locales al carrito del backend...");

      for (const product of this.shoppingCartProducts) {
        const cartContent = new CartContent(product.id, product.total);
        await this.apiService.post("ShoppingCart/addProductOrChangeQuantity", cartContent);
      }

      localStorage.removeItem("shoppingCart");
      
    }*/

    // Podríamos optimizar esto haciendo que en el login ponga el contenido del carrito en el localStorage, de manera que no tenga que hacer peticiones
    // Sin embargo, si un admin borra o cambia un producto, cagamos
    if (this.apiService.jwt !== "") {
      const result = await this.apiService.get("ShoppingCart", {}, 'json');
      if (result.data) {
        const data: any = result.data;
        const cartContent: any[] = data.cartContent;
        for (const product of cartContent) {
          let p = product.product
          p.total = product.quantity
          p = this.shoppingCartService.addCorrectPath(p)
          this.shoppingCartProducts.push(p);
        }
      }
      console.log("CARRITO SINCRONIZADO: ", this.shoppingCartProducts);

      
    }
    else {
      this.getLocalStorageCart();
    }

    
    this.shoppingCartService.contProduct = this.shoppingCartProducts.length

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
        const p = this.findProductInArray(product.id)
        p.total = parseInt(input.value);
        localStorage.setItem("shoppingCart", JSON.stringify(this.shoppingCartProducts));
      }
      else {
        localStorage.removeItem("shoppingCart")

        const cartContent = new CartContent(product.id, parseInt(input.value), product)
        this.getShoppingCart()
      }
    }
  }

  async deleteProduct(productId: number) {
    const index = this.shoppingCartProducts.findIndex(product => product.id === productId);

    if (index !== -1) {
      const product = this.shoppingCartProducts[index];

      if (product.total > 1) {
        product.total -= 1;
      } else {
        this.shoppingCartProducts.splice(index, 1);
      }

      if (this.apiService.jwt == "") {
        this.deleteFromArray(product, false)
      }
      else {
        await this.apiService.delete("ShoppingCart", { productId })
        this.getShoppingCart()
      }
    }

  }

  findProductInArray(id: number): Product {
    const index = this.shoppingCartProducts.findIndex(product => product.id === id);
    const product = this.shoppingCartProducts[index];
    return product;
  }

  async pay(method: string) {


    if (this.shoppingCartProducts.length == 0) {
      alert("No hay nada que pagar, bobolón")
      return
    }
    for (const product of this.shoppingCartProducts) {
      const newProduct = await this.productService.getById(product.id)
      if (newProduct) {
        const difference = newProduct.stock - product.stock;
        if (difference < 0) {
          this.deleteFromArray(product, true)
          return
        }
      }
      else {
        this.deleteFromArray(product, true)
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
      localStorage.removeItem("goToCheckout")
    }
    else {
      localStorage.setItem("goToCheckout", "true")
      this.router.navigateByUrl("login")
    }
  }

  // Esta función solo debería servir para el carrito local
  async createDirectPayment() {
    this.getLocalStorageCart()
    const cartContents: CartContent[] = []
    for (const product of this.shoppingCartProducts) {
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

  deleteFromArray(product: Product, showAlert: boolean) {
    const index = this.shoppingCartProducts.findIndex(p => p.id === product.id);
    this.shoppingCartProducts.splice(index, 1);
    localStorage.setItem("shoppingCart", JSON.stringify(this.shoppingCartProducts))
    if (showAlert) {
      alert("Uno o varios productos han sido eliminados por falta de stock")
    }
  }

  totalprice() {
    let totalcount = 0;
    for (const product of this.shoppingCartProducts) {
      totalcount += product.total * product.price;
    }
    return totalcount;
  }
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





}