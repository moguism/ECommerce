import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent implements OnInit
{  

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

  }
  

}
