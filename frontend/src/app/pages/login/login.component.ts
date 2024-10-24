import { Component, OnInit } from '@angular/core';
import { __values } from 'tslib';
import { User } from '../../models/user';
import { FormsModule } from '@angular/forms';
import { RegisterService } from '../../services/register.service';



@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit
{  


  constructor(private registerService : RegisterService){

  }

    name = "";
    email = "";
    password = "";
    address = "";
    role = "user";

    path = "Auth/signup";

  ngOnInit(): void {

    let container  = document.querySelector(".container")
    let sign_in_button  = document.getElementById("btn-sign-in") 
    let sign_up_button = document.getElementById("btn-sign-up")

    if(sign_in_button != null){

        sign_in_button.addEventListener("click",()=>{
          if(container != null)
            container.classList.remove("toggle");
        });
    }

    if(sign_up_button != null){

      sign_up_button.addEventListener("click",()=>{
        if(container != null)
          container.classList.add("toggle");
      });
    }

    //Botón para registrar a un usuario
    let register_button = document.getElementById("register_button");

    


    

    if(register_button != null)
    {
      register_button.addEventListener("click", () =>
      {
        //Usuario con los datos introducidos
        let user = new User(this.name,this.email,this.password,this.address,this.role);

        this.registerService.registerUser(this.path,user);
        
      });
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
