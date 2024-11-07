import { Component, OnInit } from '@angular/core';
import { HeaderComponent } from '../../components/header/header.component';
import { ActivatedRoute } from '@angular/router';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product.service';
import { ShoppingCartComponent } from '../shopping-cart/shopping-cart.component';

@Component({
  selector: 'app-product-view',
  standalone: true,
  imports: [HeaderComponent,ShoppingCartComponent],
  templateUrl: './product-view.component.html',
  styleUrl: './product-view.component.css'
})
export class ProductViewComponent implements OnInit {

  protected count=0;
  product: Product | null = null
  constructor(private productService: ProductService, private activatedRoute: ActivatedRoute){}

  ngOnInit(): void
  {
    const id = this.activatedRoute.snapshot.paramMap.get('id') as unknown as number;
    this.getProduct(id)
  }

  async getProduct(id: number)
  {
    const result = await this.productService.getById(id)
    if(result != null)
    {
      this.product = result
    }
  }
  sumar(){
    this.count++;
  }
  restar(){
    if(this.count>0){
      this.count--
    }
  }
}
