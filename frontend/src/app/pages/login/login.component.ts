import { Component, OnInit } from '@angular/core';
import { __values } from 'tslib';
import { User } from '../../models/user';
import { Login } from '../../models/login';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { Router } from '@angular/router';
import { NgIf } from '@angular/common';
import { UserService } from '../../services/user.service';
import { HeaderComponent } from '../../components/header/header.component';
import { ShoppingCartService } from '../../services/shopping-cart.service';
//import { HeaderComponent } from '../../components/header/header.component';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule, NgIf, HeaderComponent],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit {
  constructor(private apiService: ApiService, private router: Router, 
    private formBuilder: FormBuilder, private userService : UserService, private shoppingCartService: ShoppingCartService) {

    this.registerForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      name: ['', Validators.required],
      password: ['', [Validators.required]],
      address: ['', Validators.required],
      confirmPassword: ['', Validators.required]
    },
    { validators: this.passwordMatchValidator })

    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]]
    })
  }

  passwordMatchValidator(form: FormGroup) {
    const password = form.get('password')?.value;
    const confirmPasswordControl = form.get('confirmPassword');
    const confirmPassword = confirmPasswordControl?.value;
     
    if (confirmPasswordControl != null) {
      if (password !== confirmPassword) {
        confirmPasswordControl.setErrors({ mismatch: true });
      }
    }
  }

  registerForm: FormGroup;
  loginForm: FormGroup;

  name = "";
  email = "";
  password = "";
  address = "";
  role = "user";

  signUpPath = "Auth/signup";
  loginPath = "Auth/login";

  rememberUser = false;

  async ngOnInit(): Promise<void> {
    if (this.apiService.jwt != "") {
      console.log("Tiene cuenta: ", this.apiService.jwt)
      this.router.navigateByUrl("user")
      return
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
    if (this.loginForm.valid) {
      const login = new Login(this.email.trim(), this.password.trim())
      await this.apiService.post(this.loginPath, login)
      if (this.apiService.jwt != "") {
        await this.rememberFunction()
      }
      else {
        alert("Los datos introducidos no son correctos")
      }
    }
    else {
      alert("Campos inválidos.")
    }
  }

  async rememberFunction()
  {
    if(this.apiService.jwt == null) { return; }
    if (this.rememberUser) {
      console.log("Recordando al usuario...")
      localStorage.setItem("token", this.apiService.jwt)
      //localStorage.setItem("remember", "true")
    }
    else {
      console.log("No recordando al usuario...")
      sessionStorage.setItem("token", this.apiService.jwt)
      //localStorage.removeItem("token") // Por si el usuario cierra sesión y vuelve a abrirla pero sin recordar
    }
    const goToCheckout = localStorage.getItem("goToCheckout")
    if(goToCheckout && goToCheckout == "true")
    {
      this.router.navigateByUrl("shopping-cart")
    }
    else
    {
      this.router.navigateByUrl("user")
      await this.shoppingCartService.syncronizeCart(true)
    }


    this.userService.userName = this.name
    console.log("USuario : "  + this.userService.userName)
  }

  async registerUser(): Promise<void> {
    if(this.registerForm.controls['password'].value != this.registerForm.controls['confirmPassword'].value){
      alert("Las  contraseñas tienen que ser iguales");
    }else if (this.registerForm.valid) {
      let user = new User(0, this.name.trim(), this.email.trim(), this.password.trim(), this.address.trim(), this.role.trim());
      await this.apiService.post(this.signUpPath, user);
      if (this.apiService.jwt != "") {
        await this.rememberFunction()
        this.userService.userName = this.name

      }
    }else {
        alert("Campos inválidos.");
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
