import { Component, OnInit } from '@angular/core';
import { HeaderComponent } from '../../components/header/header.component';
import { User } from '../../models/user';
import { UserService } from '../../services/user.service';
import { ProductService } from '../../services/product.service';
import { Product } from '../../models/product';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [HeaderComponent, FormsModule],
  templateUrl: './user.component.html',
  styleUrl: './user.component.css'
})
export class UserComponent implements OnInit {
  constructor(private userService: UserService, private productService: ProductService) {
  }

  user: User | null = null;
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
}
