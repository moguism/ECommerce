import { Component, OnInit } from '@angular/core';
import { PruebaShoppingCartServiceService } from '../../services/prueba-shopping-cart-service.service';
import { Order } from '../../models/order';

@Component({
  selector: 'app-prueba-shopping-cart',
  standalone: true,
  imports: [],
  templateUrl: './prueba-shopping-cart.component.html',
  styleUrl: './prueba-shopping-cart.component.css'
})
export class PruebaShoppingCartComponent implements OnInit {
  shoppingCart : Order | null = null
   
  constructor(private shoppingCartService: PruebaShoppingCartServiceService){}

  async ngOnInit(): Promise<void> {
    await this.getShoppingCart();
  }
  
  async getShoppingCart()
  {
    const result = await this.shoppingCartService.getShoppingCart();
    this.shoppingCart = result.data;
  }
}
