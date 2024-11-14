import { Component, OnInit } from '@angular/core';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product.service';
import { ApiService } from '../../services/api.service';
import { CartContent } from '../../models/cart-content';
import { HeaderShopComponent } from '../../components/header-shop/header-shop.component';
import { FormsModule } from '@angular/forms';
import { EurosToCentsPipe } from '../../pipes/euros-to-cents.pipe';
import { Router } from '@angular/router';

@Component({
  selector: 'app-shopping-cart',
  standalone: true,
  imports: [HeaderShopComponent, FormsModule,EurosToCentsPipe],
  templateUrl: './shopping-cart.component.html',
  styleUrl: './shopping-cart.component.css'
})
export class ShoppingCartComponent implements OnInit {
  shoppingCartProducts: Product[] = []
  protected count = 0;
  
  constructor(private productService: ProductService, private apiService: ApiService, private router: Router) {}

  async ngOnInit(): Promise<void> {
    const goToCheckout = localStorage.getItem("goToCheckout")
    if(this.apiService.jwt != "" && goToCheckout && goToCheckout == "true")
    {
      this.createDirectPayment();
      localStorage.removeItem("goToCheckout")
    }
    else
    {
      this.getShoppingCart();
    }
  }

  getLocalStorageCart()
  {
    this.shoppingCartProducts = [];
    const productsRaw = localStorage.getItem("shoppingCart");
    if (productsRaw) this.shoppingCartProducts = JSON.parse(productsRaw);
  }

  async getShoppingCart() {
    this.getLocalStorageCart();

    if (this.apiService.jwt !== "" && this.shoppingCartProducts.length > 0) {
      console.log("Sincronizando productos locales al carrito del backend...");
  
      for (const product of this.shoppingCartProducts) {
        const cartContent = new CartContent(product.total, product.id);
        await this.apiService.post("ShoppingCart/add", cartContent);
      }
  
      localStorage.removeItem("shoppingCart");
      this.shoppingCartProducts = [];
    }
  
    if (this.apiService.jwt !== "") {
      const result = await this.apiService.get("ShoppingCart", {"isTemporal" : false}, 'json');
      if (result.data) {
        const data: any = result.data;
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
      console.log("CARRITO SINCRONIZADO: ", this.shoppingCartProducts);
    }
  }

  async changeQuantity(product: Product) {
    const input = document.getElementById(product.id.toString()) as HTMLInputElement
    if(input && parseInt(input.value) <= 0)
    {
      alert("Cantidad no válida")
      return
    }
    else if(input)
    {
      if (this.apiService.jwt == "") {
        const p = this.findProductInArray(product.id)
        p.total = parseInt(input.value);
        localStorage.setItem("shoppingCart", JSON.stringify(this.shoppingCartProducts));
      }
      else {
        localStorage.removeItem("shoppingCart")
        const cartContent = new CartContent(parseInt(input.value), product.id)
        await this.apiService.post("ShoppingCart", cartContent)
        this.getShoppingCart()
      }
    }
  }

  async deleteProduct(productId: number) {
    const index = this.shoppingCartProducts.findIndex(product => product.id === productId);

    if (index !== -1) {
      const product = this.shoppingCartProducts[index];

      if (product.total > 1) {
        product.total -= 1;
      } else {
        this.shoppingCartProducts.splice(index, 1);
      }

      if (this.apiService.jwt == "") {
        this.deleteFromArray(product, false)
      }
      else {
        await this.apiService.delete("ShoppingCart", { productId })
        this.getShoppingCart()
      }
    }
  }

  findProductInArray(id: number): Product
  {
    const index = this.shoppingCartProducts.findIndex(product => product.id === id);
    const product = this.shoppingCartProducts[index];
    return product;
  }

  async pay(method : string) {
    if(this.shoppingCartProducts.length == 0)
    {
      alert("No hay nada que pagar, bobolón")
      return
    }
    for (const product of this.shoppingCartProducts) {
      const newProduct = await this.productService.getById(product.id)
      if (newProduct) {
        const difference = newProduct.stock - product.stock;
        if (difference < 0) {
          this.deleteFromArray(product, true)
          return
        }
      }
      else {
        this.deleteFromArray(product, true)
        return
      }
    }
    localStorage.setItem("method", method)
    if(this.apiService.jwt != "")
    {
      const result = await this.apiService.post("TemporalOrder")
      console.log("ORDEN TEMPORAL: ", result)
      this.goToCheckout(result)
    }
    else
    {
      localStorage.setItem("goToCheckout", "true")
      this.router.navigateByUrl("login")
    }
  }

  // Esta función solo debería servir para el carrito local
  async createDirectPayment()
  {
    this.getLocalStorageCart()
    const cartContents : CartContent[] = []
    for(const product of this.shoppingCartProducts)
    {
      const cartContent = new CartContent(product.total, product.id)
      cartContents.push(cartContent)
    }
    const result = await this.apiService.post("TemporalOrder/direct", cartContents)
    console.log("PAGO DIRECTO: ", result)
    this.goToCheckout(result)
  }

  goToCheckout(result: any)
  {
    if(result.data)
    {
      const data = JSON.parse(result.data)
      const url: string = "checkout/" + localStorage.getItem("method") + "/" +  data.id
      this.router.navigateByUrl(url)
    }
    else
    {
      alert("Ha ocurrido un error")
    }
  }

  deleteFromArray(product: Product, showAlert: boolean) {
    const index = this.shoppingCartProducts.findIndex(p => p.id === product.id);
    this.shoppingCartProducts.splice(index, 1);
    localStorage.setItem("shoppingCart", JSON.stringify(this.shoppingCartProducts))
    if(showAlert)
    {
      alert("Uno o varios productos han sido eliminados por falta de stock") 
    }
  }

  totalprice(){
    let totalcount=0;
    for(const product of this.shoppingCartProducts){
      totalcount+=product.total*product.price;
    }
    return totalcount;
  }
  sumar(index: number) {
    const quantity = this.shoppingCartProducts.findIndex(product => product.id === index);
    this.shoppingCartProducts[quantity].total++;
  }
  restar(index: number) {
    const quantity = this.shoppingCartProducts.findIndex(product => product.id === index);
    if(this.shoppingCartProducts[quantity].total>0){
      this.shoppingCartProducts[quantity].total--;
    }
  }

}