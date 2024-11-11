import { Component, OnInit } from '@angular/core';
import { Product } from '../../models/product';
import { OrdinationDirection } from '../../models/enums/ordination-direction';
import { OrdinationType } from '../../models/enums/ordination-type';
import { ProductType } from '../../models/enums/product-type';
import { QuerySelector } from '../../models/query-selector';
import { ProductService } from '../../services/product.service';
import { ApiService } from '../../services/api.service';
import { CartContent } from '../../models/cart-content';
import { HeaderShopComponent } from '../../components/header-shop/header-shop.component';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-shopping-cart',
  standalone: true,
  imports: [HeaderShopComponent, FormsModule],
  templateUrl: './shopping-cart.component.html',
  styleUrl: './shopping-cart.component.css'
})
export class ShoppingCartComponent implements OnInit {
  /*numOfTotalProducts = 0;
  totalPrice = 0.0;
  totalProducts : Product[] = []
  numOfIndividualProduct = 0*/

  shoppingCartProducts: Product[] = []
  allProducts: Product[] | null | undefined = []
  querySelector: QuerySelector;
  
  // Estos servicios son para pruebas
  constructor(private productService: ProductService, private apiService: ApiService) {
    const FIRST_PAGE = 1;
    const PRODUCT_PER_PAGE = 5;
    //QuerySelector por defecto para pruebas
    this.querySelector = new QuerySelector(ProductType.FRUITS, OrdinationType.NAME, OrdinationDirection.ASC, PRODUCT_PER_PAGE, FIRST_PAGE, "");
  }

  async ngOnInit(): Promise<void> {
    this.getShoppingCart();
    this.getAllProducts();
  }

  async getShoppingCart() {
    this.shoppingCartProducts = [];
    const productsRaw = localStorage.getItem("shoppingCart");
    if (productsRaw) this.shoppingCartProducts = JSON.parse(productsRaw);

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
      const result = await this.apiService.get("ShoppingCart", {}, 'json');
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

  async getAllProducts() {
    const result = await this.productService.getAllProducts(this.querySelector);
    this.allProducts = result?.products;
  }

  // ESTO HABRÁ QUE BORRARLO
  async addProductToCart(product: Product) {
    if (this.apiService.jwt == "") {
      product.total = 1
      this.shoppingCartProducts.push(product)
      localStorage.setItem("shoppingCart", JSON.stringify(this.shoppingCartProducts));
    }
    else {
      localStorage.removeItem("shoppingCart")
      const cartContent = new CartContent(1, product.id)
      await this.apiService.post("ShoppingCart/add", cartContent)
      this.getShoppingCart()
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
        this.deleteFromArray(product)
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

  async pay() {
    for (const product of this.shoppingCartProducts) {
      const newProduct = await this.productService.getById(product.id)
      if (newProduct) {
        const difference = newProduct.stock - product.stock;
        if (difference < 0) {
          this.deleteFromArray(product)
        }
      }
      else {
        this.deleteFromArray(product)
      }
    }
  }

  deleteFromArray(product: Product) {
    const index = this.shoppingCartProducts.findIndex(p => p.id === product.id);
    this.shoppingCartProducts.splice(index, 1);
    localStorage.setItem("shoppingCart", JSON.stringify(this.shoppingCartProducts))
    //alert("Uno o más productos han sido eliminados del carrrito por falta de stock");
  }

}