import { Injectable } from '@angular/core';
import { ApiService } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class ShoppingCartService {

  total: number = 0

  constructor(private api: ApiService) { }

  async getShoppingCartCount()
  {
    // Si quisiésemos podríamos recorrer el Array e ir sumando las cantidades con un for, ya como veáis
    if(this.api.jwt != "" && this.api.jwt != null)
      {
        const result = await this.api.get("ShoppingCart", {}, 'json');
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
}
