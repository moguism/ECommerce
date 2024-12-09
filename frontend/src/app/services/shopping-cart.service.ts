import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Product } from '../models/product';
import { environment } from '../../environments/environment';
import { CartContent } from '../models/cart-content';
import Swal from 'sweetalert2';

@Injectable({
  providedIn: 'root'
})
export class ShoppingCartService {
  shoppingCartProducts: Product[] = []
  productsToBuy: CartContent[] = [];

  contProduct = 0

  constructor(private apiService: ApiService) { }


  getLocalStorageCart() {
    //this.shoppingCartProducts = [];
    const productsRaw = localStorage.getItem("shoppingCart");
    console.log(productsRaw)
    if (productsRaw) {
      this.shoppingCartProducts = JSON.parse(productsRaw);
      this.contProduct = this.shoppingCartProducts.length
    }

  }

  async syncronizeCart(add: boolean) {
    const productsRaw = localStorage.getItem("shoppingCart");
    let products: Product[] = []
    if (productsRaw == null) {
      return
    }
    products = JSON.parse(productsRaw);

    if (this.apiService.jwt !== "" && products.length > 0) {
      console.log("Sincronizando productos locales al carrito del backend...");

      await this.uploadCart(products, add)

      localStorage.removeItem("shoppingCart");
    }
  }


  async getShoppingCart() {

    this.shoppingCartProducts = [];



    // Podríamos optimizar esto haciendo que en el login ponga el contenido del carrito en el localStorage, de manera que no tenga que hacer peticiones
    // Sin embargo, si un admin borra o cambia un producto, cagamos
    if (this.apiService.jwt !== "") {
      const result = await this.apiService.get("ShoppingCart", {}, 'json');
      if (result.data) {
        const data: any = result.data;
        const cartContent: any[] = data.cartContent;
        console.log("CART CONTEEEEEEEEEEEEEEENT", cartContent)
        for (const product of cartContent) {
          let p: Product = product.product
          console.log("PRODUCTOOOOOOOOOOOOO", p)
          if (p.stock <= 0) {
            await this.deleteFromArray(p)
            this.mostraralert()
            continue
          }
          p.total = product.quantity
          p = this.addCorrectPath(p)
          this.shoppingCartProducts.push(p);
        }
      }
      console.log("CARRITO SINCRONIZADO: ", this.shoppingCartProducts);

      this.contProduct = this.shoppingCartProducts.length
    }
    else {
      this.getLocalStorageCart();
    }
  }

  async uploadCart(products: Product[], add: boolean) {
    var cart: CartContent[] = []

    products.forEach(product => {
      cart.push(new CartContent(product.id, product.total, product))
    });

    const result = await this.apiService.post("ShoppingCart/save", cart, { "add": add })
    if (result.data) {
      const data: any = result.data
      this.contProduct = data
    }
  }

  findProductInArray(id: number): Product {
    const index = this.shoppingCartProducts.findIndex(product => product.id === id);
    const product = this.shoppingCartProducts[index];
    return product;
  }


  async deleteFromArray(product: Product) {
    console.log("BORRANDO ESTE PRODUCTO: ", product.id)
    if (this.apiService.jwt != "") {
      const id = product.id
      await this.apiService.delete("ShoppingCart", { "productId" : id })
    }
    const index = this.shoppingCartProducts.findIndex(p => p.id === product.id);
    if (index != -1) {
      this.shoppingCartProducts.splice(index, 1);
      localStorage.setItem("shoppingCart", JSON.stringify(this.shoppingCartProducts))
    }
    this.contProduct -= 1
  }

  mostraralert() {
    Swal.fire({
      title: 'Ha ocurrido un error',
      text: 'Ya no hay stock del producto',
      icon: 'error',
      confirmButtonText: 'Salir'
    })
  }

  /*async getShoppingCartCount() {
    // Si quisiésemos podríamos recorrer el Array e ir sumando las cantidades con un for, ya como veáis
    if (this.apiService.jwt != "" && this.apiService.jwt != null) {
      const result = await this.apiService.get("ShoppingCart", {}, 'json');
      if (result.data) {
        const data: any = result.data;
        console.log("DATA MONDONGO: ", data)
      }
    }
    else {
      const cart = localStorage.getItem("shoppingCart")
      if (cart) {
        const cartObject = JSON.parse(cart)
        console.log("CART OBJECT: ", cartObject)
      }
    }
  }*/

  addCorrectPath(product: Product) {
    product.image = environment.imageRouteBasic + product.image
    return product
  }
}
