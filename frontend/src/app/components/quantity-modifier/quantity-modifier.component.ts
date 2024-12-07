import { Component, Input, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { Router } from '@angular/router';
import { ShoppingCartService } from '../../services/shopping-cart.service';
import { UserService } from '../../services/user.service';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product.service';

@Component({
  selector: 'app-quantity-modifier',
  standalone: true,
  imports: [],
  templateUrl: './quantity-modifier.component.html',
  styleUrl: './quantity-modifier.component.css'
})
export class QuantityModifierComponent implements OnInit {

  @Input() productId!: number;
  @Input() count!: number;

  product: Product | null = null;

  constructor(private apiService: ApiService, private router: Router,private productService : ProductService,
    public shoppingCartService: ShoppingCartService, public userService: UserService) { }


  ngOnInit(): void {
    this.getProduct()
  }

  async getProduct() {

    const result = await this.productService.getById(this.productId)
      if (result != null) {
        this.product = result
        console.log("PRODUCTO: ", this.product)
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
    if (this.count > 1) {
      this.count--
    }
  }



}
