import { Component, OnDestroy, OnInit } from '@angular/core';
import { HeaderShopComponent } from '../../components/header-shop/header-shop.component';
import { HeaderComponent } from '../../components/header/header.component';
import { EurosToCentsPipe } from '../../pipes/euros-to-cents.pipe';
import { ActivatedRoute } from '@angular/router';
import { Product } from '../../models/product';
import { ApiService } from '../../services/api.service';
import { ProductService } from '../../services/product.service';
import { UserService } from '../../services/user.service';
import { User } from '../../models/user';
import { Order } from '../../models/order';

@Component({
  selector: 'app-after-checkout',
  standalone: true,
  imports: [HeaderShopComponent, EurosToCentsPipe, HeaderComponent],
  templateUrl: './after-checkout.component.html',
  styleUrl: './after-checkout.component.css'
})
export class AfterCheckoutComponent implements OnInit, OnDestroy {

  user: User | null = null
  lastOrder: Order | null = null
  private method: string = ""

  constructor(private productService: ProductService, private apiService: ApiService,
    private userService: UserService, private activatedRoute: ActivatedRoute) {
  }

  ngOnDestroy(): void {
    localStorage.removeItem("method")
  }


  // Falta obtener la orden

  async ngOnInit(): Promise<void> {

    await this.getUser()
    await this.getLastOrder()

    console.log(this.lastOrder)
  }


  async getUser() {
    this.user = await this.userService.getUser()
  }

  async getLastOrder() {
    const orderResult = await this.apiService.get<Order>("Order/lastOrder", {}, "json")
    this.lastOrder = orderResult.data
  }


  totalprice() {
    let totalcount = 0;

    if (this.lastOrder) {

      for (const product of this.lastOrder?.Products) {
        totalcount += product.total * product.price;
      }
    }

    return totalcount;
  }

}
