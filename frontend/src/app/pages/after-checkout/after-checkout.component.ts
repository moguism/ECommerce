import { Component, OnInit } from '@angular/core';
import { HeaderShopComponent } from '../../components/header-shop/header-shop.component';
import { HeaderComponent } from '../../components/header/header.component';
import { EurosToCentsPipe } from '../../pipes/euros-to-cents.pipe';
import { ActivatedRoute } from '@angular/router';
import { Product } from '../../models/product';
import { ApiService } from '../../services/api.service';
import { ProductService } from '../../services/product.service';

@Component({
  selector: 'app-after-checkout',
  standalone: true,
  imports: [HeaderShopComponent, EurosToCentsPipe, HeaderComponent],
  templateUrl: './after-checkout.component.html',
  styleUrl: './after-checkout.component.css'
})
export class AfterCheckoutComponent implements OnInit 
{
  shoppingCartProducts: Product[] = []
  id: string = ""
  private method: string = ""

  constructor(private productService: ProductService, private apiService: ApiService, private activatedRoute: ActivatedRoute) {
  }

  async ngOnInit(): Promise<void> {
    const id = this.activatedRoute.snapshot.queryParamMap.get('session_id') as unknown as string;
    console.log("PRUEBA: ", id)
    if(id != "" && id != null)
    {
      this.id = id
    }
    /*this.method = this.activatedRoute.snapshot.paramMap.get('method') as unknown as string;
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
    }*/
  }

  totalprice() {
    let totalcount = 0;
    for (const product of this.shoppingCartProducts) {
      totalcount += product.total * product.price;
    }
    return totalcount;
  }

}
