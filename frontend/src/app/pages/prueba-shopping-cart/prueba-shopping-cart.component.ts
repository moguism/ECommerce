import { Component, OnInit } from '@angular/core';
import { PruebaShoppingCartServiceService } from '../../services/prueba-shopping-cart-service.service';
import { Order } from '../../models/order';
import { ProductService } from '../../services/product.service';
import { Product } from '../../models/product';
import { QuerySelector } from '../../models/query-selector';
import { OrdinationDirection } from '../../models/enums/ordination-direction';
import { OrdinationType } from '../../models/enums/ordination-type';
import { ProductType } from '../../models/enums/product-type';
import { OrderDto } from '../../models/order-dto';

@Component({
  selector: 'app-prueba-shopping-cart',
  standalone: true,
  imports: [],
  templateUrl: './prueba-shopping-cart.component.html',
  styleUrl: './prueba-shopping-cart.component.css'
})
export class PruebaShoppingCartComponent implements OnInit {
  shoppingCart : OrderDto | null = null
  allProducts: Product[] | null | undefined = []
  querySelector: QuerySelector;
   
  constructor(private shoppingCartService: PruebaShoppingCartServiceService, private productService: ProductService)
  {
    const FIRST_PAGE = 1;
    const PRODUCT_PER_PAGE = 5;
    //QuerySelector por defecto para pruebas
    this.querySelector = new QuerySelector(ProductType.FRUITS, OrdinationType.NAME, OrdinationDirection.ASC, PRODUCT_PER_PAGE, FIRST_PAGE, "");
  }

  async ngOnInit(): Promise<void> {
    await this.getShoppingCart();
    await this.getAllProducts();
  }
  
  async getShoppingCart()
  {
    const result = await this.shoppingCartService.getShoppingCart();
    this.shoppingCart = result.data;
  }

  async getAllProducts()
  {
    const result = await this.productService.getAllProducts(this.querySelector);
    this.allProducts = result.data?.products;
  }

  async addProductToCart(product : Product)
  {
    const products: Product[] = [product]
    const order = new OrderDto(0, products)
    await this.shoppingCartService.updateShoppingCart(order)
    this.getShoppingCart()
  }
}
