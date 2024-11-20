import { Component, OnDestroy, OnInit } from '@angular/core';
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
  imports: [EurosToCentsPipe, HeaderComponent],
  templateUrl: './after-checkout.component.html',
  styleUrl: './after-checkout.component.css'
})
export class AfterCheckoutComponent implements OnInit, OnDestroy {

  user: User | null = null
  lastOrder: Order | null = null
  private method: string = ""
  id : string = ""

  constructor(private productService: ProductService, private apiService: ApiService,
    private userService: UserService, private activatedRoute: ActivatedRoute) {
  }

  ngOnDestroy(): void {
    localStorage.removeItem("method")
  }


  // Falta obtener la orden

  async ngOnInit(): Promise<void> {
    const id = this.activatedRoute.snapshot.queryParamMap.get('session_id') as unknown as string;
    if(id != null && id != "")
    {
      this.id = id
      await this.createOrder()
      //await this.getLastOrder(id)
    }
    else
    {
      const orderCheckout = sessionStorage.getItem("orderCheckout")
      if(orderCheckout)
      {
        this.lastOrder = JSON.parse(orderCheckout)
      }
    }
    await this.getUser()

    console.log(this.lastOrder)
  }

  async createOrder(){
    const orderResult = await this.apiService.get("checkout/status/"+this.id)
    if(orderResult.data)
    {
      this.lastOrder = orderResult.data 
    }
  }

  async getUser() {
    this.user = await this.userService.getUser()
  }

  /*async getLastOrder(id : string) {
    const orderResult = await this.apiService.get<Order>("Order/lastOrder", {"id" : id}, "json")
    console.log("ORDER RESULT: ", orderResult)
    this.lastOrder = orderResult.data
  }*/


  totalprice() {
    let totalcount = 0;

    if (this.lastOrder) {

      for (const product of this.lastOrder.wishlist.products) {
        totalcount += product.quantity * product.product.price;
      }
    }

    return totalcount;
  }

}
