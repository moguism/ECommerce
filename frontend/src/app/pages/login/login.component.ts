import { Component, OnInit } from '@angular/core';
import { __values } from 'tslib';
import { User } from '../../models/user';
import { Login } from '../../models/login';
import { FormsModule } from '@angular/forms';
import { RegisterService } from '../../services/register.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
  constructor(private registerService: RegisterService, private router: Router) {
  }

  name = "";
  email = "";
  password = "";
  address = "";
  role = "user";

  signUpPath = "Auth/signup";
  loginPath = "Auth/login";

  rememberUser = false;

  async ngOnInit(): Promise<void> {

    if (this.registerService.jwt != "") {
      console.log(this.registerService.jwt)
      this.router.navigateByUrl("user")
      return
    }
    else
    {
      console.log(this.registerService.jwt)
    }

    let container = document.querySelector(".container")
    let sign_in_button = document.getElementById("btn-sign-in")
    let sign_up_button = document.getElementById("btn-sign-up")

    if (sign_in_button != null) {
      sign_in_button.addEventListener("click", () => {
        if (container != null)
          container.classList.remove("toggle");
      });
    }

    if (sign_up_button != null) {

      sign_up_button.addEventListener("click", () => {
        if (container != null)
          container.classList.add("toggle");
      });
    }
  }

  async loginUser(): Promise<void> {
    // Para que no sean nulos y eliminen los espacios en blanco
    if (this.email && this.password && this.email.trim() && this.password.trim()) {
      const login = new Login(this.email.trim(), this.password.trim())
      await this.registerService.registerUser(this.loginPath, login)
      if(this.registerService.jwt != "")
      {
        if(this.rememberUser)
        {
          console.log("Recordando al usuario...")
          localStorage.setItem("token", this.registerService.jwt)
        }
        else
        {
          console.log("No recordando al usuario...")
          localStorage.removeItem("token") // Por si el usuario cierra sesión y vuelve a abrirla pero sin recordar
        }
        this.router.navigateByUrl("user")
      }
      else
      {
        alert("Los datos introducidos no son correctos")
      }
    }
    else {
      alert("No puede haber campos vacíos")
    }
  }

  async registerUser(): Promise<void> {
    if (this.name && this.name.trim() && this.email && this.email.trim() && this.password && this.password.trim() && this.address && this.address.trim() && this.role && this.role.trim()) {
      let user = new User(this.name.trim(), this.email.trim(), this.password.trim(), this.address.trim(), this.role.trim());
      await this.registerService.registerUser(this.signUpPath, user);
      this.loginUser()
    }
    else {
      alert("No puede haber campos vacíos")
    }
  }


    /*  
    ngOnDestroy(): void {
      // Cuando este componente se destruye hay que cancelar la suscripción.
      // Si no se cancela se seguirá llamando aunque el usuario no esté ya en esta página
  
      this.routeParamMap$?.unsubscribe();
    }
    */
  
}
