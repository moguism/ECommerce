import { Component, HostListener, inject } from '@angular/core';
import { LoginComponent } from '../../pages/login/login.component';
import { Router, RouterModule } from '@angular/router';
import { RegisterService } from '../../services/register.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [LoginComponent, RouterModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.css'
})
export class HeaderComponent {
  protected buttonChange: boolean = true;
  protected dropdownChange: boolean = true;
  protected jwt : string = "";
  protected name : string = "";


  constructor(private registerService: RegisterService, private router: Router){
    this.jwt = this.registerService.jwt;
    if(this.jwt != "")
    {
      this.name = JSON.parse(window.atob(this.jwt.split('.')[1])).name;
    }
  }

  deleteToken()
  {
    this.registerService.deleteToken()
    this.router.navigateByUrl("")
  }
  
  goToRoute(route : string)
  {
    this.router.navigateByUrl(route)
  }

  showDropdown() {
    this.dropdownChange = false;
    const dropdown = document.getElementsByClassName("dropdown");
    const dropdownlist = document.getElementsByClassName("dropdown-list");
    dropdownlist[0].className = "view-dropdown-list";
    dropdown[0].className = "view-dropdown";
  }

  closeDropdown(){
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
      
      dropdown[0].className = "undisplay-dropdown";

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
    this.buttonChange = true;
    const redVisibleElements = document.getElementsByClassName("redVisible");
    const textVisibleElements = document.querySelectorAll(".textVisible");
    const blackVisibleDiv = document.getElementById("blackVisible");
    const undisplaydropdown = document.getElementsByClassName("undisplay-dropdown");
    const screenWidth = window.innerWidth;

    undisplaydropdown[0].className = "dropdown";

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

  @HostListener('window:resize')
  onResize() {
    this.closeMenu();
  }
}
