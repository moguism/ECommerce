import { Component, OnInit } from '@angular/core';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product.service';
import { ApiService } from '../../services/api.service';
import { HeaderShopComponent } from '../../components/header-shop/header-shop.component';
import { EurosToCentsPipe } from '../../pipes/euros-to-cents.pipe';
import { ActivatedRoute, Router } from '@angular/router';
import { HeaderComponent } from '../../components/header/header.component';
import { interval, Subscription } from 'rxjs';

@Component({
  selector: 'app-shopping-cart',
  standalone: true,
  imports: [HeaderShopComponent, EurosToCentsPipe, HeaderComponent],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.css'
})
export class CheckoutComponent implements OnInit {
  shoppingCartProducts: Product[] = []
  autoRefreshSubscription: Subscription | undefined;
  private id: number = 0
  private method: string = ""

  constructor(private productService: ProductService, private apiService: ApiService, private router: Router, private activatedRoute: ActivatedRoute) {
  }

  async ngOnInit(): Promise<void> {
    this.id = this.activatedRoute.snapshot.paramMap.get('id') as unknown as number;
    this.method = this.activatedRoute.snapshot.paramMap.get('method') as unknown as string;
    const result = await this.apiService.get("TemporalOrder", { "id" : this.id })
    console.log("RESULT CHECKOUT: ", result)

    const shoppinCartResult = await this.apiService.get("ShoppingCart", { "isTemporal": true }, 'json');
    if (shoppinCartResult.data) {
      const data: any = shoppinCartResult.data;
      const cartContent: any[] = data.cartContent;
      for (const product of cartContent) {
        const productResult = await this.productService.getById(product.productId);
        if (productResult != null) {
          const p: Product = {
            id: productResult.id,
            name: productResult.name,
            average: productResult.average,
            category: productResult.category,
            categoryId: productResult.categoryId,
            description: productResult.description,
            image: productResult.image,
            price: productResult.price,
            reviews: productResult.reviews,
            stock: productResult.stock,
            total: product.quantity
          };
          this.shoppingCartProducts.push(p);
        }
      }
    }

    this.autoRefreshSubscription = this.startAutoRefresh();
  }

  totalprice() {
    let totalcount = 0;
    for (const product of this.shoppingCartProducts) {
      totalcount += product.total * product.price;
    }
    return totalcount;
  }

  startAutoRefresh() {
    // 300000 milisegundos son 5 minutos
    return interval(300000).subscribe(() => {this.refreshOrder()});
  }

  async refreshOrder() {
    return await this.apiService.get("TemporalOrder/refresh", {"id" : this.id})
  }

}