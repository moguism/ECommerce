import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { HeaderComponent } from '../../components/header/header.component';
import { User } from '../../models/user';
import { UserService } from '../../services/user.service';
import { ProductService } from '../../services/product.service';
import { FormsModule } from '@angular/forms';
import { EurosToCentsPipe } from '../../pipes/euros-to-cents.pipe';

// Pipe Import
import { CorrectDatePipe } from '../../pipes/correct-date.pipe';
import { Product } from '../../models/product';
import { Order } from '../../models/order';
import { ProductsToBuy } from '../../models/products-to-buy';
import { TranslatePipe } from '../../pipes/translate.pipe';
import { Category } from '../../models/category';



@Component({
  selector: 'app-user',
  standalone: true,
  imports: [HeaderComponent, FormsModule, CorrectDatePipe, EurosToCentsPipe, TranslatePipe],
  templateUrl: './user.component.html',
  styleUrl: './user.component.css'
})
export class UserComponent implements OnInit {
  constructor(private userService: UserService, private productService: ProductService) {
  }


  user: User | null = null;
  btnEdit: boolean = false;
  orders: Order[] = [];
  elementShowing: string = "";
  allUsers: User[] = [];
  allProducts: Product[] = [];
  formState: string | null = null; // Puede ser 'editRole' o 'createProduct'
  editRoleValue: string = "";
  newProductName: string = "";
  newProductPrice: number | null = null;
  newProductCategory:string="";
  newProductStock: number|null=null;
  newproductDescription: string="";
  selectedUser: User | null = null;
  Product:Product|null=null;
  category:string="";
  categorytranslate:string="";

  async ngOnInit(): Promise<void> {
    await this.getUser();
    await this.getAllOrders();
  }

  async getUser() {
    const result = await this.userService.getUser();
    if (result) {
      this.user = result;
      this.changeElementShowing("users");
    }
  }

  async getAllOrders(){
    const result = await this.userService.getAllOrders();
    if (result) {
      this.orders = result;
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
      //await this.userService.obtainNewJwt()
    }
    
    this.btnEdit = false
  }

  async changeElementShowing(newElement: string) {
    if (newElement == this.elementShowing) {
      return;
    }
    this.elementShowing = newElement;
    switch (this.elementShowing) {
      case "users":
        await this.getAllUsers();
        break;
      case "products":
        await this.getAllProducts();
        break;
    }
  }

  showEditRoleForm(user: User) {
    this.formState = "editRole";
    this.editRoleValue = user.role;
    this.selectedUser = user
  }

  showCreateProductForm() {
    this.formState = "createProduct";
    this.newProductName = "";
    this.newProductPrice = null;
    /*this.newProductCategory = "";*/
  }
  showEditProductForm(id: number){
    const translatepipe=new TranslatePipe();
    const eurosToCentsPipe=new EurosToCentsPipe();
    this.formState="modifyProduct"
    this.Product=this.allProducts[id-1];
    this.newProductName = this.Product.name;
    this.newProductPrice = this.Product.price;
    this.category=this.Product.category.name;
    this.categorytranslate=translatepipe.transform(this.category)
    this.newProductCategory = this.categorytranslate;
    this.newProductStock=this.Product.stock;
    this.newproductDescription=this.Product.description;

  }

  closeForm() {
    this.formState = null;
  }

  async submitEditRole() {
    if(this.selectedUser)
    {
      this.selectedUser.role = this.editRoleValue;
      await this.userService.updateUser(this.selectedUser);
      this.closeForm();
      this.getAllUsers();
      alert(`Rol actualizado a: ${this.editRoleValue}`);
    }
  }

  async submitCreateProduct() {
    alert(`Producto creado: ${this.newProductName}, Precio: ${this.newProductPrice}, Categoría: ${this.newProductCategory}`);
    this.closeForm();
  }
  /*async submitModifyProduct() {
    alert(`Producto creado: ${this.newProductName}, Precio: ${this.newProductPrice}, Categoría: ${this.newProductCategory}`);
    await this.productService.modifyProduct();
    this.closeForm();
  }*/

  async deleteUser(id: number) {
    const response = confirm("¿Seguro que quieres borrar al usuario?");
    if (response) {
      await this.userService.deleteUser(id);
      alert("Usuario borrado correctamente");
      this.getAllUsers();
    }
  }

  async getAllUsers() {
    const users = await this.userService.getAllUsers();
    if (users != null) this.allUsers = users;
  }

  async getAllProducts() {
    const products = await this.productService.getCompleteProducts();
    if (products != null) this.allProducts = products;
  }

  totalprice(products: ProductsToBuy[]) {
    let totalcount = 0;
    for (const product of products) {
      totalcount += product.quantity * product.product.price;
    }
    return totalcount;
  }
  
}
