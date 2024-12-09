import { Component, HostListener, Input, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { ShoppingCartService } from '../../services/shopping-cart.service';
import { ShoppingCart } from '../../models/shopping-cart';
import { UserService } from '../../services/user.service';
import { any } from 'three/webgpu';
import { User } from '../../models/user';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent implements OnInit {
  protected buttonChange: boolean = true;
  protected dropdownChange: boolean = true;
  protected jwt: string = "";
  @Input() name: string = "";

  contProducts: Number | undefined = 0

  constructor(private apiService: ApiService, private router: Router, 
    public shoppingCartService: ShoppingCartService, public userService : UserService) {
  }

  async ngOnInit(): Promise<void> {
    //this.shoppingCartService.getShoppingCartCount()
    if (this.apiService.jwt == null) {
      return;
    }
    this.jwt = this.apiService.jwt;
    if (this.jwt != "") {
      this.name = JSON.parse(window.atob(this.jwt.split('.')[1])).name;
      console.log("EL NOMBRE ES: ", this.name)

      const result = await this.apiService.get<ShoppingCart>("ShoppingCart", {}, 'json');
      const shoppingCart: ShoppingCart | null = result.data
      if (shoppingCart?.cartContent){
        this.shoppingCartService.contProduct = shoppingCart?.cartContent.length
        console.log("Actualizado contador " + this.contProducts)
      }

      //Si inicia sesión, actualiza el nombre del header
      if (this.userService.userName == "") {
        console.log("No tiene nombre");
        const user : User | null = await this.userService.getUser()
        console.log("Nuevo nombre : ", this.userService.userName); 

      }
      

    }
    else {
      const productsRaw = localStorage.getItem("shoppingCart");
      if (productsRaw) {
        const shoppingCartProducts = JSON.parse(productsRaw);
        if (shoppingCartProducts.cartContent) {
          var cont = shoppingCartProducts.cartContent.length
          console.log("ftyghujiok  --" + shoppingCartProducts.cartContent.length)
          localStorage.setItem("contProducts", cont.toString())
        }
      }

    }

  }

  async deleteToken() {
    this.apiService.deleteToken();
    await this.router.navigateByUrl("login");
    window.location.reload()
  }

  goToRoute(route: string) {
    this.router.navigateByUrl(route)
  }

  showAndClose() {
    if (this.dropdownChange) {
      this.showDropdown();
    } else {
      this.closeDropdown();
    }
  }

  showDropdown() {
    this.dropdownChange = false;
    const dropdown = document.getElementsByClassName("dropdown");
    const dropdownlist = document.getElementsByClassName("dropdown-list");
    dropdownlist[0].className = "view-dropdown-list";
    dropdown[0].className = "view-dropdown";
  }

  closeDropdown() {
    this.dropdownChange = true;
    const viewdropdown = document.getElementsByClassName("view-dropdown");
    const viewdropdownlist = document.getElementsByClassName("view-dropdown-list");
    viewdropdownlist[0].className = "dropdown-list";
    viewdropdown[0].className = "dropdown";
  }

  showMenu() {
    if (this.dropdownChange) {
      this.buttonChange = false;
      const redElements = document.getElementsByClassName("red");
      const textElements = document.querySelectorAll(".text");
      const blackDiv = document.getElementById("black");
      const dropdown = document.getElementsByClassName("dropdown");
      document.getElementById("logo")?.classList.add("not-display");

      if (this.jwt != "") {
        dropdown[0].className = "undisplay-dropdown";
      }

      for (let i = 0; i < redElements.length; i++) {
        redElements[i].className = "redVisible";
      }

      textElements.forEach((text) => {
        text.className = "textVisible";
      });

      if (blackDiv) {
        blackDiv.id = "blackVisible";
      }
    }
  }

  closeMenu() {
    try {
      this.buttonChange = true;
      const redVisibleElements = document.getElementsByClassName("redVisible");
      const textVisibleElements = document.querySelectorAll(".textVisible");
      const blackVisibleDiv = document.getElementById("blackVisible");
      const undisplaydropdown = document.getElementsByClassName("undisplay-dropdown");
      document.getElementById("logo")?.classList.remove("not-display");
      const screenWidth = window.innerWidth;

      if (this.jwt != "") {
        undisplaydropdown[0].className = "dropdown";
      }

      if (screenWidth > 400) {
        for (let i = 0; i < redVisibleElements.length; i++) {
          redVisibleElements[i].className = "red";
        }
        textVisibleElements.forEach((textVisible) => {
          textVisible.className = "text";
        });
        if (blackVisibleDiv) {
          blackVisibleDiv.id = "black";
        }
      }

      for (let i = 0; i < redVisibleElements.length; i++) {
        redVisibleElements[i].className = "red";
      }
      textVisibleElements.forEach((textVisible) => {
        textVisible.className = "text";
      });

      if (blackVisibleDiv) {
        blackVisibleDiv.id = "black";
      }
    }
    catch (error) { }
  }

  @HostListener('window:resize')
  onResize() {
    this.closeMenu();
  }
}
