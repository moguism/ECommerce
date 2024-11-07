import { Component, OnInit } from '@angular/core';
import { HeaderComponent } from '../../components/header/header.component';
import { ActivatedRoute } from '@angular/router';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product.service';
import { ApiService } from '../../services/api.service';
import { CartContent } from '../../models/cart-content';
import { FormsModule } from '@angular/forms';
@Component({
  selector: 'app-product-view',
  standalone: true,
  imports: [HeaderComponent, FormsModule],
  templateUrl: './product-view.component.html',
  styleUrl: './product-view.component.css'
})
export class ProductViewComponent implements OnInit {

  protected count=0;
  product: Product | null = null
  constructor(private productService: ProductService, private activatedRoute: ActivatedRoute, private apiService: ApiService){}

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

  async addToCart(product: Product)
  {
    if(this.apiService.jwt == "")
    {
        let allProducts : Product[] = []
        const productsLocalStore = localStorage.getItem("shoppingCart")
        if(productsLocalStore)
        {
          allProducts = JSON.parse(productsLocalStore)
          allProducts.push(product)
          localStorage.setItem("shoppingCart", JSON.stringify(allProducts))
        }
    }
    else
    {
      localStorage.removeItem("shoppingCart")
      const cartContent = new CartContent(1, product.id)
      await this.apiService.post("ShoppingCart", cartContent)
    }
  }

}
