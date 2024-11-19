import { Component, OnInit } from '@angular/core';
import { HeaderComponent } from '../../components/header/header.component';
import { User } from '../../models/user';
import { ApiService } from '../../services/api.service';
import { UserService } from '../../services/user.service';
import { ProductService } from '../../services/product.service';
import { Product } from '../../models/product';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [HeaderComponent],
  templateUrl: './user.component.html',
  styleUrl: './user.component.css'
}) 
export class UserComponent implements OnInit {
  constructor(private userService: UserService, private productService: ProductService){
  }

  user: User | null = null;
  elementShowing : string = "";
  allUsers : User[] = []
  allProducts : Product[] = []

  async ngOnInit(): Promise<void> {
    await this.getUser()
  }

  async getUser(){

    const result = await this.userService.getUser();
    if(result){
      this.user = result
      this.changeElementShowing("users")
    }
  }

  async changeElementShowing(newElement : string)
  {
    if(newElement == this.elementShowing)
    {
      return
    }

    this.elementShowing = newElement
    switch(this.elementShowing)
    {
      case "users":
        await this.getAllUsers()
        break
      case "products":
        await this.getAllProducts()
        break
    }
  }

  async deleteUser(id : number)
  {
    const response = prompt("¿Seguro que quieres borrar al usuario? Escribe lo que sea para hacerlo, o no hagas nada para no hacerlo")
    if(response == "" || response == null)
    {
      return;
    }

    await this.userService.deleteUser(id)
    alert("Usuario borrado correctamente")
    this.getAllUsers()
  }

  async getAllUsers()
  {
    const users = await this.userService.getAllUsers()
    if(users != null) this.allUsers = users
  }

  async getAllProducts()
  {
    const products = await this.productService.getCompleteProducts()
    if(products != null) this.allProducts = products
  }
  
}
