import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product.service';
import { ApiService } from '../../services/api.service';
import { HeaderShopComponent } from '../../components/header-shop/header-shop.component';
import { EurosToCentsPipe } from '../../pipes/euros-to-cents.pipe';
import { ActivatedRoute, Router } from '@angular/router';
import { HeaderComponent } from '../../components/header/header.component';
import { interval, Subscription } from 'rxjs';
import { StripeService } from 'ngx-stripe';
import { StripeEmbeddedCheckout, StripeEmbeddedCheckoutOptions } from '@stripe/stripe-js';

@Component({
  selector: 'app-shopping-cart',
  standalone: true,
  imports: [HeaderShopComponent, EurosToCentsPipe, HeaderComponent],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.css'
})
export class CheckoutComponent implements OnInit, OnDestroy {
  shoppingCartProducts: Product[] = []
  autoRefreshSubscription: Subscription | undefined;
  private id: number = 0
  private method: string = ""
  
  @ViewChild('checkoutDialog')
  checkoutDialogRef: ElementRef<HTMLDialogElement> | null = null;
  stripeEmbedCheckout: StripeEmbeddedCheckout | null = null;

  constructor(private productService: ProductService, private apiService: ApiService, private router: Router, private activatedRoute: ActivatedRoute, private stripeService: StripeService) {
  }
  
  ngOnDestroy(): void {
    this.autoRefreshSubscription?.unsubscribe();
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

  /*async initiatePayment() {
    //const response = await this.apiService.post('Checkout/embedded', this.shoppingCartProducts);
    const response = await this.apiService.post('Checkout/embedded');
    if(response.data == null) return;
    const data : any = JSON.parse(response.data);
    const sessionId = data.sessionId;

    this.stripeService.redirectToCheckout({ sessionId })
      .subscribe({
        next: (result) => {
          if (result.error) {
            console.error('Error al redirigir a Stripe Checkout:', result.error.message);
          }
        }
      });
  }*/

  async embeddedCheckout() {
    const request = await this.apiService.post('Checkout/embedded');

    if (request.success && request.data) {
      const data : any = JSON.parse(request.data)
      const options: StripeEmbeddedCheckoutOptions = {
        clientSecret: data.clientSecret
      };

      this.stripeService.initEmbeddedCheckout(options)
        .subscribe((checkout) => {
          this.stripeEmbedCheckout = checkout;
          checkout.mount('#checkout');
          if(this.checkoutDialogRef)
          {
            this.checkoutDialogRef.nativeElement.showModal();
          }
        });
      }
  }

  async hostedCheckout() {
    const request = await this.apiService.post('Checkout/hosted');

    if (request.success && request.data) {
      const data : any = JSON.parse(request.data)
      // Abrimos la url de la session de stripe sin crear una nueva pestaña en el navegador 
      window.open(data.sessionUrl, '_self');
    }
  }

  cancelCheckoutDialog() {
    if(this.stripeEmbedCheckout && this.checkoutDialogRef)
    {
      this.stripeEmbedCheckout.destroy();
      this.checkoutDialogRef.nativeElement.close();
    }
  }

  totalprice() {
    let totalcount = 0;
    for (const product of this.shoppingCartProducts) {
      totalcount += product.total * product.price;
    }
    return totalcount;
  }

  startAutoRefresh() {
    // 120.000 milisegundos son 2 minutos
    return interval(30000).subscribe(() => {this.refreshOrder()});
  }

  async refreshOrder() {
    console.log("Mandando petición...")
    return await this.apiService.get("TemporalOrder/refresh", {"id" : this.id})
  }

}