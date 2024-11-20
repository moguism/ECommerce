import { Component, OnInit } from '@angular/core';
import { HeaderComponent } from '../../components/header/header.component';
import { User } from '../../models/user';
import { UserService } from '../../services/user.service';
import { ProductService } from '../../services/product.service';
import { FormsModule } from '@angular/forms';
import { EurosToCentsPipe } from '../../pipes/euros-to-cents.pipe';

// Pipe Import
import { CorrectDatePipe } from '../../pipes/correct-date.pipe';
import { Product } from '../../models/product';


@Component({
  selector: 'app-user',
  standalone: true,
  imports: [HeaderComponent, FormsModule, CorrectDatePipe, EurosToCentsPipe],
  templateUrl: './user.component.html',
  styleUrl: './user.component.css'
})
export class UserComponent implements OnInit {
  constructor(private userService: UserService, private productService: ProductService) {
  }

  user: User | null = null;
  btnEdit: boolean = false;
  elementShowing: string = "";
  allUsers: User[] = [];
  allProducts: Product[] = [];
  formState: string | null = null; // Puede ser 'editRole' o 'createProduct'
  editRoleValue: string = "";
  newProductName: string = "";
  newProductPrice: number | null = null;
  newProductCategory: string = "";
  selectedUser: User | null = null;

  async ngOnInit(): Promise<void> {
    await this.getUser();
  }

  async getUser() {
    const result = await this.userService.getUser();
    if (result) {
      this.user = result;
      this.changeElementShowing("users");
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
    this.newProductCategory = "";
  }

  closeForm() {
    this.formState = null;
  }

  async submitEditRole() {
    if(this.selectedUser)
    {
      this.selectedUser.role = this.editRoleValue;
      await this.userService.updateUserAdmin(this.selectedUser);
      this.closeForm();
      this.getAllUsers();
      alert(`Rol actualizado a: ${this.editRoleValue}`);
    }
  }

  async submitCreateProduct() {
    alert(`Producto creado: ${this.newProductName}, Precio: ${this.newProductPrice}, Categoría: ${this.newProductCategory}`);
    this.closeForm();
  }

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

  totalprice(products: Product[]) {
    let totalcount = 0;
    for (const product of products) {
      totalcount += product.total * product.price;
    }
    return totalcount;
  }
  
}
