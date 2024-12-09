import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
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
  @Output() countChange = new EventEmitter<{ productId: number; newCount: number }>();

  product: Product | null = null;

  constructor(private productService : ProductService) { }


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
        this.countChange.emit({ productId: this.productId, newCount: this.count });
      }
    }
  }

  restar() {
    if (this.count > 1) {
      this.count--
      this.countChange.emit({ productId: this.productId, newCount: this.count });
    }
  }



}
