import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product.service';
import { ApiService } from '../../services/api.service';
import { EurosToCentsPipe } from '../../pipes/euros-to-cents.pipe';
import { ActivatedRoute, Router } from '@angular/router';
import { HeaderComponent } from '../../components/header/header.component';
import { interval, Subscription } from 'rxjs';
import { StripeService } from 'ngx-stripe';
import { StripeEmbeddedCheckout, StripeEmbeddedCheckoutOptions } from '@stripe/stripe-js';
import { BlockchainService } from '../../services/blockchain.service';
import { CreateEthTransactionRequest } from '../../models/create-eth-transaction-request';
import { LoadingComponent } from '../../components/loading/loading.component';

@Component({
  selector: 'app-shopping-cart',
  standalone: true,
  imports: [EurosToCentsPipe, HeaderComponent, LoadingComponent],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.css'
})
export class CheckoutComponent implements OnInit, OnDestroy {
  shoppingCartProducts: Product[] = []
  autoRefreshSubscription: Subscription | undefined;
  private id: number = 0
  method: string = ""
  
  @ViewChild('checkoutDialog')
  checkoutDialogRef: ElementRef<HTMLDialogElement> | null = null;
  stripeEmbedCheckout: StripeEmbeddedCheckout | null = null;

  //blockchain
  networkUrl: string = 'https://rpc.bordel.wtf/test'; // Red de pruebas;
  eurosToSend: number = 0;
  addressToSend: string = "0x9af71A6E4d25e16B56f944fbB59c9c67DecbFFD2"; //Café para mauricio
  showLoading: boolean = false

  sessionId: string = "";

  constructor(private productService: ProductService, private apiService: ApiService,
    private router: Router, private activatedRoute: ActivatedRoute,
    private stripeService: StripeService, private blockchainService: BlockchainService) {
  }

  ngOnDestroy(): void {
    this.stripeEmbedCheckout?.destroy();
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

  async goToAfterCheckout()
  {
    await this.refreshOrder()
    if(this.sessionId != "")
    {
      this.router.navigateByUrl("after-checkout?session_id=" + this.sessionId)
    }
  }

  async embeddedCheckout() {
    const request = await this.apiService.post('Checkout/embedded', this.id);

    if (request.success && request.data) {
      const data: any = JSON.parse(request.data)
      const options: StripeEmbeddedCheckoutOptions = {
        clientSecret: data.clientSecret,
        onComplete: () => this.goToAfterCheckout()
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

  /*async hostedCheckout() {
    const request = await this.apiService.post('Checkout/hosted', this.id);

    if (request.success && request.data) {
      const data: any = JSON.parse(request.data)
      // Abrimos la url de la session de stripe sin crear una nueva pestaña en el navegador 
      window.open(data.sessionUrl, '_self');
    }
  }*/

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
    const response = await this.apiService.get("TemporalOrder/refresh", { "id": this.id })
    if(response.data)
    {
      const data : any = response.data
      if(data.sessionId)
      {
        this.sessionId = data.sessionId
      }
    }
    console.log("RESPUESTA XD: ", response)
  }


  /******** Blockchain  **********/
  async createTransaction() {

    // Si no está instalado Metamask se lanza un error y se corta la ejecución
    if (!window.ethereum) {
      alert("Metamask no ha sido encontrado")
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

    this.eurosToSend = this.totalprice() //total a pagar
    this.eurosToSend = this.eurosToSend / 100

    // Obtenemos los datos que necesitamos para la transacción: 
    // gas, precio del gas y el valor en Ethereum
    const transactionRequest: CreateEthTransactionRequest = {
      NetworkUrl: this.networkUrl,
      Euros: this.eurosToSend
    };

    console.log("TRANSATION REQUEST: ", transactionRequest)


    const ethereumInfoResult = await this.blockchainService.getEthereumInfo(transactionRequest);
    //para no dar problemas con los posibles nulos
    if(ethereumInfoResult.data == null)
    {
      alert("Ha ocurrido un error")
      return;
    }
    const ethereumInfo =  JSON.parse(ethereumInfoResult.data.toString());

    console.log("ETHERIUM INFO RESULT: ", ethereumInfoResult)
    console.log("ETHERIUM INFO GOOD ENDING: ", ethereumInfo)

    try {
      // Creamos la transacción y pedimos al usuario que la firme
      if (ethereumInfo != null) {
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


        this.showLoading = true
        const checkTransactionResult = await this.blockchainService.checkTransaction(checkTransactionRequest);
        this.showLoading = false

        console.log("Data : ",          
          "\n" + checkTransactionRequest.networkUrl, 
          "\n" + checkTransactionRequest.hash, 
          "\n" + checkTransactionRequest.from ,
          "\n" + checkTransactionRequest.to, 
          "\n" + checkTransactionRequest.value)

        //Si la transacción ha sido exitosa
        if (checkTransactionResult.success) {
          alert('Transacción realizada con éxito');
          console.log(checkTransactionResult.data)
          if(checkTransactionResult.data)
          {
            console.log("Orden creada")
            this.router.navigateByUrl("after-checkout")
            sessionStorage.setItem("method", 'eth')
            sessionStorage.setItem("orderCheckout", JSON.stringify(checkTransactionResult.data))
            console.log("ORDER CHECKOUT: ", checkTransactionResult.data)
          }
          else
            console.log("Error al crear la orden")

        } else {
          alert('Transacción fallida');
        }
      }
      else{
        console.log("error en la información del ethereum")
      }
    } catch (error) {
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