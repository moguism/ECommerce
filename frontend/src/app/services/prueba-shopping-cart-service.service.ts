import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Order } from '../models/order';
import { Result } from '../models/result';
import { OrderDto } from '../models/order-dto';

@Injectable({
  providedIn: 'root'
})
export class PruebaShoppingCartServiceService {

  constructor(private api : ApiService) { }

  async getShoppingCart(): Promise<Result<Order>>
  {
    return this.api.get<Order>("Order/shopping-cart", {}, 'json');
  }

  async updateShoppingCart(order: OrderDto): Promise<Result<OrderDto>>
  {
    return this.api.post("Order", order);
  }
}
