import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Product } from '../models/product';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ShoppingCartService {

  total: number = 0
  contProduct : number = 0
  shoppingCartProducts: Product[] = []



  constructor(private apiService: ApiService) { }


  getLocalStorageCart() {
    //this.shoppingCartProducts = [];
    const productsRaw = localStorage.getItem("shoppingCart");
    if (productsRaw) {
      this.shoppingCartProducts = JSON.parse(productsRaw);
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
        for (const product of cartContent) {
          let p = product.product
          p.total = product.quantity
          p = this.addCorrectPath(p)
          this.shoppingCartProducts.push(p);
        }
      }
      console.log("CARRITO SINCRONIZADO: ", this.shoppingCartProducts);

      
    }
    else {
      this.getLocalStorageCart();
    }

    
    this.contProduct = this.shoppingCartProducts.length

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


  deleteFromArray(product: Product, showAlert: boolean) {
    const index = this.shoppingCartProducts.findIndex(p => p.id === product.id);
    this.shoppingCartProducts.splice(index, 1);
    localStorage.setItem("shoppingCart", JSON.stringify(this.shoppingCartProducts))
    if (showAlert) {
      alert("Uno o varios productos han sido eliminados por falta de stock")
    }
  }




  async getShoppingCartCount()
  {
    // Si quisiésemos podríamos recorrer el Array e ir sumando las cantidades con un for, ya como veáis
    if(this.apiService.jwt != "" && this.apiService.jwt != null)
      {
        const result = await this.apiService.get("ShoppingCart", {}, 'json');
        if(result.data)
        {
          const data: any = result.data;
          console.log("DATA MONDONGO: ", data)
          this.total = data.cartContent.length
        }
      }
      else
      {
        const cart = localStorage.getItem("shoppingCart")
        if(cart)
        {
          const cartObject = JSON.parse(cart)
          console.log("CART OBJECT: ", cartObject)
          this.total = cartObject.length
        }
      }
  }

  addCorrectPath(product : Product)
  {
    product.image = environment.imageRouteBasic + product.image 
    return product
  }
}
