import { Component, OnDestroy, OnInit } from '@angular/core';
import { HeaderComponent } from '../../components/header/header.component';
import { EurosToCentsPipe } from '../../pipes/euros-to-cents.pipe';
import { ActivatedRoute } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { ProductService } from '../../services/product.service';
import { UserService } from '../../services/user.service';
import { User } from '../../models/user';
import { Order } from '../../models/order';
import { ShoppingCartService } from '../../services/shopping-cart.service';
import { LoadingComponent } from '../../components/loading/loading.component';

@Component({
  selector: 'app-after-checkout',
  standalone: true,
  imports: [EurosToCentsPipe, HeaderComponent, LoadingComponent],
  templateUrl: './after-checkout.component.html',
  styleUrl: './after-checkout.component.css'
})
export class AfterCheckoutComponent implements OnInit, OnDestroy {

  user: User | null = null
  lastOrder: Order | null = null
  id: string = ""

  constructor(private productService: ProductService, private apiService: ApiService,
    private userService: UserService, private activatedRoute: ActivatedRoute, private shoppingCartService : ShoppingCartService) {
  }

  ngOnDestroy(): void {
    localStorage.removeItem("method")
  }


  async ngOnInit(): Promise<void> {
    const id = this.activatedRoute.snapshot.queryParamMap.get('temporalId') as unknown as string;
    if (id != null && id != "") {
      this.id = id
      await this.createOrder()
    }
    else {
      const orderCheckout = sessionStorage.getItem("orderCheckout")
      if (orderCheckout) {
        this.lastOrder = JSON.parse(orderCheckout)
        const dataRaw = JSON.parse(orderCheckout)
        this.lastOrder = JSON.parse(dataRaw)
      }
    }
    await this.convertToCompleteProduct()
    await this.getUser()

    console.log("LAST ORDER: ", this.lastOrder)

    //this.shoppingCartService.getShoppingCartCount()
  }

  async createOrder() {
    const orderResult = await this.apiService.get("checkout/status/" + this.id)
    if (orderResult.data) {
      this.lastOrder = orderResult.data
    }
  }

  async convertToCompleteProduct() {
    if (!this.lastOrder?.wishlist?.products) return;

    for (const productToBuy of this.lastOrder.wishlist.products) {
      var product = await this.productService.getById(productToBuy.product.id);
      if(product)
        productToBuy.product = product
    }
  }

  async getUser() {
    this.user = await this.userService.getUser()
  }


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
