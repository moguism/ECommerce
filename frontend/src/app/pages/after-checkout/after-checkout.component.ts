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

@Component({
  selector: 'app-after-checkout',
  standalone: true,
  imports: [HeaderShopComponent, EurosToCentsPipe, HeaderComponent],
  templateUrl: './after-checkout.component.html',
  styleUrl: './after-checkout.component.css'
})
export class AfterCheckoutComponent implements OnInit, OnDestroy
{

  user : User | null = null
  newOrder : Order | null


  shoppingCartProducts: Product[] = []
  id: string = ""
  private method: string = ""

  constructor(private productService: ProductService, private apiService: ApiService, 
    private userService : UserService, private activatedRoute: ActivatedRoute) {
  }

  ngOnDestroy(): void {
    localStorage.removeItem("method")
  }

  async createOrder(){
    this.apiService.get("checkout/status/"+this.id)
  }

  // Falta obtener la orden

  async ngOnInit(): Promise<void> {
    const id = this.activatedRoute.snapshot.queryParamMap.get('session_id') as unknown as string;
    console.log("PRUEBA: ", id)
    if(id != "" && id != null)
    {
      this.id = id
      this.createOrder()
    }
    else
    {
      const method = localStorage.getItem("method")
      if(method)
      {
        this.id = "completado"
      }
    }

    await this.getUser()

  }


  async getUser(){
    this.user = await this.userService.getUser()
  }



  totalprice() {
    let totalcount = 0;
    for (const product of this.shoppingCartProducts) {
      totalcount += product.total * product.price;
    }
    return totalcount;
  }

}
