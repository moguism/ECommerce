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
import { BlockchainService } from '../../services/blockchain.service';
import { CreateEthTransactionRequest } from '../../models/create-eth-transaction-request';

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

  //blockchain
  networkUrl: string = 'https://rpc.bordel.wtf/test'; // Red de pruebas;
  eurosToSend: number = 0;
  addressToSend: string = "0x3402A2c72FFc187C67f2c467eCDd4181d873778a";

  constructor(private productService: ProductService, private apiService: ApiService,
    private router: Router, private activatedRoute: ActivatedRoute,
    private stripeService: StripeService, private blockchainService: BlockchainService) {
  }

  ngOnDestroy(): void {
    this.autoRefreshSubscription?.unsubscribe();
  }


  async ngOnInit(): Promise<void> {
    this.id = this.activatedRoute.snapshot.paramMap.get('id') as unknown as number;
    this.method = this.activatedRoute.snapshot.paramMap.get('method') as unknown as string;
    const shoppinCartResult = await this.apiService.get("TemporalOrder", { "id": this.id }, 'json');
    console.log("RESULTADO AAAAAA: ", shoppinCartResult)
    if (shoppinCartResult.data) {
      const data: any = shoppinCartResult.data;
      const cartContent: any[] = data.cartContentDtos;
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
    const request = await this.apiService.post('Checkout/embedded', this.id);

    if (request.success && request.data) {
      const data: any = JSON.parse(request.data)
      const options: StripeEmbeddedCheckoutOptions = {
        clientSecret: data.clientSecret
      };

      this.stripeService.initEmbeddedCheckout(options)
        .subscribe((checkout) => {
          this.stripeEmbedCheckout = checkout;
          checkout.mount('#checkout');
          if (this.checkoutDialogRef) {
            this.checkoutDialogRef.nativeElement.showModal();
          }
        });
    }
  }

  async hostedCheckout() {
    const request = await this.apiService.post('Checkout/hosted', this.id);

    if (request.success && request.data) {
      const data: any = JSON.parse(request.data)
      // Abrimos la url de la session de stripe sin crear una nueva pestaña en el navegador 
      window.open(data.sessionUrl, '_self');
    }
  }

  cancelCheckoutDialog() {
    if (this.stripeEmbedCheckout && this.checkoutDialogRef) {
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
    return interval(30000).subscribe(() => { this.refreshOrder() });
  }

  async refreshOrder() {
    console.log("Mandando petición...")
    return await this.apiService.get("TemporalOrder/refresh", { "id": this.id })
  }


  /******** Blockchain  **********/
  async createTransaction() {

    // Si no está instalado Metamask se lanza un error y se corta la ejecución
    if (!window.ethereum) {
      throw new Error('Metamask not found');
    }

    // Obtener la cuenta de metamask del usuario
    const accounts = await window.ethereum.request({ method: 'eth_requestAccounts' });
    const account = accounts[0];

    // Pedimos permiso al usuario para usar su cuenta de metamask
    await window.ethereum.request({
      method: 'wallet_requestPermissions',
      params: [{
        "eth_accounts": { account }
      }]
    });

    // Obtenemos los datos que necesitamos para la transacción: 
    // gas, precio del gas y el valor en Ethereum
    const transactionRequest: CreateEthTransactionRequest = {
      networkUrl: this.networkUrl,
      euros: this.eurosToSend
    };


    const ethereumInfoResult = await this.blockchainService.getEthereumInfo(transactionRequest);
    //para no dar problemas con los posibles nulos
    var ethereumInfo: any
    ethereumInfo = ethereumInfoResult.data;

    try {
      // Creamos la transacción y pedimos al usuario que la firme
      const transactionHash = await window.ethereum.request({
        method: 'eth_sendTransaction',
        params: [{
          from: account,
          to: this.addressToSend,
          value: ethereumInfo.value,
          gas: ethereumInfo.gas,
          gasPrice: ethereumInfo.gasPrice
        }]
      });

      // Pedimos al servidor que verifique la transacción.
      // CUIDADO: si el cliente le manda todos los datos,
      // podría engañar al servidor.
      const checkTransactionRequest = {
        networkUrl: this.networkUrl,
        hash: transactionHash,
        from: account,
        to: this.addressToSend,
        value: ethereumInfo.value
      }

      const checkTransactionResult = await this.blockchainService.checkTransaction(checkTransactionRequest);

      //Si la transacción ha sido exitosa, crea la orden y elimina el carrito
      if (checkTransactionResult.success && checkTransactionResult.data) {
        alert('Transacción realizada con éxito');




        
      } else {
        alert('Transacción fallida');
      }
    }catch(error)
    {
      // Captura el error en la transacción y muestra un mensaje
      console.error('Error en la transacción:', error);
      alert('Transacción fallida');
    }
    

    
    
  
  }






}


declare global {
  interface Window {
    ethereum: any;
  }
}