import { Component } from '@angular/core';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent 
{
  onButtonsClick(add : boolean)
  {
    if(add)
    {
      document.getElementById("container")?.classList.add("toggle")
    }
    else
    {
      document.getElementById("container")?.classList.remove("toggle")
    }
  }
}
