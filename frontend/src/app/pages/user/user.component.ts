import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { HeaderComponent } from '../../components/header/header.component';
import { User } from '../../models/user';
import { UserService } from '../../services/user.service';
import { ProductService } from '../../services/product.service';
import { FormsModule } from '@angular/forms';
import { EurosToCentsPipe } from '../../pipes/euros-to-cents.pipe';
import { CommonModule, DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';

// Pipe Import
import { CorrectDatePipe } from '../../pipes/correct-date.pipe';
import { Product } from '../../models/product';
import { Order } from '../../models/order';
import { ProductsToBuy } from '../../models/products-to-buy';
import { Router } from '@angular/router';
import { ApiService } from '../../services/api.service';



@Component({
  selector: 'app-user',
  standalone: true,
  imports: [HeaderComponent, FormsModule, CorrectDatePipe, EurosToCentsPipe,CommonModule,RouterLink],
  templateUrl: './user.component.html',
  styleUrl: './user.component.css',
  providers:[DecimalPipe]
})
export class UserComponent implements OnInit {
  constructor(public userService: UserService, private productService: ProductService,private decimalPipe:DecimalPipe, private router: Router, private api : ApiService) {
  }


  user: User | null = null;
  orders: Order[] = [];
  elementShowing: string = "";
  allUsers: User[] = [];
  allProducts: Product[] = [];
  formState: string | null = null; // Puede ser 'editRole' o 'createProduct'
  editRoleValue: string = "";
  newProductName: string = "";
  newProductPrice: number = 0;
  newProductCategory:string="";
  newProductStock: number = 0;
  newproductDescription: string="";
  selectedUser: User | null = null;
  Product:Product|null=null;
  category:string="";
  categorytranslate:string="";
  image: File | null = null
  pricedecimal:string="";
  create : boolean = false;
  idToUpdate : number = 0

  async ngOnInit(): Promise<void> {
    /*if(this.api.jwt == null || this.api.jwt == "")
    {
      this.router.navigateByUrl("login")
    }*/
    await this.getUser();
    await this.getAllOrders();
  }

  async getUser() {
    const result = await this.userService.getUser();
    if (result) {
      this.user = result;
    }
  }

  async getAllOrders(){
    const result = await this.userService.getAllOrders();
    if (result) {
      this.orders = result;
      this.orders.forEach(order => {
        order.wishlist.products.forEach(async productToBuy => {
          const product = await this.productService.getById(productToBuy.productId);
          if(product)
            productToBuy.product = product;  
        })
      });
    }
  }

  async saveUserData(){
    const newName = document.getElementById("new-name") as HTMLInputElement
    const newEmail = document.getElementById("new-email") as HTMLInputElement
    const newAddress = document.getElementById("new-address") as HTMLInputElement
    const newPassword = document.getElementById("new-password") as HTMLInputElement

    if (newName && newEmail && newAddress && newPassword && this.user) {
      if (newName.value != "") {
        this.user.name = newName.value
        this.userService.userName = newName.value
      }
      if(newEmail.value != ""){
        this.user.email = newEmail.value
      }
      if(newAddress.value != ""){
        this.user.address = newAddress.value
      }
      if(newPassword.value != ""){
        this.user.password = newPassword.value
      }
      
      await this.userService.updateUser(this.user);
      await this.userService.obtainNewJwt()
    }
    
    this.formState = null;
  }

  closeForm() {
    this.formState = null;
    this.image = null;
  }

  totalprice(products: ProductsToBuy[]) {
    let totalcount = 0;
    for (const product of products) {
      totalcount += product.quantity * product.purchasePrice; //Precio total pagado, no se modifica cuando se cambian el precio de los productos
    }
    return totalcount;
  }
  
}
